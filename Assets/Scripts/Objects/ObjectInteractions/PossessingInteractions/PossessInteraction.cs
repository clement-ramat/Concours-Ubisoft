using Mirror;
using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;


[System.Serializable]
public class UnityEventFloat2 : UnityEvent<float, float>
{

}

/// <summary>
/// Component to call for possessing objects actions
/// </summary>
public abstract class PossessInteraction : IGhostInteraction
{

    /// <summary>
    /// Tell the ghost how to unpossess this object, if true try first behind him and if false from forward.
    /// </summary>
    public bool ghostUnpossessFromBehindFirst = true;

    [SerializeField][ReadOnly]
    private Possesser possessedBy = null;

    [Title("Possess Interaction Events")]
    [SerializeField]
    private UnityEvent OnPossessEvent = new UnityEvent();

    [SerializeField]
    private UnityEvent OnUnpossessEvent = new UnityEvent();

    [Title("Possess Interaction Vibration")]
    public float possessVibrationPower = 0.2f;

    public float possessVibrationDuration = 0.2f;

    public UnityEventFloat2 OnMakeVibration;

    private bool isPossessed = false;
    

    public bool IsPossessed
    {
        get
        {
            return isPossessed;
        }
    }
    

    public Possesser PossessedBy { get => possessedBy; set => possessedBy = value; }

    public bool ForceUnpossess()
    {
        if (!isPossessed)
        {
            return false;
        }
        
        possessedBy.Unpossess();

        return true;

    }

    public virtual void OnUnpossess()
    {
        isPossessed = false;
        OnUnpossessEvent?.Invoke();
        OnMakeVibration?.Invoke(possessVibrationPower, possessVibrationDuration);

        RpcOnUnpossess();
    }

    public virtual void OnPossess()
    {
        isPossessed = true;
        OnPossessEvent?.Invoke();
        OnMakeVibration?.Invoke(possessVibrationPower, possessVibrationDuration);

        RpcOnPossess();

    }
    

    [ClientRpc]
    public void RpcOnPossess()
    {
        isPossessed = true;
        OnPossessEvent?.Invoke();
        
    }

    [ClientRpc]
    public void RpcOnUnpossess()
    {
        isPossessed = false;
        OnUnpossessEvent?.Invoke();
    }

    public abstract void JoystickInteraction(Vector3 inputAxis);

}
