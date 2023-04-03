using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] private VariableJoystick joystick;

    [SerializeField] private Animator animator;


    private const string RUNNING = "Running";
    private const string DYNIDLE = "DynIdle";

    [SerializeField]
    private float MovementSpeed;
    [SerializeField]
    private float RotationSpeed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.GetComponent<Brick>())
        {
            


            CollectBrick();
        }
    }

    private void CollectBrick()
    {
        
    }


    // Update is called once per frame
    void Update()
    {
        Vector3 inputVector = new(joystick.Horizontal, 0, joystick.Vertical);
        inputVector = inputVector.normalized;

        if (inputVector.magnitude > 0)
        {
            animator.SetTrigger(RUNNING);
        }
        else
        {
            animator.SetTrigger(DYNIDLE);
        }


        Vector3 movementVector = MoveTowardTarget(inputVector);

        RotateTowardMovementVector(movementVector);
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
