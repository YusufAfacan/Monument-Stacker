using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;

public class Stacker : MonoBehaviour
{
    public event EventHandler OnBrickCollected;
    public event EventHandler OnBrickSpawned;

    public List<Transform> collectedBricks;

    public Monument monument;
    public DeploymentGround deploymentGround;
    private Vector3 brickOffset = new Vector3(0,0.2f,0);

    private bool canDeploy;

    private BrickPool brickPool;

    private CharacterAnimator animator;
    private CameraControl cameraControl;

    [HideInInspector] public enum State { Free, Deploying, Jumping, Flairing};
    public State state;

    private void Awake()
    {
        canDeploy = true;
        state = State.Free;
        animator = GetComponent<CharacterAnimator>();
    }
    void Start()
    {
        brickPool = FindObjectOfType<BrickPool>();
        cameraControl = FindObjectOfType<CameraControl>();
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Brick>())
        {
            Brick touchedBrick = other.GetComponent<Brick>();

            if (touchedBrick.isCollectible == true)
            {
                touchedBrick.isCollectible = false;
                collectedBricks.Add(touchedBrick.transform);
                touchedBrick.transform.SetParent(transform);
                touchedBrick.transform.SetLocalPositionAndRotation(LastCollectedBrickPos(), Quaternion.identity);
                touchedBrick.transform.localScale = Vector3.one;
                OnBrickCollected?.Invoke(this, EventArgs.Empty);
            }
        }

        if (other.GetComponent<DeploymentGround>() == deploymentGround)
        {
            if (collectedBricks.Count > 0 && canDeploy)
            {
                DeployBrick();
            }
        }
    }

    private void DeployBrick()
    {
        state = State.Deploying;
        canDeploy = false;
        Transform brickToBeDeployed = collectedBricks[^1];
        Vector3 deployPosition = monument.bricksToBeActivated[monument.nextBrickIndex].transform.position;
        brickToBeDeployed.SetParent(brickPool.transform);
        collectedBricks.Remove(brickToBeDeployed);
        brickToBeDeployed.DOJump(deployPosition, 2, 1, 0.4f).OnComplete(() => FinishDeployBrick(brickToBeDeployed)); 
    }

    private void FinishDeployBrick(Transform brick)
    {
        monument.ActivateBlock();
        brick.DOKill();
        brick.transform.SetPositionAndRotation(Vector3.zero, Quaternion.identity);
        brick.transform.localScale = Vector3.one;
        brick.GetComponent<Brick>().isCollectible = false;
        brick.gameObject.SetActive(false);
        canDeploy = true;
        state = State.Free;

        if (monument.nextBrickIndex >= monument.bricksToBeActivated.Count)
        {
            Win();
            canDeploy = false;
            return;
        }

        if (collectedBricks.Count <= 0)
        {
            JumpSequence();
        }

    }

    private void Win()
    {
        state = State.Flairing;
        transform.DOLookAt(Vector3.zero, 0.5f);
        animator.Flair();
        cameraControl.WinShoot(transform);
        collectedBricks.ForEach(brick => {brick.gameObject.SetActive(false);});
    }

    private void JumpSequence()
    {
        state = State.Jumping;
        Vector3 offset = new Vector3(0, 0.5f, 0);
        Vector3 monumentPos = monument.bricksToBeActivated[monument.nextBrickIndex - 1].transform.position;
        Vector3 jumpPos = monumentPos + offset;

        transform.DOLookAt(monumentPos, 0.5f).OnComplete(() =>
        {
            animator.Jumping();
            transform.DOJump(jumpPos, 2, 1, 1).OnComplete(() =>
            {
                transform.DOLookAt(Vector3.zero, 0.5f).OnComplete(() =>
                {
                    animator.Jumping();
                    transform.DOJump(Vector3.zero, 2, 1, 1).OnComplete(() =>
                    {
                        BlastGround();
                    });
                });
            });
        });
    }

    private void BlastGround()
    {
        state = State.Free;

        int brickSpawnAmount = Random.Range(4, 9);

        for (int i = 0; i < brickSpawnAmount; i++)
        {
            int x = Random.Range(-3, 3);
            int y = Random.Range(-3, 3);
            Vector3 jumpPos = new Vector3(x, 0, y);

            Transform spawnedBrick = brickPool.NextFreeBrick().transform;
            spawnedBrick.gameObject.SetActive(true);
            spawnedBrick.position = transform.position;
            spawnedBrick.DOJump(jumpPos, 1, 1, 0.5f).OnComplete(() =>
            {
                spawnedBrick.GetComponent<Brick>().isCollectible = true;
                OnBrickSpawned?.Invoke(this, EventArgs.Empty);
            });
        }

    }

    private Vector3 LastCollectedBrickPos()
    {
        return new Vector3(0, 0.55f, -0.2f) + collectedBricks.Count * brickOffset;
    }
}
