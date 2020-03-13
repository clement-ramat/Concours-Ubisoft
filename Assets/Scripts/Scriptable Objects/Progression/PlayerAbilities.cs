using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ThirteenPixels.Soda;

[CreateAssetMenu(fileName = "NewPlayerAbilities", menuName = "ScriptableObjects/PlayerAbilities")]
public class PlayerAbilities : ScriptableObject
{
    public GlobalBool BasicCamera;
    public GlobalBool PastCamera;

    public void Reset()
    {
        BasicCamera.value = false;
        PastCamera.value = false;
    }
}
