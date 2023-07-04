using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;
using DG.Tweening;

public class ObjectPooler : MonoBehaviour
{
    public static ObjectPooler Instance;

    public event EventHandler OnRockMined;
    public event EventHandler OnRockSpawned;
    public event EventHandler OnBrickSpawned;
    public event EventHandler OnBrickCollected;

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
        InitiliazePools();
        SpawnRockGameStart();
        InvokeRepeating(nameof(SpawnRock), 10f, 5f);
    }

    private void SpawnRock()
    {
        GameObject rock = poolDictionary["Rock"].Dequeue();

        float xPos = Random.Range(-5f, 5f);
        float zPos = Random.Range(-4f, 4f);

        rock.SetActive(true);
        rock.transform.position = new Vector3(xPos, 0.25f, zPos);
        rock.transform.DOShakePosition(0.5f, 0.5f);
        rock.transform.DOShakeRotation(0.5f, 0.5f);
        rock.transform.DOShakeScale(0.5f, 0.5f);


        poolDictionary["Rock"].Enqueue(rock);
        OnRockSpawned?.Invoke(this, EventArgs.Empty);
    }

    // Start is called before the first frame update
    void Start()
    {
        
        
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

        if (objectToSpawn == null)
        {
            Instantiate(objectToSpawn, position, rotation);
        }

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
            SpawnRock();

        }
    }

    public void RockMined(Rock rock)
    {
        rock.transform.SetParent(pools[1].parent);
        rock.gameObject.SetActive(false);
        poolDictionary["Rock"].Enqueue(rock.gameObject);
        OnRockMined?.Invoke(this, EventArgs.Empty);
        
    }

    public void BrickSpawned()
    {
        OnBrickSpawned?.Invoke(this, EventArgs.Empty);
    }

    public void BrickCollected()
    {
        OnBrickCollected?.Invoke(this, EventArgs.Empty);
    }
}
