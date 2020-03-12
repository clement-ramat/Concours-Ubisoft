using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SceneLoaderBehavior : MonoBehaviour
{
    [SerializeField]
    private bool hideInstantOnStart = false;
    public bool HideInstantOnStart
    {
        get
        {
            return hideInstantOnStart;
        }
    }

    [SerializeField]
    private Animator sceneLoaderAnimator;

    public bool Hidden {
        set
        {
            sceneLoaderAnimator.SetBool("Hide", value);
        }
    }

    private void Start()
    {

        if (hideInstantOnStart)
        {
            sceneLoaderAnimator.SetTrigger("HideInstant");
            Hidden = true;
        }
    }

}
