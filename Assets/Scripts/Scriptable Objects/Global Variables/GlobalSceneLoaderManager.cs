using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

[CreateAssetMenu(menuName = "GlobalVariable/Scene Loader Manager")]
public class GlobalSceneLoaderManager : GlobalGameObjectWithComponentCacheBase<SceneLoaderManager>
{

    protected override bool TryCreateComponentCache(GameObject gameObject, out SceneLoaderManager componentCache)
    {
        componentCache = gameObject.GetComponent<SceneLoaderManager>();
        return componentCache != null;
    }
}
