using UnityEngine;
using System;
using UnityEngine.Events;
using System.Collections.Generic;

/// <summary>
/// Component to trigger events when colliding with certains marked triggerers (TriggererEvents) 
/// </summary>
public class TriggerEvents : MonoBehaviour
{
    //Si la liste vide, active la collision avec n'importe quel autre collider, sinon, uniquement ceux portant le même tag.
    [SerializeField]
    private List<string> allowedTags = new List<string>();
  
    public Action OnEnterTrigger = null;
    public Action OnExitTrigger = null;
    
    public MyUnityEventCollider OnEnterTriggerEvent = null;
    
    public MyUnityEventCollider OnExitTriggerEvent = null;

    private void Start()
    {
        if (GetComponent<Collider>() == null)
        {
            Debug.Log("No collider 2D detected with Trigger! Add one ! (" + gameObject.name + ")");
        }
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (IsTriggerValid(collision))
        {
            OnEnterTriggerEvent?.Invoke(collision);
            OnEnterTrigger?.Invoke();
        }
    }


    private void OnTriggerExit(Collider collision)
    {
        if (IsTriggerValid(collision))
        {
            OnExitTrigger?.Invoke();
            OnExitTriggerEvent?.Invoke(collision);
        }
    }

        
    private bool IsTriggerValid(Collider coll)
    {
        //check errors
        if (coll == null || coll.gameObject == null) {
            return false;
        }

        //Si aucun tag, autorise tout.
        if (allowedTags.Count == 0)
        {
            return true;
        }

        foreach (string tag in allowedTags)
        {
            if (tag.Equals(coll.gameObject.tag))
            {
                return true;
            }
        }
        return false;

    }
}

[System.Serializable]
public class MyUnityEventCollider : UnityEvent<Collider>
{
        
}

