using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    [Serializable]
    public class Pool
    {
        public string tag;
        public GameObject prefab;
        public int size;
        public Transform parent;
    }

    public List<Pool> pools;

    public Dictionary<string, Queue<GameObject>> poolDictionary;


    private void Awake()
    {
        Instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        InitiliazePools();
        SpawnRockGameStart();
    }

    private void InitiliazePools()
    {
        poolDictionary = new Dictionary<string, Queue<GameObject>>();

        foreach (Pool pool in pools)
        {
            Queue<GameObject> objectPool = new Queue<GameObject>();

            for (int i = 0; i < pool.size; i++)
            {
                GameObject obj = Instantiate(pool.prefab);
                obj.SetActive(false);
                objectPool.Enqueue(obj);
                obj.transform.SetParent(pool.parent);
            }

            poolDictionary.Add(pool.tag, objectPool);
        }
    }

    public GameObject SpawnFromPool(string tag, Vector3 position, Quaternion rotation)
    {
        GameObject objectToSpawn = poolDictionary[tag].Dequeue();

        objectToSpawn.SetActive(true);
        objectToSpawn.transform.position = position;
        objectToSpawn.transform.rotation = rotation;

        poolDictionary[tag].Enqueue(objectToSpawn);

        return objectToSpawn;
    }

    void SpawnRockGameStart()
    {
        int rockAmount = 5;


        for (int i = 0; i < rockAmount; i++)
        {
            GameObject rock = poolDictionary["Rock"].Dequeue();

            float xPos = Random.Range(-5f, 5f);
            float zPos = Random.Range(-4f, 4f);

            rock.SetActive(true);
            rock.transform.position = new Vector3(xPos, 0f, zPos);

            poolDictionary["Rock"].Enqueue(rock);


        }
    }


}
