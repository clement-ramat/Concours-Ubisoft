using UnityEngine;
using NodeCanvas.Framework;



public class MyTestTask : ActionTask
{
    protected override void OnExecute()
    {
        Debug.Log("FLASHED STATE " + agent.name);
        EndAction(true);
    }
}




