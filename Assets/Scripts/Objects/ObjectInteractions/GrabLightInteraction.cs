using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabLightInteraction : GrabInteraction
{
    public override void HumanAction(Human character)
    {

        character.GrabLightObject(gameObject);

    }
}
