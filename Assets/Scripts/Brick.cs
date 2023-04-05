using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class Brick : MonoBehaviour
{
    public float jumpHeight = 1;
    public float gravity = -18;
    private Vector3 target = Vector3.zero;
    private float airTime;

    private Rigidbody rb;
    private BoxCollider col;

    public bool isCollectible;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        col = rb.GetComponent<BoxCollider>();
    }
    public void Jump()
    {
        Physics.gravity = Vector3.up * gravity;
        col.isTrigger = true;
        rb.isKinematic = false;
        rb.useGravity = true;

        float x = Random.Range(-2, 2);
        float y = Random.Range(-2, 2);

        target = new Vector3(x, 0, y);

        rb.velocity = CalculateLaunchData().initialVelocity;
        

        Invoke(nameof(Staticize), airTime);

    }

    LaunchData CalculateLaunchData()
    {
        float displacementY = target.y - rb.position.y;
        Vector3 displacementXZ = new Vector3(target.x - rb.position.x, 0, target.z - rb.position.z);
        airTime = Mathf.Sqrt(-2 * jumpHeight / gravity) + Mathf.Sqrt(2 * (displacementY - jumpHeight) / gravity);

        //Debug.Log(airTime);

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
        col.isTrigger = false;
        rb.isKinematic = true;
        rb.useGravity = false;
        transform.position = new Vector3(transform.position.x, 0, transform.position.z);
        isCollectible = true;
    }
}
