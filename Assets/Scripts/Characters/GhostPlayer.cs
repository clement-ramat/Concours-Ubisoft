using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;

public class GhostPlayer : Ghost
{

    [Title("Ghost Player Settings")]
    [Title("", "Reveal Aera")]
    [SerializeField]
    [Required]
    [ChildGameObjectsOnly]
    private RevealArea revealArea;

    [SerializeField]
    private PlayerAbilities playerAbilities;

    [SerializeField]
    private float revealCooldown = 1f;

    private float timeLeftRevealCooldown = 0.0f;

    [SyncVar]
    private bool canReveal = false;

    public override void OnStartServer()
    {
        base.OnStartServer();

        playerAbilities.BasicCamera.onChange.AddResponse(UpdateReveal);
        UpdateReveal(playerAbilities.BasicCamera.value);

        playerAbilities.PastCamera.onChange.AddResponse(UpdateBlast);
        UpdateBlast(playerAbilities.PastCamera.value);

        ResetVelocity();
        
    }

    private void UpdateReveal(bool newValue)
    {
        canReveal = newValue;
    }

    private void UpdateBlast(bool newValue)
    {
        //TO DO si on active le blast uniquement avec l'obtention de l'AP++
    }

    protected override void Update()
    {
        base.Update();

        if (!isServer)
        {
            return;
        }

        if (possesser.IsPossessing())
        {
            possesser.PossessInteraction.JoystickInteraction(MoveInput);
        }
        else
        {
            characterMovement.MoveInput = MoveInput;
        }

        timeLeftRevealCooldown -= Time.deltaTime;
    }


    public bool PossessionAction()
    {
        if (possesser.IsPossessing())
        {
            UnPossess();
            return true;
        }
        else if (interactionArea.HasInteractable())
        {
            PossessInteraction possessInteraction = interactionArea.InteractableObject.GetComponent<PossessInteraction>();

            if (possessInteraction != null)
            {
                return Possess(possessInteraction);
            }
            else
            {
                // failed to possess
                return false;
            }
        }
        else
        {
            // failed to possess
            return false;
        }

    }

    public void RevealBlast()
    {
        if (!IsPossessing() && timeLeftRevealCooldown <= 0)
        {
            if (canReveal)
            {
                RpcRevealBlast();
                timeLeftRevealCooldown = revealCooldown;
            }
        }
    }

    [ClientRpc]
    public void RpcRevealBlast()
    {
        revealArea.RevealBlast(isServer);
    }
}
