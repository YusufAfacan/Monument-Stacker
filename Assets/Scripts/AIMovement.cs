using DG.Tweening;
using System;
using UnityEngine;
using Random = UnityEngine.Random;

public class AIMovement : MonoBehaviour
{

    private Stacker stacker;
    [SerializeField] private Transform deploymentArea;
    private Stacker[] stackersInGame;
    private Vector3 target;
    private CharacterAnimator characterAnimator;
    private float decisionDelay;

    private void Awake()
    {
        characterAnimator = GetComponent<CharacterAnimator>();
        stacker = GetComponent<Stacker>();
    }

    // Start is called before the first frame update
    void Start()
    {

        stackersInGame = FindObjectsOfType<Stacker>();

        foreach (Stacker stacker in stackersInGame)
        {
            stacker.OnBrickCollected += Stacker_OnBrickCollected;
            //stacker.OnBrickSpawned += Stacker_OnBrickSpawned;
        }

        stacker.state = Stacker.State.Free;

        MoveToClosestBrick();
    }

    private void Update()
    {
        if (stacker.state == Stacker.State.Jumping)
        {
            return;
        }

        if (stacker.state == Stacker.State.Flairing)
        {
            return;
        }

        if (stacker.state == Stacker.State.Free)
        {
            transform.position = Vector3.MoveTowards(transform.position, target, 5 * Time.deltaTime);
            transform.DOLookAt(target, 0.5f);
            characterAnimator.Running();
        }

        else
        {
            characterAnimator.Idling();
        }
    }

    private void Stacker_OnBrickSpawned(object sender, EventArgs e)
    {
        decisionDelay = Random.Range(0.4f, 0.6f);
        Invoke(nameof(MoveToClosestBrick), decisionDelay);
    }

    private void Stacker_OnBrickCollected(object sender, EventArgs e)
    {
        decisionDelay = Random.Range(0.4f, 0.6f);
        Invoke(nameof(MoveToClosestBrick), decisionDelay);
    }

    private void MoveToClosestBrick()
    {
        if (stacker.state == Stacker.State.Jumping)
        {
            return;
        }

        if (stacker.state == Stacker.State.Flairing)
        {
            return;
        }

        //if (brickPool.BricksOnTheGround().Count == 0)
        //{
        //    MoveToDeploy();
        //}

        else if (stacker.collectedBricks.Count >= 7)
        {
            MoveToDeploy();
        }

        else
        {
            //target = FindClosestBrick();
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<DeploymentGround>() == stacker.deploymentGround)
        {
            characterAnimator.Idling();
        }
    }

    private void MoveToDeploy()
    {
        target = stacker.deploymentGround.transform.position;
    }

    //private Vector3 FindClosestBrick()
    //{
    //    Vector3 closestBrickPos = Vector3.zero;

    //    float lowestDistance = Mathf.Infinity;

    //    foreach (Transform brickOnGround in brickPool.BricksOnTheGround())
    //    {
            
    //        float distance = Vector3.Distance(transform.position, brickOnGround.position);

    //        if (distance < lowestDistance)
    //        {
    //            closestBrickPos = brickOnGround.position;
    //            lowestDistance = distance;
    //        }

    //    }

    //    return closestBrickPos;
    //}
}
