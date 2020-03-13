using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

public class CameraCullingMaskCharacterType : MonoBehaviour
{

    [SerializeField]
    private ScopedInt characterType;

    [SerializeField]
    private LayerMask ghostCullingMask;

    [SerializeField]
    private LayerMask humanCullingMask;

    private Camera mainCamera;

    private void Start()
    {
        mainCamera = GetComponent<Camera>();

        UpdateCameraCullingMask(characterType.value);
        characterType.onChangeValue.AddResponse(UpdateCameraCullingMask);
    }

    private void UpdateCameraCullingMask(int character)
    {
        CharacterType characterType = (CharacterType)character;
        switch (characterType)
        {
            case CharacterType.Human:
                mainCamera.cullingMask = humanCullingMask;
                break;

            case CharacterType.Ghost:
                mainCamera.cullingMask = ghostCullingMask;
                break;
        }
    }
}
