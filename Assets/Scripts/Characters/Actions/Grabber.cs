using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Grabber : MonoBehaviour
{

    [Title("Grab")]
    [SerializeField][ReadOnly]
    protected GameObject grabbedObject;

    public GameObject GrabbedObject
    {
        get
        {
            return grabbedObject;
        }
    }

    public abstract void Grab(GameObject go);

    public abstract bool Ungrab();

    public bool IsGrabbingObject()
    {
        return grabbedObject != null;
    }
}
