using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private VariableJoystick joystick;

    [SerializeField]
    private float MovementSpeed;
    [SerializeField]
    private float RotationSpeed;

    private Stacker stacker;

    private CharacterAnimator characterAnimator;

    private void Awake()
    {
        stacker = GetComponent<Stacker>();
        characterAnimator = GetComponent<CharacterAnimator>();
    }

    void Update()
    {
        if (stacker.state == Stacker.State.Jumping)
        {
            return;
        }
        else if (stacker.state == Stacker.State.Flairing)
        {
            return;
        }
        else
        {
            Vector3 inputVector = new(joystick.Horizontal, 0, joystick.Vertical);

            inputVector = inputVector.normalized;

            Vector3 movementVector = MoveTowardTarget(inputVector);

            RotateTowardMovementVector(movementVector);

            if (inputVector.magnitude > 0)
            {
                characterAnimator.Running();
            }
            else
            {
                characterAnimator.Idling();
            }
        }

    }


    private Vector3 MoveTowardTarget(Vector3 targetVector)
    {
        float speed = MovementSpeed * Time.deltaTime;

        Vector3 targetPosition = transform.position + targetVector * speed;
        transform.position = targetPosition;
        return targetVector;
    }

    private void RotateTowardMovementVector(Vector3 movementDirection)
    {
        if (movementDirection.magnitude == 0) { return; }
        var rotation = Quaternion.LookRotation(movementDirection);
        transform.rotation = Quaternion.RotateTowards(transform.rotation, rotation, RotationSpeed);
    }
}
