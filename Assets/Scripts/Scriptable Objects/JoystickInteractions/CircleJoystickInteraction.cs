using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Joystick Interactions/New CircleInteraction")]
public class CircleJoystickInteraction : IJoystickInteraction
{
    [SerializeField]
    //ReadOnly car le counter clockwise ne marche pas encore
    private bool isClockwise = true;

    [SerializeField]
    private int subAreas = 8;

    /**
     * Idée Général :
     * On sous-divise un cercle en N part de camembert
     * On normalise le vecteur input et on regarde dans quel part du camembert il tombe
     * On ne valide chaque part du camembert que si elles sont passées dans le bon ordre,
     * et on compte le nombre de part validés
     * 
     * lorsque le threshold est atteint, on valide l'event et on reset le count
     * */

    /// <summary>
    /// Nombre de zone consécutives réussi
    /// </summary>
    private int correctAreaReached = 0;

    [SerializeField]
    [ReadOnly]
    private bool isReset = true;
    
    private int nextArea = 0;
    
    private void Reset()
    {
        isReset = true;
        correctAreaReached = 0;
    }

    public override bool IsJoystickInteractionValid()
    {
        //Si on a fait toutes les zones + 1 (retour celle de départ)
        if (correctAreaReached > subAreas)
        {
            Reset(); //on flush toute progression
            return true;
        }

        return false;
    }

    public override Vector3 JoystickInteraction(Vector3 inputAxis)
    {

        Vector2 joystickAxis = inputAxis;

        //Si l'input est trop faible == au repos, on reset
        if (joystickAxis.magnitude <= Mathf.Epsilon)
        {
            Reset();
        }
        else
        {
            int currentArea = GetCurrentArea(joystickAxis.normalized);

            //Si c'est le premier input
            if (isReset || currentArea == nextArea)
            {
                correctAreaReached++;
                nextArea = GetNextArea(currentArea);
                isReset = false;
            }

        }

        return joystickAxis.normalized;
    }

    private int GetNextArea(int currentArea)
    {
        int nextArea;

        if (isClockwise)
        {
            nextArea = currentArea + 1;
            if (nextArea >= subAreas)
            {
                return 0;
            }
        }
        else
        {
            nextArea = currentArea - 1;
            if (nextArea < 0)
            {
                return subAreas - 1;
            }
        }

        return nextArea;
    }


    private int GetCurrentArea(Vector2 direction)
    {
        //float radAngle = Mathf.Atan2(direction.x, direction.y);
        float angle = ((Mathf.Atan2(direction.x, direction.y) / Mathf.PI) * 180f);

        if (angle < 0f)
            angle += 360f;

        int angleRounded = Mathf.RoundToInt(angle);

        //On divise pour savoir dans quel zone il tombe.
        return Mathf.FloorToInt(angle / (360f / (float)subAreas));

    }
    
}
