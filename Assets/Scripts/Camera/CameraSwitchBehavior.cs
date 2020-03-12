using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

public class CameraSwitchBehavior : MonoBehaviour
{

    [SerializeField]
    private ScopedInt currentCharacterType;

    [SerializeField]
    private CinemachineVirtualCamera cameraToActivate;

    private RoomBehavior roomBehavior;

    private void Start()
    {
        roomBehavior = GetComponentInParent<RoomBehavior>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (CheckCorrectObject(other))
        {
            if(roomBehavior != null)
            {
                roomBehavior.ActivateVirtualCamera(cameraToActivate);
            }
        }
    }

    private bool CheckCorrectObject(Collider other)
    {
        CharacterType characterType = (CharacterType)currentCharacterType.value;
        switch (characterType)
        {
            case CharacterType.Ghost:
                PossessInteraction possessInteraction = other.GetComponentInParent<PossessInteraction>();
                return other.transform.root.CompareTag("Ghost") 
                    || (possessInteraction != null && possessInteraction.IsPossessed && possessInteraction.PossessedBy.transform.root.CompareTag("Ghost"));
            case CharacterType.Human:
                return other.transform.root.CompareTag("Human");
        }

        return false;
    }
}
