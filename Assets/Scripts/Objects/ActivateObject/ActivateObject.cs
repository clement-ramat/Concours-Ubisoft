using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class ActivateObjectEvent : UnityEvent<ActivateObject> { }

/// <summary>
/// Represents the interface for objects we can activate.
/// Used various classes to trigger events (Openable...), can also directly used the UnityEvent.
/// </summary>
public abstract class ActivateObject : NetworkBehaviour
{

    public ActivateObjectEvent OnActivateEvent = new ActivateObjectEvent();
    public ActivateObjectEvent OnDeactivateEvent = new ActivateObjectEvent();

    [SyncVar]
    private bool activated = false;
    public bool Activated
    {
        protected set
        {
            activated = value;

            if (activated && OnActivateEvent != null)  
            {
                OnActivateEvent.Invoke(this);
            }else if (!activated && OnDeactivateEvent != null)
            {
                OnDeactivateEvent.Invoke(this);
            }
        }

        get
        {
            return activated;
        }
    }

    [SyncVar]
    private bool canBeActivate = true;
    public bool CanBeActivate
    {
         get
        {
            return canBeActivate;
        }

        set
        {
            Debug.Log(value);

            canBeActivate = value;

            if (!canBeActivate)
            {
                Activated = false;
            }
        }
    }

    public abstract void Activate();
}
