using UnityEngine;
using NodeCanvas.Framework;

public class ManipulateHeavyObject : ActionTask
{

    protected override void OnExecute()
    {

        //agent.GetComponent<Ghost>().Possesser.PossessInteraction.JoystickInteraction(Vector3.left);
        
        HeavyPossessInteraction heavy = agent.GetComponent<Ghost>().Possesser.PossessInteraction.GetComponent<HeavyPossessInteraction>();

        if (heavy)
        {
            heavy.ForceExecutePossessInteraction();
            EndAction(true);
        }
        else
        {
            EndAction(false);
        }

    }
}

