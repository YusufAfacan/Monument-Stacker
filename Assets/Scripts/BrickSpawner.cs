using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BrickSpawner : MonoBehaviour
{
    [SerializeField] private GameObject brickPrefab;
    [SerializeField] private Transform brickParent;

    [SerializeField] private int column;
    [SerializeField] private int row;
    [SerializeField] private int spacing;

    // Start is called before the first frame update
    void Start()
    {
        InitialSpawn();
    }

    private void InitialSpawn()
    {
        Vector3 spawnPos;

        for (int x = -column / 2; x < column / 2; x++)
        {
            for (int z = -row / 2; z < row / 2; z++)
            {
                spawnPos = new Vector3(x, 0, z) * spacing;

                GameObject initialBrick = Instantiate(brickPrefab, spawnPos, Quaternion.identity);
                initialBrick.transform.SetParent(brickParent);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
