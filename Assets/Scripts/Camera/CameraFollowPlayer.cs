using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirteenPixels.Soda;
using Cinemachine;

public class CameraFollowPlayer : MonoBehaviour
{

    [SerializeField]
    private ScopedInt characterTypeGlobal;
    [SerializeField]
    private ScopedGameObject ghostGameObject;
    [SerializeField]
    private ScopedGameObject humanGameObject;

    // Start is called before the first frame update
    void Start()
    {
        characterTypeGlobal.onChangeValue.AddResponse(FollowPlayer);

        ghostGameObject.onChangeValue.AddResponse(FollowPlayerGameobject);
        humanGameObject.onChangeValue.AddResponse(FollowPlayerGameobject);

        FollowPlayer(characterTypeGlobal.value);
    }

    private void OnDestroy()
    {
        ghostGameObject.onChangeValue.RemoveResponse(FollowPlayerGameobject);
        humanGameObject.onChangeValue.RemoveResponse(FollowPlayerGameobject);
    }

    private void FollowPlayer(int characterType = 0)
    {
        if (characterTypeGlobal.value == 1)//Human
        {
            if(humanGameObject.value != null)
            {
                GetComponent<CinemachineVirtualCamera>().Follow = humanGameObject.value.transform;
            }
            
        }
        else //ghost
        {
            if (ghostGameObject.value != null)
            {
                GetComponent<CinemachineVirtualCamera>().Follow = ghostGameObject.value.transform;
            }
        }
    }

    private void FollowPlayerGameobject(GameObject gameObject)
    {
        FollowPlayer();
    }
}
