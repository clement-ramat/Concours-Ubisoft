using System.Collections;
using System.Collections.Generic;
using ThirteenPixels.Soda;
using UnityEngine;

public class GlobalGameObjectRegisterDontDisable : MonoBehaviour
{
    [SerializeField]
    private GlobalGameObject globalGameObject = default;

    private void OnEnable()
    {
        if (globalGameObject == null)
        {
            Debug.LogError("GlobalGameObjectRegister doesn't have a GlobalGameObject assigned.", gameObject);
            enabled = false;
            return;
        }

        var couldSetValue = globalGameObject.TrySetValue(gameObject);

        if (!couldSetValue)
        {
            Debug.LogError("GlobalGameObjectRegister couldn't register the GameObject due to its components not matching the requirements.", gameObject);
            enabled = false;
            return;
        }

    }

    private void OnDisable()
    {
        if (globalGameObject == null) return;

        if (globalGameObject.value == gameObject)
        {
            globalGameObject.value = null;
        }
    }


}
