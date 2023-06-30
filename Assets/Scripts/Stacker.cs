using System;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Random = UnityEngine.Random;
using System.Collections;

public class Stacker : MonoBehaviour
{
    public event EventHandler OnBrickCollected;
    
    private ObjectPooler pooler;

    public List<Transform> collectedBricks;
    private bool canCollectBrick;

    public Monument monument;
    public DeploymentGround deploymentGround;
    private Vector3 brickOffset = new Vector3(0,0.2f,0);

    private bool canDeploy;
    public bool hasBomb;
    public bool hasSuperJump;
    private const string BOMB = "Bomb";
    private const string SHOE = "Shoe";
    private GameObject ownedBomb;


    public GameObject bombFX;
    public GameObject blastWaveFX;

    private CharacterAnimator animator;
    private CameraControl cameraControl;
    [SerializeField] private float miningInterval;
    [SerializeField] private float miningTime;

    public List<Stacker> otherStackers;

    [HideInInspector] public enum State { Free, Deploying, Jumping, Flairing, Fallen};
    public State state;

    private void Awake()
    {
        canCollectBrick = true;
        canDeploy = true;
        state = State.Free;
        animator = GetComponent<CharacterAnimator>();



    }
    void Start()
    {
        miningTime = miningInterval;

        cameraControl = FindObjectOfType<CameraControl>();
        pooler = ObjectPooler.Instance;
    }

    private void Update()
    {
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hit, 1f))
        {
            if (hit.transform.GetComponent<Rock>() != null)
            {
                Rock rock = hit.transform.GetComponent<Rock>();
                animator.Mining();
                miningTime -= Time.deltaTime;
                if (miningTime <= 0 && rock.isMineable)
                {
                    rock.Mine(this);
                    miningTime = miningInterval;
                }
            }
            
        }

        else
        {
            animator.NotMining();
        }


    }

    private void OnTriggerEnter(Collider other)
    {
        if (collectedBricks.Count <= 0)
        {
            if (other.CompareTag(BOMB) && hasBomb == false)
            {
                ownedBomb = other.gameObject;

                ownedBomb.transform.SetParent(transform);
                canCollectBrick = false;
                ownedBomb.transform.SetLocalPositionAndRotation(LastCollectedBrickPos() + new Vector3(0, 0.5f, 0), Quaternion.identity);
                hasBomb = true;
            }

        }

        if (other.CompareTag(SHOE) && hasSuperJump == false)
        {
            hasSuperJump = true;
            other.gameObject.SetActive(false);
        }

    }

    private void OnTriggerStay(Collider other)
    {
        if (canCollectBrick)
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
        }

        if (other.GetComponent<DeploymentGround>() == deploymentGround)
        {
            if (collectedBricks.Count > 0 && canDeploy)
            {
                DeployBrick();
            }
        }

        if (other.GetComponent<DeploymentGround>() == true)
        {
            if (other.GetComponent<DeploymentGround>() != deploymentGround)
            {
                if (hasBomb)
                {
                    DeploymentGround otherDeploymentGround = other.GetComponent<DeploymentGround>();

                    ownedBomb.transform.SetParent(null);
                    Monument otherMonument = otherDeploymentGround.monument;
                    Vector3 deploymentPos = otherMonument.bricksToBeActivated[otherMonument.nextBrickIndex - 1].transform.position;
                    Vector3 offset = new Vector3(0,0.5f,0);
                    ownedBomb.transform.DOJump(deploymentPos + offset, 2, 1, 0.4f).OnComplete(() =>
                    {
                        Instantiate(bombFX, ownedBomb.transform.position, Quaternion.identity);

                        for (int i = 0; i < 5; i++)
                        {
                            otherMonument.DeactivateBlock();
                            ownedBomb.gameObject.SetActive(false);
                        }
                    });

                    hasBomb = false;
                    canCollectBrick = true;
                    
                }
            }
        }
    }

    

    private void DeployBrick()
    {
        state = State.Deploying;
        canDeploy = false;
        Transform brickToBeDeployed = collectedBricks[^1];
        Vector3 deployPosition = monument.bricksToBeActivated[monument.nextBrickIndex].transform.position;
        brickToBeDeployed.SetParent(pooler.pools[0].parent);
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

    public IEnumerator FallGround()
    {
        state = State.Fallen;
        animator.Fallen();
        yield return new WaitForSeconds(1f);
        state = State.Free;
    }

    private void BlastGround()
    {
        state = State.Free;

        if (hasSuperJump)
        {
            Rock[] rocksInScene = FindObjectsOfType<Rock>();

            foreach (var rock in rocksInScene)
            {
                rock.Mine(null);
            }

            pooler.SpawnFromPool("SuperJumpFX", transform.position, Quaternion.identity);
            hasSuperJump = false;

            otherStackers.ForEach(stacker => { StartCoroutine(nameof(FallGround));});

        }


        int rockSpawnAmount = 1;

        for (int i = 0; i < rockSpawnAmount; i++)
        {
            float x = Random.Range(-3, 3);
            float y = Random.Range(-3, 3);
            Vector3 jumpPos = new Vector3(x, 0.4f, y);

            GameObject rockObj = pooler.SpawnFromPool("Rock", transform.position, Quaternion.identity);
            Rock rock = rockObj.GetComponent<Rock>();
            SphereCollider col = rockObj.GetComponent<SphereCollider>();

            rockObj.SetActive(true);
            col.isTrigger = true;
            rock.reservedBricks = 5;
            rockObj.transform.DOJump(jumpPos, 1, 1, 0.5f).OnComplete(() =>
            {
                col.isTrigger = false;
                rock.isMineable = true;

            });
        }

        

    }

    public Vector3 LastCollectedBrickPos()
    {
        return new Vector3(0, 0.55f, -0.2f) + collectedBricks.Count * brickOffset;
    }
}
