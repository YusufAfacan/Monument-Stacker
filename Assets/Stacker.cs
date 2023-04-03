using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Stacker : MonoBehaviour
{
    [SerializeField] private Transform freeStackPoint;
    private Vector3 stackPointOffset = new Vector3(0,0.2f,0);

    [SerializeField] private List<Brick> stackedBricks = new List<Brick>();

    private int index;

    [SerializeField] private float deployInterval;
    private bool canDeploy;

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        canDeploy = true;
    }

    // Update is called once per frame
    void Update()
    {
        if(deployInterval > 0)
        {
            deployInterval -= Time.deltaTime;
        }
        else
        {
            canDeploy = true;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.GetComponent<Brick>())
        {
            other.transform.SetParent(transform);
            other.transform.SetPositionAndRotation(freeStackPoint.position, transform.rotation);
            stackedBricks.Add(other.GetComponent<Brick>());
            freeStackPoint.position += stackPointOffset;
        }

        
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<MonumentGround>())
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);

            if (stackedBricks.Count > 0 && canDeploy)
            {
                Brick lastBrick = stackedBricks[stackedBricks.Count - 1];
                stackedBricks.RemoveAt(stackedBricks.Count - 1);
                Destroy(lastBrick.gameObject);
                freeStackPoint.position -= stackPointOffset;

                other.GetComponent<MonumentGround>().objectsToBeActivated[index].SetActive(true);
                transform.position = other.GetComponent<MonumentGround>().objectsToBeActivated[index].transform.position + offset;
                canDeploy = false;
                deployInterval = 0.05f;
                index++;
            }

            else if(stackedBricks.Count <= 0)
            {
                transform.position = Vector3.zero;
            }

        }
    }
}
