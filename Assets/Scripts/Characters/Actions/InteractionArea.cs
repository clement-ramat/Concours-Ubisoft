using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;

/// <summary>
/// Garde en mémoire le dernier objet interactable qui est entré dans le collider pour que les personnages peuvent l'utiliser
/// </summary>
/// 

[RequireComponent(typeof(Collider))]
public class InteractionArea : MonoBehaviour
{

    /// <summary>
    /// Objet mis en cache avec lequel le personnage peut interagir.
    /// </summary>
    [SerializeField][ReadOnly]
    private GameObject interactableObject = null;

    /// <summary>
    /// Events se déclenchant sur l'arrive/sortie d'un objet interactable
    /// </summary>
    // TODO : A modifier pour adapter à SODA ?
    [SerializeField]
    private Action<GameObject> OnNewInteractable = null;
    [SerializeField]
    private Action<GameObject> OnLossInteractable = null;
    


    public GameObject InteractableObject
    {
        get => interactableObject;

        private set
        {

            if (interactableObject != null)
            {
                OnLossInteractable?.Invoke(interactableObject);
            }

            if (value != null)
            {
                OnNewInteractable?.Invoke(value);
            }
            interactableObject = value;

        }
    }

    private void Awake()
    {

        //OnNewInteractable += PrintNewInteractable;
        //OnLossInteractable += PrintLostInteractable;
    }

    public bool HasInteractable()
    {
        return interactableObject != null;
    }

    private void OnTriggerEnter(Collider other)
    {
        
        //Si l'objet a un interactable, on le sauvegarde en cache.

        IObjectInteraction interactable = other.GetComponent<IObjectInteraction>();

        if (interactable != null)
        {
            InteractableObject = other.gameObject;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Si l'objet qui sort est l'interactable en cache, on le vire

        if (other.gameObject.Equals(interactableObject))
        {
            InteractableObject = null;
        }

    }

    private void PrintNewInteractable(GameObject go)
    {
        Debug.Log("New Interactable : " + go.name);
    }

    private void PrintLostInteractable(GameObject go)
    {
        Debug.Log("Lost Interactable : " + go.name);
    }
}
