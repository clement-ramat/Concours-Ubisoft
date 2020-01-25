using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class GrabInteraction : MonoBehaviour, IObjectInteraction
{
    public void GhostAction(Ghost character) {
        //Ghost cannot grab

    }

    public abstract void HumanAction(Human character);

}
