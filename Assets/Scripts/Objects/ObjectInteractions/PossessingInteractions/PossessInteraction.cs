using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Component to call for possessing objects actions
/// </summary>
public abstract class PossessInteraction : MonoBehaviour, IObjectInteraction
{

    public virtual void OnPossess() { }
    public virtual void OnUnpossess() { }

    public abstract void GhostAction(Ghost character);

    public abstract void JoystickInteraction(Vector2 inputAxis);

    public void HumanAction(Human character)
    {
        //None, players can't possess objects.
    }

}
