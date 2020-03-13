using UnityEngine;
using NodeCanvas.Framework;


public class IsPossessed : ConditionTask
{
    [SerializeField]
    public BBParameter<GameObject> myObject;

    protected override bool OnCheck()
    {
        //Debug.Log("check : " + myObject.value);
        return myObject.value.GetComponent<PossessInteraction>().IsPossessed;
    }
}