using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using Sirenix.OdinInspector;
using Mirror;
using UnityEngine.Events;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Classe that represents objects we can open.
/// We can use an key (via same index) to open the object, or use multiple ActivateObjects.
/// </summary>
public class Openable : NetworkBehaviour
{

    [Title("Openable Settings")]

    [SerializeField]
    protected bool isKeyNeeded = true;

    // The index of the openable (can only be opened by keys of the same index)
    [SerializeField]
    [ShowIfGroup("isKeyNeeded")]
    private int openableIndex;

    [Title("", "Activate Objects")]

    // The list of activateObject we must activate to open the openable object.
    [SerializeField]
    protected List<ActivateObject> activateObjects = new List<ActivateObject>();

    [SerializeField]
    private float timeToOpenViaActivateObjects = 1f;

    [Title("Openable Events")]
    public UnityEvent OnOpen = new UnityEvent();
    public UnityEvent OnClose = new UnityEvent();
    public UnityEvent OnUnlock = new UnityEvent();
    public UnityEvent OnCantUnlock = new UnityEvent();

    public Transform transformToShake;

    private bool isShaking = false;

    [SyncVar]
    private bool isLocked = true;
    public bool IsLocked
    {
        get
        {
            return isLocked;
        }

        protected set
        {
            isLocked = value;
            SetActivateObjectsCanActivate(!isLocked);
        }
    }

    [SyncVar(hook = nameof(OnClientOpenChanged))]
    protected bool isOpen = false;
    protected bool alreadyUsed = false;

    public int Index
    {
        get
        {
            return openableIndex;
        }
    }

    public override void OnStartServer()
    {
        base.OnStartServer();

        // Set the openable to unlocked if no keys are needed
        IsLocked = isKeyNeeded;

        for (int i = 0; i < activateObjects.Count; i++)
        {
            if (activateObjects[i])
            {
                activateObjects[i].OnActivateEvent.AddListener(OnActivateObjectActive);
                activateObjects[i].OnDeactivateEvent.AddListener(OnActivateObjectDisable);
            }
        }
    }

    protected virtual void OnActivateObjectActive(ActivateObject activateObject)
    {
        Debug.Log("OnActivateObjectActive");

        // We try to know if every ActivateObjects are active, if so we can simple open.
        bool canBeOpen = true;
        activateObjects.ForEach(_object =>
        {

            if (!_object.Activated)
            {
                canBeOpen = false;
            }

        });

        if (canBeOpen)
        {
            StopAllCoroutines();
            StartCoroutine(OpenViaActivateObjectsCoroutine());
        }
    }

    private IEnumerator OpenViaActivateObjectsCoroutine()
    {
        yield return new WaitForSeconds(timeToOpenViaActivateObjects);
        Open();
    }

    protected virtual void OnActivateObjectDisable(ActivateObject activateObject)
    {
        StopAllCoroutines();
    }

    protected virtual void Update()
    {

    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {

        GUIStyle gUIStyle = new GUIStyle();
        gUIStyle.normal.textColor = IsLocked ? Color.red : Color.green;

        string lockedStr = IsLocked ? "LOCKED" : "UNLOCKED";
        Handles.Label(transform.position, lockedStr, gUIStyle);
    }
#endif

    public void InteractWithOpenable(Human human)
    {
        if (isOpen)
        {
            TryClosing(human);
        }
        else
        {
            TryOpening(human);
        }
    }

    public bool GrabbedObjectCanOpen(Human human)
    {

        KeyBehavior keyBehavior = human.GrabbedObject.GetComponent<KeyBehavior>();

        // Check if the grabbed object is a key
        if (keyBehavior)
        {
            // Check if the key has the same index as the openable
            if (keyBehavior.Index == this.Index)
            {
                return true;
            }
            else
            {
                // Wrong key !
                return false;
            }
        }
        else
        {
            // Grabbed object is not a key
            return false;
        }
    }

    private void TryOpening(Human human)
    {
        Debug.Log("TryOpening");
        // Prevent from opening twice when we don't want to
        if (alreadyUsed)
        {
            return;
        }

        // if openable is unlocked and we do not need to activate objects, just open it
        if (!IsLocked && activateObjects.Count == 0)
        {
            Open();
            return;
        }

        if (human.GrabbedObject == null)
        {
            // human has no object (so he has no key)
            OnCantUnlock?.Invoke();
            return;
        }

        KeyBehavior keyBehavior = human.GrabbedObject.GetComponent<KeyBehavior>();

        // Check if the grabbed object is a key
        if (keyBehavior != null)
        {
            Debug.Log("TryOpening Key not null");
            // Check if the key has the same index as the openable
            if (keyBehavior.Index == this.Index)
            {
                Debug.Log("TryOpening Correct Key = Unlock");
                human.ConsumeGrabbedObject();
                keyBehavior.ConsumeKey();

                // We have used the key to open the object
                IsLocked = false;
                //OnUnlock?.Invoke();
                RpcUnlock();

                // We open only if we do not need activate objects
                if (activateObjects.Count == 0)
                {
                    Open();
                }
            }
            else
            {
                // Wrong key !
                OnCantUnlock?.Invoke();
            }
        }
        else
        {
            // Grabbed object is not a key
            OnCantUnlock?.Invoke();
        }
    }

    [ClientRpc]
    private void RpcUnlock()
    {
        OnUnlock?.Invoke();
    }

    private void TryClosing(Human human)
    {
        Close();
    }

    public void SetActivateObjectsCanActivate(bool value)
    {
        activateObjects.ForEach(activateObject =>
        {
            if (activateObject)
            {
                activateObject.CanBeActivate = value;
            }
        });
    }

    [Button]
    public virtual void Open()
    {
        isOpen = true;

        if (OnOpen != null)
        {
            OnOpen.Invoke();
        }
    }

    [Button]
    public virtual void Close()
    {
        isOpen = false;

        if (OnClose != null)
        {
            OnClose.Invoke();
        }
    }

    /// <summary>
    /// Hook function called on client when isOpen changed on Server.
    /// Simply call the Open or Close function.
    /// </summary>
    private void OnClientOpenChanged(bool oldValue, bool newValue)
    {
        if (!isClientOnly)
        {
            return;
        }

        if (isOpen)
        {
            Open();
        }
        else
        {
            Close();
        }
    }

    
    public virtual void ShakeCantUnlock()
    {
        if (transformToShake != null)
        {
            if (!isShaking)
            {
                RpcShaketransform();
            }

        }
    }

    [ClientRpc]
    public void RpcShaketransform()
    {
        float shakePower = 0.1f;
        isShaking = true;
        transformToShake.DOShakePosition(0.5f, new Vector3(shakePower, shakePower, 0), 10, 90f, false, true).OnComplete(() =>
        {
            isShaking = false;
        });
    }
}
