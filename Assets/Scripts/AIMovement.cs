using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.AI;
using Random = UnityEngine.Random;

public class AIMovement : MonoBehaviour
{
    private NavMeshAgent agent;
    private Stacker stacker;
    [SerializeField] private Vector3 deploymentArea;
    private Stacker[] stackersInGame;
    private Vector3 target;
    private CharacterAnimator animator;
    private float decisionDelay;
    private ObjectPooler objectPooler;


    private void Awake()
    {
        animator = GetComponent<CharacterAnimator>();
        agent = GetComponent<NavMeshAgent>();
        stacker = GetComponent<Stacker>();
        
    }

    private void Start()
    {
        objectPooler = ObjectPooler.Instance;
        target = ClosestRock();
        objectPooler.OnRockMined += ObjectPooler_OnRockMined;
        objectPooler.OnRockSpawned += ObjectPooler_OnRockSpawned;
        objectPooler.OnBrickSpawned += ObjectPooler_OnBrickSpawned;
        objectPooler.OnBrickCollected += ObjectPooler_OnBrickCollected;
        
    }

    private void ObjectPooler_OnBrickCollected(object sender, EventArgs e)
    {
        if(stacker.collectedBricks.Count > 10)
        {
            target = deploymentArea;
            


        }

        else if (ClosestBrick() != null && Vector3.Distance(transform.position, ClosestBrick()) <= 1f)
        {
            target = ClosestBrick();


        }
        else
        {
            target = ClosestRock();


        }
    }

    private void ObjectPooler_OnBrickSpawned(object sender, EventArgs e)
    {
        if (Vector3.Distance(transform.position, ClosestBrick()) <= 1f)
        {
            target = ClosestBrick();

        }

    }

    private void ObjectPooler_OnRockSpawned(object sender, EventArgs e)
    {
        target = ClosestRock();


    }

    private void ObjectPooler_OnRockMined(object sender, EventArgs e)
    {
        if (stacker.collectedBricks.Count > 10)
        {
            target = deploymentArea;


        }
        else
        {
            target = ClosestRock();


        }

        
    }

    private void Update()
    {
        if (!agent.pathPending)
        {
            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                if (!agent.hasPath || agent.velocity.sqrMagnitude == 0f)
                {
                    animator.NotRunning();
                }
            }
        }
        if (stacker.canMove)
        {
            agent.SetDestination(target);
        }
        if(!stacker.canMove)
        {
            agent.isStopped = true;
        }


        if (stacker.canMove && !stacker.isMining)
        {
            animator.Running();
        }

    }

    private Vector3 ClosestRock()
    {
        Rock[] rocks = FindObjectsOfType<Rock>();

        if( rocks.Length == 0 )
        {
            return new Vector3( Random.Range(-3,3), 0, Random.Range(-3,3));
        }

        Rock closestRock = null;

        float distance = Mathf.Infinity;

        foreach (Rock rock in rocks)
        {
            if (Vector3.Distance(transform.position, rock.transform.position) <= distance)
            {
                distance = Vector3.Distance(transform.position, rock.transform.position);
                closestRock = rock;
            }

        }

        return closestRock.transform.position;

    }

    private Vector3 ClosestBrick()
    {
        Brick[] bricks = FindObjectsOfType<Brick>();

        if (bricks.Length == 0)
        {
            return new Vector3(Random.Range(-3, 3), 0, Random.Range(-3, 3));
        }

        Brick closestBrick = null;

        float distance = Mathf.Infinity;

        foreach (Brick brick in bricks)
        {
            if (Vector3.Distance(transform.position, brick.transform.position) <= distance)
            {
                distance = Vector3.Distance(transform.position, brick.transform.position);
                closestBrick = brick;
            }
        }

        return closestBrick.transform.position;
    }
}
