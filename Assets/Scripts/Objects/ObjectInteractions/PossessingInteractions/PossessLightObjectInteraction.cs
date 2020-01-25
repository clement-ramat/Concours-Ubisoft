using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class PossessLightObjectInteraction : PossessInteraction
{

    private CharacterMovement objectMovement;

    private void Awake()
    {
        objectMovement = GetComponent<CharacterMovement>();
    }

    public override void OnUnpossess()
    {
        objectMovement.SetMoveInput(Vector3.zero);
    }

    public override void GhostAction(Ghost character)
    {
        character.Possess(this);
    }

    public override void JoystickInteraction(Vector2 inputAxis)
    {
        objectMovement.SetMoveInput(new Vector3(inputAxis.x, 0, inputAxis.y));
    }

}
