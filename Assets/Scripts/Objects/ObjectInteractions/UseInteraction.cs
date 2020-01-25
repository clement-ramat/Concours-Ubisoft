using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class UseInteraction : MonoBehaviour, IObjectInteraction
{
    public abstract void HumanAction(Human character);

    public abstract void GhostAction(Ghost character);
}
