using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterAnimations : NetworkBehaviour
{

    [Title("On Ability Given Settings")]
    [SerializeField]
    private PlayerAbilities playerAbilities;

    [SerializeField]
    private string happyAnimationTrigger;

    private NetworkAnimator myNetworkAnimator;

    private void Start()
    {
        myNetworkAnimator = GetComponent<NetworkAnimator>();

        playerAbilities.BasicCamera.onChange.AddResponse(AnimateOnAbilityGiven);
        playerAbilities.PastCamera.onChange.AddResponse(AnimateOnAbilityGiven);
    }

    [Server]
    private void AnimateOnAbilityGiven(bool value)
    {
        if (value && myNetworkAnimator != null)
        {
            myNetworkAnimator.SetTrigger(happyAnimationTrigger);
        }
    }
}
