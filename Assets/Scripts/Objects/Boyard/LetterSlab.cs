using Sirenix.OdinInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(SlabToggle))]
public class LetterSlab : SerializedMonoBehaviour
{
    [SerializeField]
    private char letter;

    [NonSerialized]
    public Action<LetterSlab> OnToggle;
    [NonSerialized]
    public Action<LetterSlab> OnUntoggle;

    private SlabToggle slab;

    private bool ranOnce = false;

    public SlabToggle Slab {

        get {

            if (slab == null)
            {
                slab = GetComponent<SlabToggle>();
            }

            return slab;
        }
    }
    public char Letter { get => letter; }
    
    private void Awake()
    {

        slab = GetComponent<SlabToggle>();

        //slab.OnToggle.AddListener(() => Debug.Log("tick"));
        slab.OnToggle.AddListener(() => OnToggle?.Invoke(this));
        slab.OnUntoggle.AddListener(() => OnUntoggle?.Invoke(this));
    }
    

    //Je suis désolé - elie
    public bool RunOnce(Action function)
    {
        if (ranOnce)
        {
            return false;
        }

        function?.Invoke();

        ranOnce = true;
        return true;
    }

    public bool LetterIsEqual(char otherLetter)
    {
        return char.ToUpper(letter).Equals(char.ToUpper(otherLetter));
    }


}

public class MyLetterUnityEvent : UnityEvent<LetterSlab>
{

}
