using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class IJoystickInteraction : ScriptableObject
{

    public abstract Vector3 JoystickInteraction(Vector3 inputAxis);
    public abstract bool IsJoystickInteractionValid();
}
