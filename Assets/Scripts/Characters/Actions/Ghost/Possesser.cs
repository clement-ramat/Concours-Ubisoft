using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;
using UnityEngine.Events;

public class Possesser : MonoBehaviour
{
    // ah bah c'est cool, il est déjà ici le ghost dans possesser ! pouce vert !
    // du coup faudra peut être changer plus tard (cf grabber), à voir ~~ Outil Boy
    [SerializeField]
    [Required]
    private Ghost ghost;


    [Header("UnPossess")]
    [SerializeField]
    private LayerMask groundAndWallLayers;

    [SerializeField]
    private LayerMask possessObjectsLayers;

    [Title("Possessing")]
    [SerializeField]
    [ReadOnly]
    protected GameObject possessedObject;

    [Title("Events")]
    [SerializeField]
    private UnityEvent OnPossess;

    [SerializeField]
    private UnityEvent OnUnpossess;

    [Title("Debug")]
    [SerializeField]
    private bool showDebugGizmos = true;

    [SerializeField]
    [ShowIf("showDebugGizmos")]
    private float durationGizmos = 2f;

    public GameObject PossessedObject
    {
        get
        {
            return possessedObject;
        }
    }
    
    public PossessInteraction PossessInteraction
    {
        get
        {
            return possessedObject?.GetComponent<PossessInteraction>();
        }
    }

    public void Possess(GameObject go) {
        
        ghost.SetGhostTangibility(false);

        possessedObject = go;

        PossessInteraction.OnMakeVibration.AddListener(ghost.RpcVibrationEvent);
        PossessInteraction.PossessedBy = this;
        PossessInteraction.OnPossess(); //Possessed object event
        OnPossess?.Invoke(); //Possesser Event

        ghost.RpcPossessUIEvent(possessedObject);
    }

    public bool Unpossess()
    {
        //Check if possessing
        if (!IsPossessing())
        {
            return false; //no possessing
        }


        PossessInteraction.JoystickInteraction(Vector3.zero);

        //Ghost things
        ghost.SetGhostTangibility(true);

        ghost.transform.position = FindClosestValidPosition();

        PossessInteraction.PossessedBy = null;
        PossessInteraction.OnUnpossess();
        OnUnpossess?.Invoke(); //Possesser Event

        //unity event
        ghost.RpcUnpossessUIEvent(possessedObject);

        PossessInteraction.OnMakeVibration.RemoveListener(ghost.RpcVibrationEvent);
        possessedObject = null;

        return true;
    }

    public bool IsPossessing()
    {
        return possessedObject != null;
    }


    /// <summary>
    /// Find the closest position we can go when we unpossess an object. 
    /// </summary>
    /// <returns>Return the closest valid position, transform.position if none is found.</returns>
    private Vector3 FindClosestValidPosition()
    {
        // Find the closest point in the world not in object
        int RaysToShoot = 30;
        float rayCastLength = 3f;

        float incrDegree = 360.0f / RaysToShoot;

        // We find the initial degree to shoot the raycast (it's behind the possessed object direction or forward for heavy objects)
        Vector3 forward = PossessedObject.GetComponentInChildren<PossessInteraction>().ghostUnpossessFromBehindFirst ? PossessedObject.transform.forward : -PossessedObject.transform.forward;
        float initialDegree = Vector3.SignedAngle(new Vector3(1, 0, 0), new Vector3(-forward.x, 0, forward.z).normalized, Vector3.up);

        Vector3 possessedPosition = GetPossessedObjectPosition();

        //We show the starting pos
        if (showDebugGizmos)
        {
            DrawDebugCross(possessedPosition, 0.5f, Color.cyan, durationGizmos);
        }
        for (int i = 0; i < RaysToShoot; i++)
        {

            float curDegree = ((incrDegree * i) + initialDegree) % 360;

            float incrRad = curDegree * (Mathf.PI / 180);

            Vector3 dir = new Vector3(Mathf.Cos(incrRad), 0, Mathf.Sin(incrRad)).normalized;

            RaycastHit hit;

            if (showDebugGizmos)
            {
                Debug.DrawLine(possessedPosition, possessedPosition + dir * rayCastLength, Color.red, durationGizmos);
            }

            // We try to find a wall
            if (!Physics.Raycast(possessedPosition, dir, out hit, rayCastLength, groundAndWallLayers))
            {

                //There is no wall, we can now try if we are outside the possessed object

                RaycastHit possessHit;
                if (showDebugGizmos)
                {
                    Debug.DrawRay(possessedPosition + dir * rayCastLength, -dir, Color.green, durationGizmos);
                }

                if (Physics.SphereCast(possessedPosition + dir * rayCastLength, 2f, -dir, out possessHit, rayCastLength, possessObjectsLayers))
                {

                    Debug.Log(possessHit.transform.gameObject);

                    IObjectInteraction objectInteraction = possessHit.transform.gameObject.GetComponentInParent<IObjectInteraction>();
                    if (objectInteraction != null && objectInteraction.gameObject == PossessInteraction.gameObject)
                    {
                        // We have found the possess object, we can return the value
                        if (showDebugGizmos)
                        {
                            Debug.DrawLine(possessHit.point + dir * 1.0f, possessHit.point + dir * 1.15f, Color.yellow, durationGizmos * 2f);
                            DrawDebugCross(possessHit.point, 0.5f, Color.yellow, durationGizmos);
                        }
                        return possessHit.point + dir * 1.0f;
                    }
                }
            }
        }

        return transform.position;
    }

    private Vector3 GetPossessedObjectPosition()
    {
        Vector3 position;
        Shaker shaker = PossessedObject.GetComponent<Shaker>();
        if (shaker)
        {
            position = shaker.InitialPosition ;
        }
        else
        {
            position = PossessedObject.transform.position;
        }

        if (ghost.GetComponent<LevitatingMovement>())
        {
            position += Vector3.up * ghost.GetComponent<LevitatingMovement>().Height;
        }

        return position;
    }

    private void DrawDebugCross(Vector3 position, float lenghtCross, Color color, float duration)
    {
        Vector3[] directions = new Vector3[]{Vector3.up, Vector3.down, Vector3.left, Vector3.right, Vector3.forward};

        foreach(Vector3 dir in directions)
        {
            Debug.DrawRay(position, dir * lenghtCross, color, duration);
            Debug.DrawRay(position, -dir * lenghtCross, color, duration);
        }
    }
}
