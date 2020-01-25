using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabberLightObject : Grabber
{
    

    [Title("Grab")]
    /// <summary>
    /// Nouveau parent de l'objet attrapé
    /// </summary>
    [SerializeField]
    [Required]
    private Transform grabParent;
    

    [Title("Un-Grab")]

    /// <summary>
    /// Parent de où l'objet a été pris
    /// </summary>
    [SerializeField][ReadOnly]
    private Transform initialParent;

    [SerializeField]
    [Required]
    private Transform dropPosition;
    


    public override void Grab(GameObject go)
    {
        if (IsGrabbingObject())
        {
            Debug.Log("Try to grab while already grabbing something ! Should not be possible?");
            return;
        }

        grabbedObject = go;

        //remember initial parent
        initialParent = grabbedObject.transform.parent;

        //grab and move object
        grabbedObject.transform.parent = grabParent;
        grabbedObject.transform.position = grabParent.position;

        //If object has rigibody, set as kinematic
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = true;
        }
    }

    /// <summary>
    /// Ungrab l'object actuelle, return true s'il y en avait un, false si aucn
    /// </summary>
    /// <returns></returns>
    public override bool Ungrab()
    {
        if (grabbedObject == null)
        {
            return false;
        }

        grabbedObject.transform.parent = initialParent;
        grabbedObject.transform.position = dropPosition.transform.position;

        //If object has rigibody, set as non-kinematic
        Rigidbody rb = grabbedObject.GetComponent<Rigidbody>();
        if (rb)
        {
            rb.isKinematic = false;
        }

        grabbedObject = null;

        return true;
    }
}
