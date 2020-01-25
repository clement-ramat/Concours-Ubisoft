using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ghost : Character
{

    [SerializeField]
    [Required]
    private Possesser possesser;

    private void Update()
    {
        if (Input.GetKeyDown("space"))
        {
            if (possesser.IsPossessing())
            {
                possesser.Unpossess();
            }
            else if (!possesser.IsPossessing() && interactionArea.HasInteractable())
            {
                interactionArea.InteractableObject.GetComponent<PossessInteraction>().GhostAction(this);
            }
        }

        Vector3 moveInput;

        if (possesser.IsPossessing())
        {
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), 0).normalized;
            possesser.PossessInteraction.JoystickInteraction(moveInput);
        }
        else
        {
            moveInput = new Vector3(Input.GetAxisRaw("Horizontal"), 0, Input.GetAxisRaw("Vertical")).normalized;
            characterMovement.SetMoveInput(moveInput);
        }
    }

    public void Possess(PossessInteraction toPossess)
    {
        possesser.Possess(toPossess.gameObject);
    }

    public void SetGhostTangibility(bool isTangible)
    {
        characterMovement.CanMove = isTangible;

        visualsParent.gameObject.SetActive(isTangible);

        rb.isKinematic = !isTangible;
        rb.useGravity = isTangible;
    }

    public void SetVisualActive(bool active)
    {
        visualsParent.gameObject.SetActive(active);
    }
}
