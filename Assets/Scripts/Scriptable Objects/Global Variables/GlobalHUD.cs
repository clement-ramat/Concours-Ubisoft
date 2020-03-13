using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(menuName = "GlobalVariable/GlobalHUD")]
public class GlobalHUD : GlobalGameObjectWithComponentCacheBase<HUDBehavior>
{
    protected override bool TryCreateComponentCache(GameObject gameObject, out HUDBehavior componentCache)
    {
        componentCache = gameObject.GetComponent<HUDBehavior>();
        return componentCache != null;
    }

}
