using System.IO;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Voodoo.Render
{
    public class SceneBaker : MonoBehaviour
    {
#if UNITY_EDITOR
        [SerializeField] private DefaultMaterialStorage defaultMaterialStorage;
        [SerializeField] private Camera sphereCamera;
        [SerializeField] private Renderer sphere;

        public static SceneBaker Instance => FindObjectOfType<SceneBaker>();
        
        public Material SphereMaterial => sphere.sharedMaterial;

        public Material[] SphereMaterials => sphere.sharedMaterials;

        public void Bake(Texture2D outTexture, string texturePath)
        {
            if (sphereCamera == null)
            {
                Debug.LogError("There is no camera rendering to save");
                return;
            }

            RenderTexture rt = new RenderTexture(outTexture.width, outTexture.height, 24)
            {
                antiAliasing = 8
            };

            sphereCamera.targetTexture = rt;
            sphereCamera.Render();
            RenderTexture.active = rt;

            byte[] originalTextureData = outTexture.GetRawTextureData();
            outTexture.ReadPixels(new Rect(0, 0, outTexture.width, outTexture.height), 0, 0);

            sphereCamera.targetTexture = null;
            RenderTexture.active = null;
            DestroyImmediate(rt);

            byte[] textureData = outTexture.GetRawTextureData();

            bool identical = true;

            if (originalTextureData.Length != textureData.Length)
            {
                identical = false;
            }
            else
            {
                for (int i = 0; i < textureData.Length; i++)
                {
                    if (textureData[i] != originalTextureData[i])
                    {
                        identical = false;
                        break;
                    }
                }
            }

            if (identical)
            {
                return;
            }
            
            FileInfo fileInfo = new FileInfo(texturePath);
            fileInfo.Directory?.Create();

            if (File.Exists(texturePath))
            {
                File.Delete(texturePath);
            }

            byte[] bytes = outTexture.EncodeToPNG();
            File.WriteAllBytes(texturePath, bytes);

            AssetDatabase.Refresh();
        }

        public void ResetSphere()
        {
            //Improve this by saving the value into the component instance in the scene
            if (defaultMaterialStorage == null)
            {
                defaultMaterialStorage = DefaultMaterialStorage.Find();
            }
            
            sphere.sharedMaterial.shader = Shader.Find(defaultMaterialStorage.currentMaterial.shader.name);
            sphere.sharedMaterial.CopyPropertiesFromMaterial(defaultMaterialStorage.currentMaterial);
        }

        //Move the SceneView camera to focus the Sphere
        public void FocusSphere()
        {
            SceneView.lastActiveSceneView.in2DMode = false;
            SceneView.lastActiveSceneView.orthographic = false;
			
            SceneView.lastActiveSceneView.AlignViewToObject(sphereCamera.transform);
			
            Selection.activeObject = sphere.gameObject;
            SceneView.FrameLastActiveSceneView();
            Selection.activeObject = null;
        }
#endif
    }
}