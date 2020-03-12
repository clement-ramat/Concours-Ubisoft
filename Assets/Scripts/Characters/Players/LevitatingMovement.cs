using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using DG.Tweening;
using Mirror;

[RequireComponent(typeof(Rigidbody))]
public class LevitatingMovement : NetworkBehaviour
{

    

    [SerializeField]
    private float height = 1f;


    [Title("Speeds")]
    [SerializeField]
    private float speed = 4f;

    [SerializeField]
    private float distanceScaler = 1f;

    [Title("Float")]
    [SerializeField]
    private bool isFloating = true;

    [SerializeField]
    [ShowIf("isFloating")]
    private float amplitude = 1f;

    [SerializeField]
    [ShowIf("isFloating")]
    private float period = 1f;

    [SerializeField]
    [ReadOnly]
    [ShowIf("isFloating")]
    private float currentOffset = 0f;

    private Vector3 targetFloating = Vector3.zero;


    [HideIf("isFloating")]
    [Title("Falling")]

    [HideIf("isFloating")]
    [SerializeField]
    private float fallSpeed = 4f;

    [SerializeField]
    [HideIf("isFloating")]
    private float thresholdFall = 1f;

    


    

    [Title("Ground Raycast Settings")]
    [SerializeField]
    private LayerMask groundLayers;

    [SerializeField]
    private Vector3 raycastOffsets = new Vector3();

    [SerializeField]
    // width of raycasts around the center of your character
    private float raycastWidth = 0.35f;

    private Rigidbody rb;

    private Vector3 currentVelocity = Vector3.zero;

    private Vector3 targetPosition = Vector3.zero;

    private bool gravityBeforeEnable;

    [SerializeField]
    [ReadOnly]
    GroundRayCast[] casts;

    private GroundRayCast shortestRay;
    private GroundRayCast centralRay;

    public float Height { get => height; }

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnEnable()
    {
        gravityBeforeEnable = rb.useGravity;
        rb.useGravity = false;
    }

    private void OnDisable()
    {
        rb.useGravity = gravityBeforeEnable;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (isServer)
        {
            Levitate();
        }
    }

    public void ResetPositionToTargetHeight()
    {

        currentOffset = 0f;

        GroundRayCast shortestRay;
        Vector3 pos = GetTargetHeightPosition();

        if (LevitationHasHit())
        {
            transform.position = pos;
        }
        else
        {
            pos = transform.position;
            pos.y += height;
            transform.position = pos;
        }

        currentVelocity = Vector3.zero;
    }
    
    private void Levitate()
    {
        Vector3 pos = GetTargetHeightPosition();

        if (!LevitationHasHit())
        {

            pos = transform.position;
            pos.y += height;
            transform.position = pos;

            currentOffset = 0f;
            currentVelocity = Vector3.zero;
            return;
        }

        targetPosition = pos;

        Vector3 newPos = rb.position;
        float verticalSpeed = 0f;
        float diffWithTarget = Mathf.Abs(rb.position.y - targetPosition.y);

        
        //Too high, go down
        if (shortestRay.distance > height && diffWithTarget > thresholdFall) {
            verticalSpeed = fallSpeed;
        }
        else
        {
            verticalSpeed = speed;
            verticalSpeed *= 1f + diffWithTarget * distanceScaler; //Speed scale with remaining distance
        }


        if (verticalSpeed != 0f)
        {
          
            newPos = Vector3.SmoothDamp(rb.position, targetPosition, ref currentVelocity, 1f / verticalSpeed);

            //If is floating, add offset float
            if (isFloating)
            {
                //float rangeCos = Mathf.Abs(offsetMin - offsetMax) / 2f;
                currentOffset = (amplitude /2f) * Mathf.Cos(((2 * Mathf.PI) / period) * Time.time);

                targetFloating = newPos;
                targetFloating.y += currentOffset;
               
            }
            else
            {
                currentOffset = 0f;
            }

            rb.MovePosition(newPos);
        } 
        
    }

    private Vector3 GetTargetHeightPosition()
    {
        centralRay = FloorRaycasts(raycastOffsets);
        shortestRay = GetShortestGroundCast();

        if (!LevitationHasHit())
        {
            Debug.Log("LEVITATION RAYCAST DIDNT FIND A GROUND", gameObject);
            return transform.position;
        }


        Vector3 target = centralRay.posStart;
        target.y -= (shortestRay.distance - height) - currentOffset;

        return target;
    }

    private bool LevitationHasHit()
    {
        return centralRay.hasHit || shortestRay.hasHit;
    }
    
    private GroundRayCast GetShortestGroundCast()
    {
        GroundRayCast[] circleRaycasts = new GroundRayCast[5];
        Vector3[] raycastStartPoints =
        {
            raycastOffsets,
            new Vector3(raycastOffsets.x + raycastWidth, raycastOffsets.y, raycastOffsets.z),
            new Vector3(raycastOffsets.x - raycastWidth, raycastOffsets.y, raycastOffsets.z),
            new Vector3(raycastOffsets.x , raycastOffsets.y, raycastOffsets.z + raycastWidth),
            new Vector3(raycastOffsets.x , raycastOffsets.y, raycastOffsets.z - raycastWidth),
        };

        for (int i = 0; i < circleRaycasts.Length; i++)
        {
            circleRaycasts[i] = FloorRaycasts(raycastStartPoints[i]);
        }

        casts = circleRaycasts;

        GroundRayCast shortestCast = circleRaycasts[0];
        for (int i = 1; i < circleRaycasts.Length; i++)
        {
            if (circleRaycasts[i].hasHit && !shortestCast.hasHit)
            {
                //case where default is not a valid cast
                shortestCast = circleRaycasts[i];
            }
            else if (circleRaycasts[i].hasHit && circleRaycasts[i].distance < shortestCast.distance)
            {
                shortestCast = circleRaycasts[i];
            }
        }

        return shortestCast;
    }


    /// <summary>
    /// Raycast a line from the transform position with offset to the ground, return the hit position.
    /// </summary>
    /// <param name="offsetx"></param>
    /// <param name="offsety"></param>
    /// <param name="offsetz"></param>
    /// <returns></returns>
    private GroundRayCast FloorRaycasts(Vector3 offsetFromTransform)
    {
        RaycastHit hit;
        GroundRayCast ray;

        // move raycast
        Vector3 raycastStartPoint = transform.TransformPoint(offsetFromTransform);


        if (Physics.Raycast(raycastStartPoint, -Vector3.up, out hit, Mathf.Infinity, groundLayers))
        {
            ray = new GroundRayCast(raycastStartPoint, hit);
            Debug.DrawLine(ray.posStart, hit.point, Color.magenta);
            return ray;
        }
        else
        {
            return new GroundRayCast(raycastStartPoint);
        }
    }

    [System.Serializable]
    struct GroundRayCast
    {
        public readonly Vector3 posStart;
        public readonly Vector3 posHit;
        public readonly float distance;

        public bool hasHit;

        public GroundRayCast(Vector3 startPoint, RaycastHit hit)
        {
            posStart = startPoint;
            posHit = hit.point;
            distance = hit.distance;
            hasHit = true;
        }

        public GroundRayCast(Vector3 startPoint)
        {
            posStart = startPoint;
            posHit = Vector3.zero;
            distance = 0f;
            hasHit = false;
        }


    }

    private void OnDrawGizmos()
    {
        //Doesn't show gizmos if component disabled
        if (!enabled)
        {
            return;
        }

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + raycastOffsets, raycastWidth);

        Gizmos.color = Color.cyan;
        Vector3 drawPos = targetPosition;
        drawPos.y -= currentOffset;
        Gizmos.DrawWireSphere(drawPos + raycastOffsets, 0.1f);

        if (isFloating)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(targetFloating + raycastOffsets, 0.1f);
        }

    }
}
