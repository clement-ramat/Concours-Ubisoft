using System;
﻿using Sirenix.OdinInspector;
﻿using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterMovementOffline : MonoBehaviour
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



    [Title("Debug Info")]

    [ReadOnly]
    [SerializeField]
    private bool canMove = true;

    [ReadOnly]
    [SerializeField]
    private Vector3 moveInput = Vector3.zero;

    private Rigidbody rb;

    private Vector3 moveVelocity;

    private Vector3 currentVelocity;

    private NetworkIdentity networkIdentity;

    private Vector3 moveDirection;
 

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

    public Vector3 MoveInput
    {
        set
        {
            moveInput = value;
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        //networkIdentity = GetComponent<NetworkIdentity>();
        rb = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {


        if (!canMove)
        {
            moveInput = Vector3.zero;
        }

        moveInput = new Vector3(-Input.GetAxis("Horizontal"), -Input.GetAxis("Vertical"), 0);

        // We set the right speed and movement smoothdamp based on state
        float currentSpeed = moveSpeed;
        float currentSmoothTime = smoothTime;

        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);

        // We smoothdamp the movement
        moveVelocity = Vector3.SmoothDamp(moveVelocity, moveDirection.normalized * currentSpeed, ref currentVelocity, currentSmoothTime);
    }


    private void FixedUpdate()
    {

        if (moveInput.magnitude > 0.25f)
        {
            // We rotate based on the input   
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), turnSpeed);
        }


        if (moveInput.magnitude > 0.25f || !clearVelocityWhenNoMoveInput)
        {
            // Move the player to it's current position plus the movement.
            moveVelocity.y = 0f;
            //rb.MovePosition(transform.position + moveVelocity * Time.deltaTime);
            //newPos = Vector3.SmoothDamp(rb.position, targetPosition, ref moveVelocity, 1f / verticalSpeed);

            //rb.MovePosition(newPos);
            rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
        }


        // We apply our constraints
        Vector3 newRotation;
        newRotation.x = constraints.xRotation ? 0 : transform.rotation.eulerAngles.x;
        newRotation.y = constraints.yRotation ? 0 : transform.rotation.eulerAngles.y;
        newRotation.z = constraints.zRotation ? 0 : transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(newRotation);
    }


    public void ResetVelocity()
    {
        moveInput = Vector3.zero;
    }
}
