using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(menuName = "GlobalVariable/GlobalDialogueManager")]
public class GlobalDialogueManager : GlobalGameObjectWithComponentCacheBase<DialogueManager>
{
    protected override bool TryCreateComponentCache(GameObject gameObject, out DialogueManager componentCache)
    {
        componentCache = gameObject.GetComponent<DialogueManager>();
        return componentCache != null;
    }

}
