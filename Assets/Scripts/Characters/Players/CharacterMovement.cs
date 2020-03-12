using System;
using Sirenix.OdinInspector;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class CharacterMovement : NetworkBehaviour
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

    public float moveDeadZone = 0.20f;


    [Header("Rotation Settings")]
    public float turnSpeed = 1f;
    public Constraints constraints;

    private bool isMoving = false;
    public UnityEvent OnStep;

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
    private Vector3 moveDirection;

    private Animator myAnimator;

    private float rotationAmount = 0.0f;
    private float defaultMoveSpeed;

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
        rb = GetComponent<Rigidbody>();
        myAnimator = GetComponentInChildren<Animator>();

        defaultMoveSpeed = moveSpeed;
    }

    // Update is called once per frame
    void Update()
    {

        if (!isServer)
        {
            return;
        }


        if (!canMove)
        {
            moveInput = Vector3.zero;
        }

        // We set the right speed and movement smoothdamp based on state
        float currentSpeed = moveSpeed;
        float currentSmoothTime = smoothTime;

        moveDirection = new Vector3(moveInput.x, 0, moveInput.y);
        moveDirection = moveDirection.magnitude > moveDeadZone ? moveDirection : Vector3.zero;

        float inputMagnitude = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.y);
        float inputAmount = Mathf.Clamp01(inputMagnitude);

        // We smoothdamp the movement
        moveVelocity = Vector3.SmoothDamp(moveVelocity, moveDirection.normalized * currentSpeed * inputAmount, ref currentVelocity, currentSmoothTime);

        if (moveDirection.magnitude > moveDeadZone)
        {
            // We rotate based on the input   
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(moveDirection), turnSpeed * Time.deltaTime);
        }

        // We apply our constraints
        Vector3 newRotation;
        newRotation.x = constraints.xRotation ? 0 : transform.rotation.eulerAngles.x;
        newRotation.y = constraints.yRotation ? 0 : transform.rotation.eulerAngles.y;
        newRotation.z = constraints.zRotation ? 0 : transform.rotation.eulerAngles.z;

        transform.rotation = Quaternion.Euler(newRotation);

        //if (currentVelocity.magnitude >= 0.2)
        //{
        //    if (!isMoving)
        //    {
        //        isMoving = true;
        //        OnStartMoving?.Invoke();
        //    }
        //} else
        //{
        //    if (isMoving)
        //    {
        //        isMoving = false;
        //        OnEndMoving?.Invoke();
        //    }
            
        //}


        UpdateAnimator();
    }


    private void FixedUpdate()
    {
        if (!isServer)
        {
            return;
        }


        if (moveInput.magnitude < moveDeadZone && clearVelocityWhenNoMoveInput)
        {
            moveVelocity = Vector3.zero;
        }

        // Move the player to it's current position plus the movement.
        rb.velocity = new Vector3(moveVelocity.x, rb.velocity.y, moveVelocity.z);
    }


    public void ResetVelocity()
    {
        moveInput = Vector3.zero;
    }

    /// <summary>
    /// Reset the move speed with the default move speed.
    /// </summary>
    public void ResetMoveSpeed()
    {
        moveSpeed = defaultMoveSpeed;
    }

    private void UpdateAnimator()
    {
        if (myAnimator == null)
        {
            return;
        }

        Vector3 move = transform.InverseTransformDirection(moveDirection);
        move = moveDirection.magnitude > moveDeadZone ? Vector3.ProjectOnPlane(move, Vector3.up) : Vector3.zero;


        // update the animator parameters
        myAnimator.SetFloat("Forward", (moveVelocity.magnitude / defaultMoveSpeed));
        myAnimator.SetFloat("Turn", Mathf.Atan2(move.x, move.z), 0.1f, Time.deltaTime);
    }

    //[ClientRpc]
    private void Step(AnimationEvent animEvent)
    {
        if (animEvent.animatorClipInfo.weight > 0.2)
        {
            OnStep?.Invoke();
        }
        //Debug.Log("Step + " + gameObject.name + " : " + rb.velocity.magnitude);
        //if (rb.velocity.magnitude > 0.1f)
        //{
        //    Debug.Log("StepEvent + " + gameObject.name);
        //    OnStep?.Invoke();
        //}
    }

    
}
