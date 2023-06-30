using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class Monument : MonoBehaviour
{
    public List<GameObject> bricksToBeActivated;
    public int nextBrickIndex;
    private ObjectPooler pooler;

    private void Awake()
    {
        nextBrickIndex = 0;

        for (int i = 0; i < transform.childCount; i++)
        {
            bricksToBeActivated.Add(transform.GetChild(i).gameObject);
            
            if (transform.GetChild(i).gameObject.activeSelf)
            {
                nextBrickIndex++;
            }
        }
    }

    private void Start()
    {
        pooler = ObjectPooler.Instance;
    }

    public void ActivateBlock()
    {
        bricksToBeActivated[nextBrickIndex].SetActive(true);
        nextBrickIndex++;
    }

    public void DeactivateBlock()
    {
        bricksToBeActivated[nextBrickIndex-1].SetActive(false);
        nextBrickIndex--;

        GameObject spawnedBrick = pooler.SpawnFromPool("Brick", transform.position, Quaternion.identity);
        spawnedBrick.gameObject.SetActive(true);
        spawnedBrick.transform.position = transform.position;

        float x = Random.Range(-4, 4);
        float z = Random.Range(-3, 3);
        Vector3 jumpPos = new Vector3(x, 0, z) + transform.position;

        spawnedBrick.transform.DOJump(jumpPos, 1, 1, 0.5f).OnComplete(() =>
        {
            spawnedBrick.GetComponent<Brick>().isCollectible = true;
        });
    }
}
