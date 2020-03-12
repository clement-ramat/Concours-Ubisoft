using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Joystick Interactions/New LeftRightInteraction")]
public class LeftRightJoystickInteraction : IJoystickInteraction
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

        // The joystick is ok to the right ( +X )
        if (inputAxis.x > offsetForValid)
        {
            if(directionNotWanted != Vector3.right)
            {
                isValidThisFrame = true;

                directionNotWanted = Vector3.right;

                return Vector3.right;
            }
        }

        // The joystick is ok to the left ( -X )
        else if (inputAxis.x < -offsetForValid)
        {
            if (directionNotWanted != Vector3.left)
            {
                isValidThisFrame = true;

                directionNotWanted = Vector3.left;

                return Vector3.left;
            }
        }

        else
        {
            directionNotWanted = Vector3.zero;
        }
        

        return Vector3.zero;
    }
}
