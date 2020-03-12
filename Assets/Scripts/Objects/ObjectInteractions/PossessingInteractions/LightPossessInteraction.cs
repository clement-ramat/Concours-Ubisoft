using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CharacterMovement))]
public class LightPossessInteraction : PossessInteraction
{
    
    private CharacterMovement characterMovement;
    private LevitatingMovement levitatingMovement;

    private void Start()
    {
        characterMovement = GetComponent<CharacterMovement>();
        levitatingMovement = GetComponent<LevitatingMovement>();

        characterMovement.enabled = false;
        if (levitatingMovement)
            levitatingMovement.enabled = false;
    }

    public override void JoystickInteraction(Vector3 inputAxis)
    {
        characterMovement.MoveInput = inputAxis;
    }

    public override void OnPossess()
    {
        base.OnPossess();

        characterMovement.enabled = true;
        if (levitatingMovement)
            levitatingMovement.enabled = true;
    }

    public override void OnUnpossess()
    {
        base.OnUnpossess();

        characterMovement.enabled = false;

        if (levitatingMovement)
            levitatingMovement.enabled = false;

        characterMovement.ResetVelocity();
    }
}
