using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;

public class Grabber : MonoBehaviour
{

    // bon c'est pas ouf cette variable mais là on a vraiment besoin d'un système de callback pour que les characters ai des retours sur leurs actions plutôt que ce soit 
    // caché je sais pas où dans grabber ou possesser là, et comme j'ai besoin d'un network behaviour je suis bien embêté :) et surtout fatigué 
    // alors ça sera comme ça pour l'instant et puis c'est tout la bise ~~ Outil Boy
    [SerializeField]
    private Human human;

    [SerializeField]
    private LayerMasks layerMasks;

    [SerializeField]
    private float checkWallLength = 1f;

    [SerializeField]
    private float checkWallRadius = 0.3f;

    [SerializeField]
    private float checkFurnitureRadius = 0.7f;

    [SerializeField]
    private bool actionInProgress = false;

    [SerializeField]
    public float actionDuration = 0.1f;

    [SerializeField]
    private bool bufferThrow = false;

    

    [Title("Grab")]
    [SerializeField][ReadOnly]
    protected GrabInteraction grabbedObject;

    /// <summary>
    /// Nouveau parent de l'objet attrapé
    /// </summary>
    [SerializeField]
    [Required]
    private Transform grabParent;

    [SerializeField]
    private float grabDuration = 0.25f;


    [Title("Un-Grab")]

    /// <summary>
    /// Parent de où l'objet a été pris
    /// </summary>
    [SerializeField]
    [ReadOnly]
    private Transform initialParent;

    [SerializeField]
    [Required]
    private Transform dropPosition;

    [SerializeField]
    private float ungrabDuration = 0.25f;

    [Title("Throw")]

    [SerializeField]
    private float launchPower = 30f;

    private Vector3 finalDropPosition;

    private float movingBackDistance;


    public GrabInteraction GrabbedObject
    {
        get
        {
            return grabbedObject;
        }
    }

    public bool IsGrabbingObject()
    {
        return grabbedObject != null;
    }

    private void Update()
    {
        if (!actionInProgress )
        {
            if (bufferThrow)
            {
                Throw();
                bufferThrow = false;
            }
        }
    }

    public void ManageGrab(InteractionArea interactionArea)
    {
        if (IsGrabbingObject())
        {
            Debug.Log("Ungrab");
            UnGrab();
            //Throw();
            interactionArea.EnableHighlight = true;
        }
        else if (interactionArea.HasInteractable())
        {
            Debug.Log("Grab");
            GrabInteraction grabInteraction = interactionArea.InteractableObject.GetComponent<GrabInteraction>();
            if(grabInteraction != null)
            {
                Grab(grabInteraction);
                interactionArea.EnableHighlight = false;
            }
        }

    }

    public void Throw()
    {
        if (!IsGrabbingObject())
        {
            //Pas d'objets à lancer
            return;
        }
        if (actionInProgress)
        {
            bufferThrow = true;
            return;
        }

        actionInProgress = true;

        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false; //On retabli sa physique
            rb.AddForceAtPosition(grabbedObject.transform.forward * launchPower, 
                new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z), 
                ForceMode.Impulse);

            //OnUngrab?.Invoke();
            human.RpcThrowUIEvent();
            
            // Stops the character movement
            StartCoroutine(transform.root.GetComponent<Human>().DisableCharacterMovementFor(ungrabDuration, false));

            StartCoroutine(ActionEnd(actionDuration));

            grabbedObject.UnGrab();
            grabbedObject = null;

        }
        else
        {
            //Pas de rigidbody, pas de lancer
            return;
        }

        return;
    }

    IEnumerator ActionEnd(float actionDuration)
    {
        yield return new WaitForSeconds(actionDuration);
        actionInProgress = false;
        //grabbedObject.UnGrab();
        //grabbedObject = null;
    }

    public void Grab(GrabInteraction grabInteraction)
    {
        Debug.Log("Grab with grab interact");

        if (IsGrabbingObject())
        {
            Debug.Log("Try to grab while already grabbing something ! Should not be possible?");
            return;
        }

        if (actionInProgress)
        {
            // TODO : add actions to a pile
            return;
        }

        actionInProgress = true;

        grabbedObject = grabInteraction;

        grabInteraction.Grab(gameObject);

        human.RpcGrabUIEvent();

        // Stops the character movement
        StartCoroutine(transform.root.GetComponent<Human>().DisableCharacterMovementFor(grabDuration, false));

        StartCoroutine(ActionEnd(actionDuration));
    }

    /// <summary>
    /// Ungrab l'object actuelle, return true s'il y en avait un, false si aucn
    /// </summary>
    /// <returns></returns>
    public void UnGrab()
    {
        if (grabbedObject == null)
        {
            return;
        }

        if (actionInProgress)
        {
            // TODO : add actions to a pile
            return;
        }

        actionInProgress = true;

        // Update the location of the drop based on walls and furnitures nearby
        bool dropPositionBlocked = UpdateDropPosition();

        //Debug.Log(finalDropPosition);
        // Ungrab and place the object at this location
        grabbedObject.UnGrab(finalDropPosition);

        // Stops the character movement
        StartCoroutine(transform.root.GetComponent<Human>().DisableCharacterMovementFor(ungrabDuration, false));

        StartCoroutine(ActionEnd(actionDuration));

        // If the original drop position is blocked (by wall or furniture)
        if (dropPositionBlocked)
        {
            // Move the human back
            CheckAndMoveHumanBackward();
        }

        //onUngrab?.Invoke();
        human.RpcUngrabUIEvent();


        grabbedObject = null;
    }

    public void ConsumeGrabbedObject()
    {
        //Does the object can be possessed, and if yes, it is possessed ?
        PossessInteraction possessable = grabbedObject.GetComponent<PossessInteraction>();

        if (possessable && possessable.IsPossessed)
        {
            //Force the current ghost out of the possessed object
            possessable.ForceUnpossess();
        }

        grabbedObject = null;
    }

    private bool UpdateDropPosition()
    {
        finalDropPosition = dropPosition.position;

        float startHeight = 10f;

        float offsetDistance = 0.2f;

        float minDistance = startHeight - offsetDistance - 1.55f;

        float maxDistance = startHeight + offsetDistance + 10f;

        Vector3 startPos = dropPosition.position + Vector3.up * startHeight;
        RaycastHit hit;

        float groundHeight = GetAdjustedPositionFromGround(transform.root.position).y;


        finalDropPosition = new Vector3(transform.root.position.x, groundHeight, transform.root.position.z);
        Vector3 raycastDirection = (new Vector3(dropPosition.position.x, groundHeight, dropPosition.position.z) - finalDropPosition).normalized;
        // Check for wall at the drop position
        if (Physics.CheckCapsule(dropPosition.position - raycastDirection * checkWallLength, dropPosition.position + raycastDirection * checkWallLength, checkWallRadius, layerMasks.groundAndWallLayer))
        {
            Debug.Log("wall is blocking");
            finalDropPosition = new Vector3(transform.root.position.x, groundHeight, transform.root.position.z);

            // Get the direction vector of the player
            raycastDirection = (new Vector3(dropPosition.position.x, groundHeight, dropPosition.position.z) - finalDropPosition).normalized;

            // Put the drop position at current human location instead
            //finalDropPosition = GetAdjustedPositionAwayFromWall(raycastDirection, layerMasks.groundAndWallLayer);
            finalDropPosition = GetAdjustedPositionAwayFromWall(raycastDirection, layerMasks.groundFurnitureAndWallLayer);
            return true;
        }

        Debug.DrawLine(startPos, startPos + Vector3.down * maxDistance, Color.cyan, 1.5f);
        // Raycast from the top to the bottom of the map to check for furnitures
        //RaycastHit[] hits = Physics.SphereCastAll(startPos, checkFurnitureRadius, Vector3.down, maxDistance, layerMasks.groundAndFurnitureLayer);

        //if (hits.Length > 0)
        RaycastHit furnitureHit;
        if (Physics.SphereCast(startPos, checkFurnitureRadius, Vector3.down, out furnitureHit, maxDistance, layerMasks.groundAndFurnitureLayer))
        {
            //foreach (RaycastHit furnitureHit in hits)
            //{
                //if (furnitureHit.collider.gameObject.GetComponent<GrabInteraction>() != grabbedObject)
                //{
                    Debug.Log("furniture is blocking");
                    //Debug.DrawLine(finalDropPosition, finalDropPosition + Vector3.forward * offsetDistance, Color.green, 1f);
                    Debug.DrawLine(furnitureHit.point, furnitureHit.point + Vector3.forward * offsetDistance, Color.white, 1.5f);
                    Debug.Log("hit distance : " + furnitureHit.distance);
                    if (furnitureHit.distance > minDistance)
                    {
                        Debug.Log("furniture is small");
                        
                        // If the furniture is small enough, we can drop the object on it
                        finalDropPosition = furnitureHit.point + Vector3.up * offsetDistance;
                        return false;
                    }
                    else
                    {
                        Debug.Log("furniture is high");
                        finalDropPosition = new Vector3(transform.root.position.x, groundHeight, transform.root.position.z);

                        raycastDirection = (new Vector3(dropPosition.position.x, groundHeight, dropPosition.position.z) - finalDropPosition).normalized;

                        // If the furniture is too high, we will drop the object at the human location
                        finalDropPosition = GetAdjustedPositionAwayFromWall(raycastDirection, layerMasks.furnitureLayer);
                        return true;
                    }
                //}
            //    Debug.Log("CEST LE MEME OBJET CRETIN");
            //}
            //return false;
            
        } else
        {
            return false;
        }
    }

    private void CheckAndMoveHumanBackward()
    {
        float minDistance = 1.6f;
        
        float maxDistance = 4f;

        int nbMaxRays = 16;

        float degreeOffset = 360f / nbMaxRays;

        Vector3 startingDirection = (transform.root.position - dropPosition.position).normalized;

        float startingDegree = Vector3.SignedAngle(Vector3.left, startingDirection, Vector3.up);

        List<Vector3> directionList = new List<Vector3>();
        List<RaycastHit> raycastList = new List<RaycastHit>();

        // Make raycasts around the human starting from behind him and progressing to the farthest of its back
        for (int i = 0; i < nbMaxRays; i++)
        {
            int index = i;
            if (index % 2 == 1)
            {
                index += 1;
                index = index * -1;
            }
            index = index / 2;

            float curDegree = ((degreeOffset * index) + startingDegree) % 360;

            float incrRad = curDegree * (Mathf.PI / 180);

            Vector3 direction = new Vector3(-Mathf.Cos(incrRad), 0, Mathf.Sin(incrRad)).normalized;

            RaycastHit hit;
            //Debug.DrawLine(transform.root.position, transform.root.position + direction * (maxDistance * (i / (float)nbMaxRays)), Color.blue, 2f);
            Physics.Raycast(transform.root.position, direction, out hit, maxDistance, layerMasks.furnitureAndWallLayer);

            directionList.Insert(i, direction);
            raycastList.Insert(i, hit);
        }

        // For all raycasts made before
        for (int i = 0; i < nbMaxRays; i++)
        {
            RaycastHit hit = raycastList[i];
            Vector3 direction = directionList[i];

            Vector3 endPosition = transform.root.position + direction * movingBackDistance;

            // If there wasn't any hit, it's a good direction for moving back
            if (hit.Equals(default(RaycastHit)))
            {
                MoveHuman(endPosition);
                return;
            }

            // If there was a hit but far enough, it's a good direction as well
            if (hit.distance > minDistance)
            {
                MoveHuman(endPosition);
                return;
            }

            // If it get here, this isn't a good position to move back to, so check the next raycast
            Debug.DrawLine(transform.root.position, endPosition, Color.red, 2f);
        }
    }

    private void MoveHuman(Vector3 endPosition)
    {
        //Debug.DrawLine(transform.root.position, endPosition, Color.cyan, 2f);
        transform.root.DOMove(endPosition, 0.1f);
    }

    private Vector3 GetAdjustedPositionAwayFromWall(Vector3 raycastDirection, int layerMask)
    {
        float maxDistanceFromWall = 1.8f;
        float maxObjectDistanceFromWall = maxDistanceFromWall - 1f;

        RaycastHit hit;

        Debug.DrawLine(finalDropPosition, finalDropPosition + raycastDirection * maxDistanceFromWall, Color.red, 2f);
        if (Physics.Raycast(finalDropPosition, raycastDirection, out hit, maxDistanceFromWall, layerMask))
        {
            float finalDropDistance = Vector3.Distance(finalDropPosition, hit.point);

            movingBackDistance = maxDistanceFromWall - finalDropDistance;

            float movingBackObjectDistance = maxObjectDistanceFromWall - finalDropDistance;

            Vector3 direction = (finalDropPosition - hit.point).normalized;
            Debug.DrawLine(finalDropPosition, finalDropPosition + direction * movingBackObjectDistance, Color.yellow, 2f);
            return finalDropPosition + direction * movingBackObjectDistance;
        } else
        {
            // Should not get here
            return dropPosition.position;
        }   
    }

    private Vector3 GetAdjustedPositionFromGround(Vector3 pos)
    {
        float startHeight = 10f;

        float offsetDistance = 0.2f;

        float minDistance = startHeight - offsetDistance;

        float maxDistance = startHeight + offsetDistance + 10f;

        Vector3 startPos = pos + Vector3.up * startHeight;
        RaycastHit hit;

        // Check the height of the ground at the drop position
        Debug.DrawLine(startPos, startPos + Vector3.down * maxDistance, Color.magenta, 1f);
        Physics.Raycast(startPos, Vector3.down, out hit, maxDistance, layerMasks.groundLayer);
        return new Vector3(pos.x, hit.point.y + offsetDistance, pos.z);
    }
}
