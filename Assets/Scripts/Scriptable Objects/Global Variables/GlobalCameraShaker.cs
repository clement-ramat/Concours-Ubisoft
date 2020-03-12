using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(menuName = "GlobalVariable/GlobalCameraShaker")]
public class GlobalCameraShaker : GlobalGameObjectWithComponentCacheBase<CameraShaker>
{
    protected override bool TryCreateComponentCache(GameObject gameObject, out CameraShaker componentCache)
    {
        componentCache = gameObject.GetComponent<CameraShaker>();
        return componentCache != null;
    }

}
