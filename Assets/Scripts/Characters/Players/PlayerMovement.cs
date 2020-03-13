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
    [Header("Rewired Settings")]
    [SerializeField]
    [Range(0, 3)]
    private int inputID;
    


    [Header("Rotation Settings")]
    [SerializeField]
    private float rotateSpeed;


    [Header("Levitation Settings")]
    [SerializeField]
    private float targetHeight = 1f;
    [SerializeField]
    private float fallSpeed = 1f;
    [SerializeField]
    private float ElevationSpeed = 1f;

    [Header("Movement Settings")]

    [SerializeField]
    private bool movementBasedOnCamera = true;
    [SerializeField]
    private float moveSpeed = 6f;
    [SerializeField]
    private float movementSmoothTime = 0.15f;

    [SerializeField]
    private bool resetVelocityWhenNoInput = false;

    [SerializeField]
    private float gravityScale = 1f;

    [Header("Ground Raycast Settings")]
    [SerializeField]
    private LayerMask groundLayers;

    [SerializeField]
    private Vector3 raycastOffsets = new Vector3();

    [SerializeField]
    // width of raycasts around the center of your character
    private float raycastWidth = 0.35f;


    private Rigidbody myRigidbody;

    private Vector3 moveVelocity;
    private float inputAmount;

    private Vector3 moveInput;

    private Vector3 gravitySpeed = Vector3.zero;
    private Vector3 currentVelocity;


    private float aveDist = 0f;

    private void Awake()
    {
        myRigidbody = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        // - INPUT DETECTION

        // get vertical and horizontal movement input (controller and WASD/ Arrow Keys)
        moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical"));

        // make sure the input doesnt go negative or above 1;
        float inputMagnitude = Mathf.Abs(moveInput.x) + Mathf.Abs(moveInput.z);
        inputAmount = Mathf.Clamp01(inputMagnitude);

        // base movement on camera
        Vector3 correctedVertical = moveInput.z * Camera.main.transform.forward;
        Vector3 correctedHorizontal = moveInput.x * Camera.main.transform.right;

        Vector3 combinedInput = movementBasedOnCamera ? correctedHorizontal + correctedVertical : moveInput;

        Vector3 direction = new Vector3(combinedInput.normalized.x, 0, combinedInput.normalized.z);

        // if the input are too low and we dont want to reset the velocity, dont smooth based on input
        float smoothTime = !resetVelocityWhenNoInput && inputAmount < 0.15f ? movementSmoothTime : movementSmoothTime * inputAmount;

        // We smoothdamp the movement
        moveVelocity = Vector3.SmoothDamp(moveVelocity, direction.normalized * inputAmount * moveSpeed, ref currentVelocity, smoothTime);

        // rotate player to movement direction
        Quaternion rot = Quaternion.LookRotation(moveInput);
        Quaternion targetRotation = Quaternion.Slerp(transform.rotation, rot, Time.fixedDeltaTime * inputAmount * rotateSpeed);
        transform.rotation = targetRotation;
    }

    private void FixedUpdate()
    {

        float distanceHit;
        bool isGrounded = FloorRaycasts(raycastOffsets.x, raycastOffsets.y, raycastOffsets.z, out distanceHit) != Vector3.zero;
        if (!isGrounded)
        {
            gravitySpeed += Vector3.up * Physics.gravity.y * gravityScale * Time.fixedDeltaTime;
        }

        myRigidbody.velocity = moveVelocity + gravitySpeed;
        
        
    
        // find the Y position via raycasts
        //Vector3 floorMovement = new Vector3(myRigidbody.position.x, floorPosition.y + targetHeight + raycastOffsets.y, myRigidbody.position.z);
        Vector3 floorMovement = new Vector3(myRigidbody.position.x, GetNewHeight(), myRigidbody.position.z);

        // only stick to floor when grounded
        if (isGrounded && floorMovement != myRigidbody.position)
        {
            // move the rigidbody to the floor
            myRigidbody.MovePosition(floorMovement);

            gravitySpeed = Vector3.zero;
        }
    }

    private float GetNewHeight()
    {
        float averageDistance;
        float maxDistance;
        float minDistance;
        float[] floorHeights = GetFloorHeights(out averageDistance, out minDistance, out maxDistance);

        float floorHeight = transform.TransformPoint(raycastOffsets).y - floorHeights[0];


        float desiredDistance = 0f;
        //The character is too high, it needs to fall down toward its targetHeight
        if (minDistance > targetHeight)
        {
            desiredDistance = targetHeight; //To smooth with fallSpeed
        }
        //Else move toward average
        else
        {
            desiredDistance = averageDistance;
        }

        return floorHeight + desiredDistance;
    }


    private float[] GetFloorHeights(out float averageDistance, out float minDistance, out float maxDistance)
    {

        float[] distancesFromFloor = new float[5];
        Vector3[] raycastStartPoints =
        {
            new Vector3(raycastOffsets.x, raycastOffsets.y, raycastOffsets.z),
            new Vector3(raycastOffsets.x + raycastWidth, raycastOffsets.y, raycastOffsets.z),
            new Vector3(raycastOffsets.x - raycastWidth, raycastOffsets.y, raycastOffsets.z),
            new Vector3(raycastOffsets.x , raycastOffsets.y, raycastOffsets.z + raycastWidth),
            new Vector3(raycastOffsets.x , raycastOffsets.y, raycastOffsets.z - raycastWidth),
        };
        

        distancesFromFloor[0] = GetFloorDistance(raycastStartPoints[0]);
        distancesFromFloor[1] = GetFloorDistance(raycastStartPoints[1]);
        distancesFromFloor[2] = GetFloorDistance(raycastStartPoints[2]);
        distancesFromFloor[3] = GetFloorDistance(raycastStartPoints[3]);
        distancesFromFloor[4] = GetFloorDistance(raycastStartPoints[4]);

        averageDistance = 0f;
        minDistance = 0f;
        maxDistance = 0f;
        for (int i = 0; i < raycastStartPoints.Length; i++)
        {
            averageDistance += distancesFromFloor[i];

            if (i == 0)
            {
                minDistance = distancesFromFloor[i];
                maxDistance = distancesFromFloor[i];
            }
            else
            {
                if (minDistance > distancesFromFloor[i])
                {
                    minDistance = distancesFromFloor[i];
                }
                if (maxDistance < distancesFromFloor[i])
                {
                    maxDistance = distancesFromFloor[i];
                }
            }
        }

        averageDistance /= raycastStartPoints.Length;

        aveDist = averageDistance;

        return distancesFromFloor;
    }


    /// <summary>
    /// Get distance between position and the floor
    /// </summary>
    /// <param name="offsetx"></param>
    /// <param name="offsety"></param>
    /// <param name="offsetz"></param>
    /// <returns></returns>
    private float GetFloorDistance(float offsetx, float offsety, float offsetz)
    {
        float distanceHit;
        Vector3 hitPoint = FloorRaycasts(offsetx, offsety, offsetz, out distanceHit);
        if (hitPoint != Vector3.zero)
        {
            return distanceHit;
        }
        else { return 0; }
    }

    private float GetFloorDistance(Vector3 offset)
    {
        return GetFloorDistance(offset.x, offset.y, offset.z);
    }

    /// <summary>
    /// Raycast a line from the transform position with offset to the ground, return the hit position.
    /// </summary>
    /// <param name="offsetx"></param>
    /// <param name="offsety"></param>
    /// <param name="offsetz"></param>
    /// <returns></returns>
    private Vector3 FloorRaycasts(float offsetx, float offsety, float offsetz, out float distance)
    {
        RaycastHit hit;

        // move raycast
        Vector3 raycastFloorPos = transform.TransformPoint(0 + offsetx, 0 + offsety, 0 + offsetz);


        if (Physics.Raycast(raycastFloorPos, -Vector3.up, out hit, groundLayers))
        {
            distance = hit.distance;
            Debug.DrawLine(raycastFloorPos, raycastFloorPos + -Vector3.up * hit.distance, Color.magenta);
            return hit.point;
        }
        else
        {
            distance = 0f;
            return Vector3.zero;
        }
    }

    private void OnDrawGizmos()
    {
        //Draw circle where raycast will start
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + new Vector3(raycastOffsets.x, raycastOffsets.y, raycastOffsets.z), raycastWidth);
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position - Vector3.up * aveDist, 0.2f);
    }
}