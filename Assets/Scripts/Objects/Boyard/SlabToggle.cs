using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class SlabToggle : MonoBehaviour
{

    [SerializeField] [ReadOnly]
    private bool isToggled = false;

    public UnityEvent OnToggle;
    public UnityEvent OnUntoggle;

    private int OnEnterTriggerCount = 0;

    public bool IsToggled { get => isToggled; }

    private void OnTriggerEnter(Collider other)
    {

        isToggled = true;

        //First toggle from untoggle
        if (OnEnterTriggerCount == 0)
        {
            OnToggle?.Invoke();
        }
        OnEnterTriggerCount++;
    }

    private void OnTriggerExit(Collider other)
    {

        OnEnterTriggerCount--;

        //Last object to leave the slab
        if (OnEnterTriggerCount == 0)
        {
            isToggled = false;
            OnUntoggle?.Invoke();
        }
    }

    private void OnDisable()
    {
        if (isToggled)
        {
            OnUntoggle?.Invoke();
            isToggled = false;
        }

        OnEnterTriggerCount = 0;

    }

}
