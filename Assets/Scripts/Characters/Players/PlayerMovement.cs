using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Events;
using Rewired;
using Sirenix.OdinInspector;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMovement : MonoBehaviour
{
    [Serializable]
    public class Constraints
    {
        public bool xRotation = false;
        public bool yRotation = false;
        public bool zRotation = false;
    }

    [Header("Rewired Settings")]
    [Range(0, 3)]
    public int playerInputID = 0;

    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float smoothTime = 0.25f;
    public bool clearVelocityWhenNoMoveInput = true;

    [Header("Dash Settings")]
    public bool canDash = false;

    [ShowIf("canDash", true)]
    public float dashSpeed = 6f;
    [ShowIf("canDash", true)]
    public float dashDuration = 0.75f;
    [ShowIf("canDash", true)]
    public float dashSmoothTime = 0.05f;
    [ShowIf("canDash", true)]
    public float dashCoolDownDuration = 1.5f;

    [Header("Rotation Settings")]
    public float turnSpeed = 1f;
    public Constraints constraints;

    [Header("Events")]
    public UnityEvent OnDashEvent;

    public enum MovementState { NotDashing, Dashing, WaitingDashCoolDown };


    private Rigidbody myRigidbody;

    private Vector3 moveInput;
    private Vector3 moveVelocity;
    private MovementState movementState = MovementState.NotDashing;
    private bool canMove = true;

    private Vector3 currentVelocity;

    private Player playerInput;

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
        playerInput = ReInput.players.GetPlayer(playerInputID);
    }

    // Update is called once per frame
    void Update()
    {

        if (Time.timeScale == 0)
        {
            return;
        }


        if (canMove && movementState != MovementState.Dashing)
        {
            // We find the right velocity accordingly the inputs
            moveInput = new Vector3(playerInput.GetAxisRaw("MoveX"), 0, playerInput.GetAxisRaw("MoveY")).normalized;
        }
        else if (!canMove)
        {
            moveInput = Vector3.zero;
        }


        // We set the right speed and movement smoothdamp based on state
        float currentSpeed = movementState != MovementState.Dashing ? moveSpeed : dashSpeed;
        float currentSmoothTime = movementState != MovementState.Dashing ? smoothTime : dashSmoothTime;

        // We smoothdamp the movement
        moveVelocity = Vector3.SmoothDamp(moveVelocity, moveInput.normalized * currentSpeed, ref currentVelocity, currentSmoothTime);

        if (canMove && canDash && playerInput.GetButtonDown("Dash") && movementState == MovementState.NotDashing)
        {
            movementState = MovementState.Dashing;

            // We set the dash direction by the player's look direction if he's not moving this frame
            if (moveInput.magnitude < 0.25f)
            {
                moveInput = transform.forward;
            }

            if (OnDashEvent != null)
            {
                OnDashEvent.Invoke();
            }

            // Start the coroutine for dealing with states and cooldown
            StartCoroutine(DashCoroutine());
        }
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

    private IEnumerator DashCoroutine()
    {

        yield return new WaitForSeconds(dashDuration);

        movementState = MovementState.WaitingDashCoolDown;

        yield return new WaitForSeconds(dashCoolDownDuration);

        movementState = MovementState.NotDashing;
    }
}