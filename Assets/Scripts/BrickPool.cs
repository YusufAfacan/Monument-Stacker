using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class BrickPool : MonoBehaviour
{
    public GameObject brickPrefab;
    public GameObject[] bricks;

    public GameObject NextFreeBrick()
    {
        for (int i = 0; i < bricks.Length - 1; i++)
        {
            if (bricks[i] != null)
            {
                if (!bricks[i].activeSelf)
                {
                    return bricks[i];
                }
            }
        }

        return InstantiateBrick();
    }

    public GameObject InstantiateBrick()
    {
        GameObject brickIns = Instantiate(brickPrefab, transform.position, Quaternion.identity);
        brickIns.transform.SetParent(transform);
        return brickIns;
    }

    public List<Transform> BricksOnTheGround()
    {
        List<Transform> bricksOnGround = new List<Transform>();

        for (int i = 0; i < transform.childCount; i++)
        {
            Transform childBrick = transform.GetChild(i);

            if (childBrick.gameObject.activeSelf)
            {
                if (childBrick.GetComponent<Brick>().isCollectible == true)
                {
                    bricksOnGround.Add(childBrick);
                } 
            }
        }
        
        return bricksOnGround;
    }
}
