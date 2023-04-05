using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Stacker : MonoBehaviour
{
    [SerializeField] private GameObject brick;
    [SerializeField] private Transform freeStackPoint;
    private Vector3 stackPointOffset = new Vector3(0,0.2f,0);

    [SerializeField] private List<Brick> stackedBricks = new List<Brick>();

    private int index;

    [SerializeField] private float deployInterval;
    private float deployCountdown;
    private bool canDeploy;

    private Rigidbody rb;

    public float jumpHeight = 25;
    public float gravity = -18;
    private Vector3 target = Vector3.zero;

    private float airTime;

    [SerializeField] private enum State { Loading, Deploying, Unloaded, Jumping, Jumped, Landed };
    [SerializeField] private State state;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        index = 0;
        canDeploy = true;
        state = State.Loading;
    }

    // Update is called once per frame
    void Update()
    {
        if(deployCountdown > 0)
        {
            deployCountdown -= Time.deltaTime;
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
            if (other.GetComponent<Brick>().isCollectible == true)
            {
                other.transform.SetParent(transform);
                other.transform.SetPositionAndRotation(freeStackPoint.position, transform.rotation);
                stackedBricks.Add(other.GetComponent<Brick>());
                freeStackPoint.position += stackPointOffset;
            }

            
            
        }

    }

   

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<MonumentGround>())
        {
            Vector3 offset = new Vector3(0, 0.5f, 0);

            if (stackedBricks.Count > 0 && canDeploy)
            {
                state = State.Deploying;

                Brick lastBrick = stackedBricks[stackedBricks.Count - 1];
                stackedBricks.RemoveAt(stackedBricks.Count - 1);
                Destroy(lastBrick.gameObject);
                freeStackPoint.position -= stackPointOffset;

                if (index < other.GetComponent<MonumentGround>().objectsToBeActivated.Count)
                {
                    other.GetComponent<MonumentGround>().objectsToBeActivated[index].SetActive(true);
                    transform.position = other.GetComponent<MonumentGround>().objectsToBeActivated[index].transform.position + offset;
                    canDeploy = false;
                    deployCountdown = deployInterval;
                    index++;
                }

                else
                {
                    Debug.Log("WIN");
                }
               
                
                
                
            }

            else if(stackedBricks.Count <= 0)
            {

                //state = State.Unloaded;
                
                if (state != State.Jumping)
                {
                    float jumpDelay = 0.6f;

                    Invoke(nameof(Jump), jumpDelay);
                    
                    

                    state = State.Jumping;
                }


            }

        }
    }

    

    private void BlastGround()
    {
        int brickSpawnAmount = UnityEngine.Random.Range(4, 9);

        for (int i = 0; i < brickSpawnAmount; i++)
        {
            GameObject spawnedBrick = Instantiate(brick, target, Quaternion.identity);
            spawnedBrick.GetComponent<Brick>().Jump();
        }
    }

    private void Jump()
    {
        Physics.gravity = Vector3.up * gravity;
        rb.isKinematic = false;
        rb.useGravity = true;

        float x = UnityEngine.Random.Range(-3, 3);
        float y = UnityEngine.Random.Range(-3, 3);

        target = new Vector3(x, 0, y);

        rb.velocity = CalculateLaunchData().initialVelocity;
        state = State.Jumping;

        Invoke(nameof(Staticize), airTime);

    }

    LaunchData CalculateLaunchData()
    {
        float displacementY = target.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(target.x - rb.position.x, 0, target.z - rb.position.z);
        airTime = Mathf.Sqrt(-2 * jumpHeight / gravity) + Mathf.Sqrt(2 * (displacementY - jumpHeight) / gravity);

        Debug.Log(airTime);

        Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * gravity * jumpHeight);
        Vector3 velocityXZ = displacementXZ / airTime;

        return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(gravity), airTime);
    }

    //void DrawPath()
    //{
    //    LaunchData launchData = CalculateLaunchData();
    //    Vector3 previousDrawPoint = rb.position;

    //    int resolution = 30;
    //    for (int i = 1; i <= resolution; i++)
    //    {
    //        float simulationTime = i / (float)resolution * launchData.timeToTarget;
    //        Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * gravity * simulationTime * simulationTime / 2f;
    //        Vector3 drawPoint = rb.position + displacement;
    //        Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
    //        previousDrawPoint = drawPoint;
    //    }
    //}

    struct LaunchData
    {
        public readonly Vector3 initialVelocity;
        public readonly float timeToTarget;

        public LaunchData(Vector3 initialVelocity, float timeToTarget)
        {
            this.initialVelocity = initialVelocity;
            this.timeToTarget = timeToTarget;
        }

    }

    private void Staticize()
    {
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        BlastGround();
    }

}
