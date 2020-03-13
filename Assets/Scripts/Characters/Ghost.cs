using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using Mirror;
using UnityEngine.AI;

[System.Serializable]
public class MyUnityEventGameObject : UnityEvent<GameObject>{
    
}

public abstract class Ghost : Character
{

    [Title("Ghost Settings")]
    [SerializeField]
    [Required]
    protected Possesser possesser;

    [SerializeField]
    private float launchPower = 20f;

    public GameObject nearestPossessable;

    public Possesser Possesser { get => possesser; }

    [Title("Events")]
    public MyUnityEventGameObject OnPossess;
    public MyUnityEventGameObject OnUnpossess;

    protected virtual void Update()
    {
        //If it's possessing, makes the ghost follow the possessed object.
        if (possesser.IsPossessing())
        {

            transform.position = possesser.PossessedObject.transform.position;
        }

    }


    /// <summary>
    /// Essaye de possèder l'objet donné
    /// </summary>
    /// <param name="ToPossess"></param>
    /// <returns></returns>
    public bool Possess(PossessInteraction ToPossess)
    {
        if (ToPossess.IsPossessed)
        {
            //Can't possess an already possessed object
            return false;
        }


        possesser.Possess(ToPossess.gameObject);
        interactionArea.EnableHighlight = false;
        Debug.Log("Disable highlight");

        return true;
    } 

    public bool UnPossess()
    {
        interactionArea.EnableHighlight = true;
        Debug.Log("Enable highlight");
        return possesser.Unpossess();

    }

    public bool ForceUnPossess()
    {
        return UnPossess();
    }

    public virtual bool LaunchPossessedObject()
    {
        if (possesser.IsPossessing())
        {
            Rigidbody objectRigidbody = possesser.PossessInteraction.GetComponent<Rigidbody>();
            if (objectRigidbody != null)
            {
                objectRigidbody.AddForceAtPosition(possesser.PossessInteraction.transform.forward * launchPower, new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z), ForceMode.Impulse);
            }

            return UnPossess();
        }

        return false;
    }

    public virtual bool LaunchPossessedObject(Vector3 direction)
    {
        if (possesser.IsPossessing())
        {
            Rigidbody objectRigidbody = possesser.PossessInteraction.GetComponent<Rigidbody>();
            if (objectRigidbody != null)
            {
                objectRigidbody.AddForceAtPosition(direction.normalized * launchPower, new Vector3(transform.position.x, transform.position.y - 0.1f, transform.position.z), ForceMode.Impulse);
            }

            return UnPossess();
        }

        return false;
    }



    public void ResetVelocity()
    {
        characterMovement.ResetVelocity();
    }

    public void SetGhostTangibility(bool isTangible)
    {
        characterMovement.CanMove = isTangible;

        rb.isKinematic = !isTangible;
        rb.useGravity = isTangible;

        colliderParent.gameObject.SetActive(isTangible);

        rb.detectCollisions = isTangible;

        RpcSetVisualActive(isTangible);
        ResetVelocity();
    }

    

    [ClientRpc]
    public void RpcSetVisualActive(bool active)
    {
        SetVisualActive(active);
    }

    private void SetVisualActive(bool active)
    {
        visualsParent.gameObject.SetActive(active);
    }

    public bool IsPossessing()
    {
        return possesser.IsPossessing();
    }


    /// <summary>
    /// Reset the ghost (unpossess object...) to a state where he can cross doors to a new room
    /// </summary>
    public override void ResetCharacter()
    {
        base.ResetCharacter();

        if (possesser.IsPossessing())
        {
            UnPossess();
        }
    }

    public GameObject GetPossessedObject()
    {
        if (possesser.IsPossessing())
            return possesser.PossessedObject;
        else
            return null;
    }

    public void OnDestroy()
    {
        UnPossess();
    }

    [ClientRpc]
    public void RpcPossessUIEvent(GameObject possessObject)
    {
        OnPossess?.Invoke(possessObject);
    }

    [ClientRpc]
    public void RpcUnpossessUIEvent(GameObject unPossessObject)
    {
        OnUnpossess?.Invoke(unPossessObject);
    }
}
