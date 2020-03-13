using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.Events;
using System;
using System.Linq;
using ThirteenPixels.Soda;

/// <summary>
/// Garde en mémoire le dernier objet interactable qui est entré dans le collider pour que les personnages peuvent l'utiliser
/// </summary>
/// 

[RequireComponent(typeof(Collider))]
public class InteractionArea : MonoBehaviour
{
    /// <summary>
    /// Objets mis en cache avec lequel le personnage peut interagir.
    /// </summary>
    [SerializeField]
    [ReadOnly]
    protected List<GameObject> interactableObjects = new List<GameObject>();

    /// <summary>
    /// Events se déclenchant sur l'arrive/sortie d'un objet interactable
    /// </summary>
    [SerializeField]
    protected Action<GameObject> OnNewInteractable = null;
    [SerializeField]
    protected Action<GameObject> OnLossInteractable = null;

    [SerializeField]
    private bool isHuman = false;

    [SerializeField]
    private bool allowHighlighting = true;

    [Title("Events")]

    [Title("", "OnHumanInteractable")]
    [SerializeField]
    private UnityEvent onGrabbable;

    [SerializeField]
    private UnityEvent onUngrabbable;

    [SerializeField]
    private UnityEvent onUsable;


    [Title("", "OnGhostInteractable")]

    [SerializeField]
    private UnityEvent onPossessable;

    [SerializeField]
    private UnityEvent onUnpossessable;


    [Header("Blocking Obstacles")]
    [SerializeField]
    private LayerMask blockingLayersForInteraction;

    [SerializeField]
    private Transform startRaycastBlocking;

    private GameObject highlightedObject;
    private Dictionary<Material, Shader> savedShaders = new Dictionary<Material, Shader>();

    [SerializeField]
    private ScopedInt localCharacterType;

    private bool enableHighlight = true;
    public bool EnableHighlight
    {
        get
        {
            return enableHighlight;
        }
        set
        {
            enableHighlight = value;
        }
    }

    public GameObject InteractableObject
    {
        get
        {
            if (interactableObjects.Count <= 0)
            {
                return null;
            }
            else
            {
                //SortListWithPriority();

                return interactableObjects[0];
            }
        }

    }

    public UnityEvent OnGrabbable { get => onGrabbable; }

    public UnityEvent OnUngrabbable { get => onUngrabbable; }
    public UnityEvent OnUsable { get => onUsable; }
    public UnityEvent OnPossessable { get => onPossessable;  }
    public UnityEvent OnUnpossessable { get => onUnpossessable;  }

    public bool HasInteractable()
    {
        return interactableObjects.Count > 0;
    }

    private void OnTriggerEnter(Collider other)
    {
        //Si l'objet a un interactable, on le sauvegarde en cache.

        //Pass through all parents until one has Interactable tag
        // which means it will be the root of the interactable object
        //Transform objParent = other.transform;

        //LayerMask ObjectInteractableLayer = prevParent.gameObject.layer;

        //objParent = GetInteractableObjectRoot(objParent);

        IObjectInteraction interactable = GetInteractionFrom(other.transform);

        if (interactable != null && interactable.CanBeInteractWith)
        {
            //Debug.Log("on enter " + interactable.name);

            // If the interactable have line of sight to the player
            if (!IsInteractableBlocked(interactable.gameObject))
            {


                if (!interactableObjects.Contains(interactable.gameObject))
                {
                    interactableObjects.Add(interactable.gameObject);

                    SanitizeInteractableList();
                    SortListWithPriority();

                    if (
                        (isHuman && localCharacterType.value == (int)CharacterType.Human) ||
                        (!isHuman && localCharacterType.value == (int)CharacterType.Ghost)
                    )
                    {
                        NotifyEvents();
                        //character.RpcNotifyUIEvents();
                        if (allowHighlighting)
                        {
                            UpdateHighlight();
                        }
                    }
                }


            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Si l'objet qui sort est l'interactable en cache, on le vire
        //Pass through all parents until one has useinteraction
        //Transform objParent = other.transform;

        //LayerMask ObjectInteractableLayer = prevParent.gameObject.layer;

        //objParent = GetInteractableObjectRoot(objParent);

        IObjectInteraction interactable = GetInteractionFrom(other.transform);

        if (interactable)
        {
            if (interactableObjects.Contains(interactable.gameObject))
            {
                interactableObjects.Remove(interactable.gameObject);
                SanitizeInteractableList();
                SortListWithPriority();
                
                if (
                    (isHuman && localCharacterType.value == (int)CharacterType.Human) ||
                    (!isHuman && localCharacterType.value == (int)CharacterType.Ghost)
                )
                {
                    NotifyEvents();
                    //character.RpcNotifyUIEvents();
                    if (allowHighlighting)
                    {
                        UpdateHighlight();
                    }
                }
            }
        }
    }

    private IObjectInteraction GetInteractionFrom(Transform tf)
    {
        if (isHuman)
        {
            return GetInteractionFrom<IHumanInteraction>(tf);
        }
        else
        {
            return GetInteractionFrom<IGhostInteraction>(tf);
        }
    }

    private IObjectInteraction GetInteractionFrom<T>(Transform tf) where T : IObjectInteraction
    {
        IObjectInteraction interactable;

        //Check transform parents
        interactable = tf.GetComponentInParent<T>();

        if (!interactable)
        {
            //if doesn't work, check current
            interactable = tf.GetComponent<T>();
        }

        if (!interactable)
        {
            //if doesn't work, check childs
            interactable = tf.GetComponentInChildren<T>();
        }

        return interactable;
    }

    private Transform GetInteractableObjectRoot(Transform tf)
    {
        Transform objParent = tf;
        while (objParent != null && !objParent.CompareTag("Interactable"))
        {
            if (objParent.parent == null)
            {
                // If we didn't find any parent with the tag Interactable, return the starting object
                return tf;
            }
            else
            {
                objParent = objParent.parent;
            }
        }
        return objParent;
    }


    private bool IsInteractableBlocked(GameObject interactable)
    {

        float heightOffset = 0.5f;
        RaycastHit hit;

        Vector3 startPos = new Vector3(transform.root.position.x, interactable.transform.position.y + heightOffset, transform.root.position.z);
        Vector3 endPos = interactable.transform.position + new Vector3(0, heightOffset, 0);

        Vector3 direction = endPos - startPos;
        float distance = Vector3.Distance(endPos, startPos);

        //Debug.DrawLine(startPos, startPos + direction.normalized * distance, Color.green, 2f);
        if (Physics.Raycast(startPos, direction.normalized, out hit, distance, blockingLayersForInteraction))
        {
            if (GetInteractableObjectRoot(interactable.transform).gameObject == interactable)
            {
                return false;
            }
            return true;
        }
        return false;
    }

    private void SanitizeInteractableList()
    {
        for (int i = interactableObjects.Count - 1; i >= 0; i--)
        {
            GameObject go = interactableObjects[i];
            
            //Si l'objet est null ou désactivé, le retire de la liste.
            if (!go || !go.activeInHierarchy)
            {
                Debug.Log("Removed : " + go);
                interactableObjects.Remove(go);
            } 
        }
    }

    private void SortListWithPriority()
    {
        interactableObjects = interactableObjects.OrderByDescending(GetPriority).ToList();
    }

    private float GetPriority(GameObject interactableObject)
    {
        IObjectInteraction objectInteraction = GetInteractionFrom(interactableObject.transform);
        return 1f / (1f + Vector3.Distance(interactableObject.transform.position, transform.root.position)) + objectInteraction.interactionPriority * 5;
    }


    private void OnDrawGizmosSelected()
    {
        if (InteractableObject != null)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(startRaycastBlocking.position, InteractableObject.transform.position);
        }
    }

    //[ClientRpc]
    private void UpdateHighlight()
    {
        if (interactableObjects.Count <= 0)
        {
            if (highlightedObject != null)
            {
                SetHighlight(highlightedObject, false);
            }
            highlightedObject = null;
            return;
        }

        GameObject objectToHighlight = GetObjectToHighlight();

        if (objectToHighlight == null)
        {
            return;
        }

        if (highlightedObject != objectToHighlight)
        {

            if (highlightedObject != null)
            {
                SetHighlight(highlightedObject, false);
            }
            SetHighlight(objectToHighlight, true);
            highlightedObject = objectToHighlight;
        }
    }

    private GameObject GetObjectToHighlight()
    {
        foreach (GameObject interactableObject in interactableObjects)
        {
            IObjectInteraction interaction = GetInteractionFrom(interactableObject.transform);
            if (interaction.allowHighlighting)
            {
                return interactableObject;
            }
        }
        return null;
    }

    public void NotifyEvents()
    {
        if (interactableObjects.Count > 0)
        {
            //Evenement pour l'humains
            if (isHuman)
            {
                UseInteraction useInteraction = interactableObjects[0].GetComponent<UseInteraction>();
                if (useInteraction)
                {
                    onUsable?.Invoke();
                }

                GrabInteraction grabInteraction = interactableObjects[0].GetComponent<GrabInteraction>();
                if (grabInteraction)
                {
                    onGrabbable?.Invoke();
                }
            }
            else //Evenements pour fantome
            {
                PossessInteraction possessInteraction = interactableObjects[0].GetComponent<PossessInteraction>();
                if (possessInteraction)
                {
                    onPossessable?.Invoke();
                }
            }
        }
        else if (isHuman)
        {
            onUngrabbable?.Invoke();
        }
        else
        {
            onUnpossessable?.Invoke();
        }
    }

    private void SetHighlight(GameObject interactableObject, bool highlighted = true)
    {
        Renderer[] renderers = interactableObject.GetComponentsInChildren<Renderer>();

        foreach(Renderer renderer in renderers)
        {

            if (renderer.GetComponent<ParticleSystem>())
            {
                //On ignore les particules
                continue;
            }

            Material[] materials = renderer.materials;

            foreach (Material material in materials)
            {
                
                
                if (highlighted && EnableHighlight)
                {
                    //Debug.Log(EnableHighlight);
                    //Debug.Log("MAIS WHAT CHIEN DES BOIS");
                    savedShaders[material] = material.shader;
                    material.shader = Shader.Find("Shader Graphs/HighlightShader");
                    material.SetFloat("_Activate", 1);
                }
                else
                {
                    if (material.shader == Shader.Find("Shader Graphs/HighlightShader"))
                    {
                        material.shader = savedShaders[material];
                    }
                    
                    //material.SetFloat("_Activate", 0);
                }
            }
        } 
    }

}
