using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CubeUseBehaviour : UseInteraction
{
    public override void HumanAction(Human character)
    {
        Debug.Log("Human Action: USE CUBE");
        Debug.Log("Used by : " + character);
    }

    public override void GhostAction(Ghost character)
    {
        Debug.Log("Ghost Action: USE CUBE");
        Debug.Log("Used by : " + character);
    }

    
}

