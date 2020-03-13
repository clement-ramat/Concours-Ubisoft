using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Joystick Interactions/New UpDownInteraction")]
public class UpDownJoystickInteraction : IJoystickInteraction
{
    [Range(0f, 1.0f)]
    public float offsetForValid;

    bool isValidThisFrame = false;
    Vector3 directionNotWanted = Vector3.zero;

    public override bool IsJoystickInteractionValid()
    {
        return isValidThisFrame;
    }

    public override Vector3 JoystickInteraction(Vector3 inputAxis)
    {

        isValidThisFrame = false;

        // The joystick is ok to the top ( +Y )
        if (inputAxis.y > offsetForValid)
        {
            if (directionNotWanted !=  Vector3.up)
            {
                isValidThisFrame = true;

                directionNotWanted = Vector3.up;

                return Vector3.up;
            }
        }

        // The joystick is ok to the bottom ( -Y )
        else if (inputAxis.y < -offsetForValid)
        {
            if (directionNotWanted != Vector3.down)
            {
                isValidThisFrame = true;

                directionNotWanted = Vector3.down;

                return Vector3.down;
            }
        }
        else
        {
            directionNotWanted = Vector3.zero;
        }

        return Vector3.zero;
    }
}
