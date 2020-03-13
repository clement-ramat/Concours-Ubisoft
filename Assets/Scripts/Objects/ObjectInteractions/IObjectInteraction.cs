using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[SelectionBase]
public class IObjectInteraction : NetworkBehaviour
{
    public int interactionPriority = 0;

    public bool allowHighlighting = true;

    private bool canBeInteractWith = true;

    public bool CanBeInteractWith
    {
        get
        {
            return canBeInteractWith;
        }
        set
        {
            canBeInteractWith = value;
        }
    }

}

public class IGhostInteraction : IObjectInteraction
{

}

public class IHumanInteraction : IObjectInteraction
{

}
