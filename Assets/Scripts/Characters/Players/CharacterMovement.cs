using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovement : MonoBehaviour
{
    [Serializable]
    public class Constraints
    {
        public bool xRotation = false;
        public bool yRotation = false;
        public bool zRotation = false;
    }

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float smoothTime = 0.25f;
    public bool clearVelocityWhenNoMoveInput = true;


    [Header("Rotation Settings")]
    public float turnSpeed = 1f;
    public Constraints constraints;


    public enum MovementState { NotDashing, WaitingDashCoolDown };


    private Rigidbody myRigidbody;

    private Vector3 moveInput = Vector3.zero;
    private Vector3 moveVelocity;
    private MovementState movementState = MovementState.NotDashing;
    private bool canMove = true;

    private Vector3 currentVelocity;


    public MovementState CurrentMouvementState
    {
        get { return movementState; }
    }

    public Vector3 MoveVelocity
    {
        get { return moveVelocity; }
    }

    public bool CanMove
    {
        get { return canMove; }
        set
        {
            canMove = value;
            if (!canMove && clearVelocityWhenNoMoveInput)
            {
                moveVelocity = Vector3.zero;
            }
        }
    }

    // Start is called before the first frame update
    void Start()
    {

        myRigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {

        if (!canMove)
        {
            moveInput = Vector3.zero;
        }


        // We set the right speed and movement smoothdamp based on state
        float currentSpeed = moveSpeed;
        float currentSmoothTime = smoothTime;

        // We smoothdamp the movement
        moveVelocity = Vector3.SmoothDamp(moveVelocity, moveInput.normalized * currentSpeed, ref currentVelocity, currentSmoothTime);

    }


    private void FixedUpdate()
    {

        if (moveInput.magnitude > 0.25f)
        {
            // We rotate based on the input   
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveInput), turnSpeed * Time.deltaTime);
        }


        if (moveInput.magnitude > 0.25f || !clearVelocityWhenNoMoveInput)
        {
            // Move the player to it's current position plus the movement.
            myRigidbody.MovePosition(myRigidbody.position + moveVelocity * Time.deltaTime);
        }


        // We apply our constraints
        Vector3 newRotation;
        newRotation.x = constraints.xRotation ? 0 : transform.rotation.eulerAngles.x;
        newRotation.y = constraints.yRotation ? 0 : transform.rotation.eulerAngles.y;
        newRotation.z = constraints.zRotation ? 0 : transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(newRotation);
    }

    public void SetMoveInput(Vector3 input)
    {
        moveInput = input;
    }
}
