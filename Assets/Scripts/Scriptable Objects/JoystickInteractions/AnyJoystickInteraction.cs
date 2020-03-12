using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Joystick Interactions/AnyJoystickInteraction")]
public class AnyJoystickInteraction : IJoystickInteraction
{


    [Range(0f, 3.0f)]
    public float offsetForValid;

    bool hasReset = true;

    Vector3 lastDirection = Vector3.zero;

    public override bool IsJoystickInteractionValid()
    {
        return hasReset;
    }

    public override Vector3 JoystickInteraction(Vector3 inputAxis)
    {
        //Debug.Log("Distance : " + Vector3.Distance(lastDirection, inputAxis));
        hasReset = (Vector3.Distance(lastDirection, inputAxis) > offsetForValid);
        if (hasReset)
        {
            lastDirection = inputAxis;
        }

        return inputAxis.normalized;
    }
}