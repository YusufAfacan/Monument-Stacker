using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using System;
using Random = UnityEngine.Random;
using static ObjectPooler;

public class Rock : MonoBehaviour
{
    public bool isMineable;
    public int hitsToSpawnBrick;
    public int currenthit;
    public int reservedBricks = 5;

    public ObjectPooler pooler;
    private void Start()
    {

        pooler = Instance;
    }

    public void Mine(Stacker stacker)
    {
        currenthit++;

        if (currenthit >= hitsToSpawnBrick)
        {
            GameObject brickObj = pooler.SpawnFromPool("Brick", transform.position, Quaternion.identity);
            Brick brick = brickObj.GetComponent<Brick>();


            if(stacker != null )
            {
                brick.isCollectible = false;
                stacker.collectedBricks.Add(brick.transform);
                brickObj.transform.SetParent(stacker.transform);
                brick.transform.SetLocalPositionAndRotation(stacker.LastCollectedBrickPos(), Quaternion.identity);

            }


            reservedBricks--;
            currenthit = 0;
        }

        if (reservedBricks <= 0)
        {
            isMineable = false;
            

            int bombChance = Random.Range(0, 100);
            int superJumpChance = Random.Range(0, 100);

            if(bombChance >= 0)
            {
                pooler.SpawnFromPool("Bomb", transform.position, Quaternion.identity);
            }

            if (superJumpChance >= 101)
            {
                pooler.SpawnFromPool("Shoe", transform.position, Quaternion.identity);
            }


            gameObject.SetActive(false);
        }

        
    }
}
