using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    [SerializeField] private VariableJoystick joystick;

    [SerializeField]
    private float MovementSpeed;
    [SerializeField]
    private float RotationSpeed;

    private Stacker stacker;

    private CharacterAnimator animator;
    private CharacterController controller;

    private void Awake()
    {
        controller = GetComponent<CharacterController>();
        stacker = GetComponent<Stacker>();
        animator = GetComponent<CharacterAnimator>();
    }

    void Update()
    {
        if (stacker.state == Stacker.State.Jumping)
        {
            return;
        }
        if (stacker.state == Stacker.State.Flairing)
        {
            return;
        }

        Vector3 inputVector = new(joystick.Horizontal, 0, joystick.Vertical);

        if(inputVector.magnitude > 0)
        {
            controller.Move(MovementSpeed * Time.deltaTime * inputVector);

            Vector3 pos = transform.position;
            pos.y = 0;
            transform.position = pos;

            Quaternion toRotation = Quaternion.LookRotation(inputVector, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, toRotation, RotationSpeed * Time.deltaTime);

            animator.Running();
            animator.NotMining();
        }
        else
        {
            animator.NotRunning();
            
        }
    }


    
}
