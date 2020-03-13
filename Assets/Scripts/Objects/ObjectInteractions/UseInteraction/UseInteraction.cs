using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[System.Serializable]
public class UnityEventHuman : UnityEvent<Human>
{

}

public class UseInteraction : IHumanInteraction
{
    public UnityEventHuman OnHumanAction;

    public void HumanAction(Human character)
    {
        OnHumanAction.Invoke(character);
    }
}

