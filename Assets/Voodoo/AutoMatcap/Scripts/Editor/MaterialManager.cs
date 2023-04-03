using System;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

namespace Voodoo.Render
{
	[Serializable]
	public class MaterialManager
	{
		private const float fieldSize = 20f; //EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
		private const int maxVisibleMaterialCount = 10;
		
		private int MatCapId => Shader.PropertyToID("_MatCap");
		private List<MatcapField> matcapFields;
		private MatcapField matcapField;

		private Renderer[] renderers;
		private MaterialViewer materialViewer;
		private SphereMaterialSaver sphereMaterialSaver;
		private SceneAsset currentMatCapScene;
		private DefaultMaterialStorage defaultMaterialStorage;

		private Vector2 scrollPosition;
		
		private bool isVisible = true;
		private bool isDirty;

		private bool Validate
		{
			get
			{
				if (isDirty == false)
				{
					return true;
				}
				
				int choice = EditorUtility.DisplayDialogComplex("Save modifications", "You change the prefab materials. Do you want to apply this change or restore the original state?", "Apply", "Restore", "Cancel");
				if (choice == 2)
				{
					return false;
				}

				if (choice == 1)
				{
					for (var i = 0; i < renderers.Length; i++)
					{
						Renderer renderer = renderers[i];
						Material[] oldMaterial = renderer.sharedMaterials;
						int matIndex = Array.IndexOf(oldMaterial, matcapField.matcapMaterial);
						if (matIndex != -1)
						{
							oldMaterial[matIndex] = matcapField.originalMaterial;
							renderer.sharedMaterials = oldMaterial;
						}
					}
				}

				return true;
			}
		}
		
		private Texture2D matcapTexture;
		public Texture2D MatcapTexture
		{
			get
			{
				if (matcapTexture == null)
				{
					matcapTexture = (Texture2D) matcapField.matcapMaterial.GetTexture(MatCapId);
					AutoMatcapTextureImporter.UpdateTextureImporter(matcapTexture);
				}
				
				return matcapTexture;
			}
		}

		public void Recalculate(GameObject model)
		{
			sphereMaterialSaver = SphereMaterialSaver.Load();
			currentMatCapScene = AssetDatabase.LoadAssetAtPath<SceneAsset>(SceneLoader.autoMatcapScenePath);
			if (defaultMaterialStorage == null)
			{
				defaultMaterialStorage = DefaultMaterialStorage.Find();
			}
			
			matcapFields = new List<MatcapField>();
			isDirty = false;

			if (model == null)
			{
				renderers = null;
				return;
			}
			
			List<Material> modelMaterials = new List<Material>();
			renderers = model.GetComponentsInChildren<Renderer>();
			for (var i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				modelMaterials.AddRange(renderer.sharedMaterials);
			}

			for (var i = 0; i < modelMaterials.Count; i++)
			{
				Material material = modelMaterials[i];
				bool isMatCapMaterial = material != null && material.HasProperty(MatCapId);

				Material originalMaterial = isMatCapMaterial ? null : material;
				Material matcapMaterial = isMatCapMaterial ? material : null;
				MatcapField currentMatcapField = new MatcapField(originalMaterial, matcapMaterial);
				
				if (matcapFields.Exists(x => x.originalMaterial == originalMaterial && x.matcapMaterial == matcapMaterial) == false)
				{
					matcapFields.Add(currentMatcapField);
				}
			}
			
			MatcapFieldSaver.LoadMatcapFields(matcapFields);
		}
		
#region Click methods

		private void OnClick(MatcapField clickedMatcapField)
		{
			if (clickedMatcapField.buttonState == MatCapFieldState.Editing)
			{
				StopEditing();
			}
			else
			{
				if (AutoMatcapWindow.window.editMode)
				{
					ChangeEdition(clickedMatcapField);
				}
				else
				{
					StartEditing(clickedMatcapField);
				}
			}
		}
		
		private void StartEditing(MatcapField newMatcapField)
		{
			if (SceneLoader.OpenMatCapScene() == false)
			{
				return;
			}
			
			AutoMatcapWindow.window.editMode = true;
			
			UpdateCurrentField(newMatcapField);
			UpdateMatcapFieldsButtonState();
			SceneBaker.Instance.FocusSphere();
			
			bool assignSuccessful = AssignMaterialToPrefab(newMatcapField.matcapMaterial);
			if (assignSuccessful == false)
			{
				return;
			}
			
			ResetAndApplySphereProperties();

			MatcapFieldSaver.SaveMatcapField(newMatcapField);
		}
		
		private void ChangeEdition(MatcapField newMatcapField)
		{
			if (Validate == false)
			{
				return;
			}
			
			SaveSphereProperties();
			UpdateCurrentField(newMatcapField);
			UpdateMatcapFieldsButtonState();
			
			bool assignSuccessful = AssignMaterialToPrefab(matcapField.matcapMaterial);
			if (assignSuccessful == false)
			{
				return;
			}
			
			ResetAndApplySphereProperties();

			MatcapFieldSaver.SaveMatcapField(matcapField);
		}
		
		private void StopEditing()
		{
			if (Validate == false)
			{
				return;
			}
			
			SaveSphereProperties();
			UpdateCurrentField(null);
			
			SceneBaker.Instance.ResetSphere();
			AutoMatcapWindow.Quit();
			UpdateMatcapFieldsButtonState();

			isDirty = false;
		}
		
#endregion

		private bool AssignMaterialToPrefab(Material materialToApply)
		{
			//TODO : if current prefab's material is already matcap, return instantly.

			if (materialToApply == null)
			{
				int userAction = EditorUtility.DisplayDialogComplex("Choose matcap material", "There is no matcap material defined for this material. \nDo you want to select an already existing one or create a new one?", "Select existing one", "Create new one", "Cancel");
				string baseMaterialPath = AssetDatabase.GetAssetPath(matcapField.originalMaterial);
				if (string.IsNullOrEmpty(baseMaterialPath))
				{
					baseMaterialPath = Path.Combine(Application.dataPath, "New-Material.mat");
				}

				if (userAction == 2)
				{
					StopEditing();
					return false;
				}
				
				string materialPath = userAction == 0 ?
					EditorUtility.OpenFilePanelWithFilters("Select matcap material", baseMaterialPath, new[]{".mat", "mat"}) :
					EditorUtility.SaveFilePanel("Create matcap material", Path.GetDirectoryName(baseMaterialPath), Path.GetFileNameWithoutExtension(baseMaterialPath) + "_Matcap", "mat");
				
				if (string.IsNullOrEmpty(materialPath))
				{
					StopEditing();
					return false;
				}
				
				materialPath = "Assets" + materialPath.Substring(Application.dataPath.Length);
				Material newMaterial = userAction == 0 ? AssetDatabase.LoadAssetAtPath<Material>(materialPath) : CreateMaterial(materialPath);
				materialToApply = newMaterial;
			}
			
			if (materialToApply.GetTexture(MatCapId) == null)
			{
				StopEditing();
				return false;
			}
				
			for (var i = 0; i < renderers.Length; i++)
			{
				Renderer renderer = renderers[i];
				Material[] newMaterials = renderer.sharedMaterials;
				for (var y = 0; y < newMaterials.Length; y++)
				{
					//Find the right material to edit and verify that its state isn't already the right one before applying modifications
					if ((newMaterials[y] == matcapField.originalMaterial || newMaterials[y] == matcapField.matcapMaterial) && newMaterials[y] != materialToApply)
					{
						newMaterials[y] = materialToApply;
						matcapField.matcapMaterial = materialToApply;
						isDirty = true;
					}
				}
				
				renderer.sharedMaterials = newMaterials;
			}

			return true;
		}
		
		private void UpdateCurrentField(MatcapField newField)
		{
			matcapField = newField;
			matcapTexture = null;
			materialViewer = matcapField != null ? new MaterialViewer(SceneBaker.Instance.SphereMaterial) : null;
		}

		private void SaveSphereProperties()
		{
			if (matcapField.matcapMaterial == null)
			{
				return;
			}
			
			AssetDatabase.TryGetGUIDAndLocalFileIdentifier(matcapField.matcapMaterial, out string matcapId, out long _);
			SphereProperty sp = new SphereProperty(
				matcapId,
				SceneBaker.Instance.SphereMaterial.shader.name,
				MaterialEditor.GetMaterialProperties(SceneBaker.Instance.SphereMaterials));

			sphereMaterialSaver.Add(sp);
			sphereMaterialSaver.Save();
		}

		private void ResetAndApplySphereProperties()
		{
			SceneBaker.Instance.ResetSphere();
			ApplySphereProperties();
		}

		private void ApplySphereProperties()
		{
			if (matcapField.matcapMaterial == null)
			{
				return;
			}
			
			AssetDatabase.TryGetGUIDAndLocalFileIdentifier(matcapField.matcapMaterial, out string matcapId, out long _);
			sphereMaterialSaver.Find(x => x.matcapId == matcapId)?.ApplyTo(SceneBaker.Instance.SphereMaterial);
		}

		private void UpdateMatcapFieldsButtonState()
		{
			for (int i = 0; i < matcapFields.Count; i++)
			{
				bool isBeingEdited = AutoMatcapWindow.window.editMode && matcapField == matcapFields[i];
				matcapFields[i].UpdateButtonState(isBeingEdited);
			}
		}

		private Material CreateMaterial(string materialPath)
		{
			if (matcapField == null)
			{
				return null;
			}

			string shaderName = "MatCap/Vertex/Textured Add";
			Material newMaterial = new Material(Shader.Find(shaderName));
			string texturePath = materialPath.Replace(".mat", "_Texture.png");

			SceneBaker.Instance.Bake(new Texture2D(512, 512), texturePath);
			AutoMatcapTextureImporter.UpdateTextureImporter(texturePath);

			Texture2D tex = AssetDatabase.LoadAssetAtPath<Texture2D>(texturePath);
			newMaterial.SetTexture(MatCapId, tex);
			AssetDatabase.CreateAsset(newMaterial, materialPath);
			SceneBaker.Instance.Bake(tex, texturePath);

			return newMaterial;
		}

		public void DrawCurrentSphereMaterial()
		{
			materialViewer?.OnGUI();
		}

		public void OnGUI()
		{
			if (matcapFields == null || matcapFields.Count == 0)
			{
				return;
			}

			EditorGUI.BeginChangeCheck();
			currentMatCapScene = (SceneAsset)EditorGUILayout.ObjectField("MatCap Scene", currentMatCapScene, typeof(SceneAsset), false);
			if (EditorGUI.EndChangeCheck())
			{
				SceneLoader.autoMatcapScenePath = AssetDatabase.GetAssetPath(currentMatCapScene);
			}

			if (defaultMaterialStorage != null)
			{
				defaultMaterialStorage.OnGUI();
			}

			isVisible = EditorGUILayout.Foldout(isVisible, "Materials", true);
			
			if (isVisible == false)
			{
				return;
			}
			
			EditorGUILayout.BeginVertical("box");
			
			EditorGUILayout.BeginHorizontal();
			EditorGUILayout.LabelField("base", EditorStyles.centeredGreyMiniLabel);
			EditorGUILayout.LabelField("MatCap", EditorStyles.centeredGreyMiniLabel);
			GUILayout.Space(EditorGUIUtility.singleLineHeight);
			EditorGUILayout.EndHorizontal();

			int rowCount = Mathf.Min(matcapFields.Count, maxVisibleMaterialCount);
			scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition, GUILayout.Height(fieldSize * rowCount + 2));
			
			for (int i = 0; i < matcapFields.Count; i++)
			{
				MatcapField isClicked = matcapFields[i].OnGUI();
				if (isClicked != null)
				{
					OnClick(isClicked);
				}
			}
			
			EditorGUILayout.EndScrollView();
			EditorGUILayout.EndVertical();
		}
	}
}