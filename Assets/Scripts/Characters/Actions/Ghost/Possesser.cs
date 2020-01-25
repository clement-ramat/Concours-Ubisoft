using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Possesser : MonoBehaviour
{
    [SerializeField]
    [Required]
    private Ghost ghost;

    //Initial parent of the ghost
    private Transform initialParent;

    [Title("Possessing")]
    [SerializeField]
    [ReadOnly]
    protected GameObject possessedObject;

    public GameObject PossessedObject
    {
        get
        {
            return possessedObject;
        }
    }
    
    public PossessInteraction PossessInteraction
    {
        get
        {
            return possessedObject?.GetComponent<PossessInteraction>();
        }
    }

    public void Possess(GameObject go) {
        

        //save parent and set new one
        initialParent = ghost.transform.parent;
        ghost.transform.SetParent(go.transform);

        ghost.SetGhostTangibility(false);

        possessedObject = go;

        PossessInteraction.OnPossess();
    }

    public bool Unpossess()
    {
        //Check if possessing
        if (possessedObject == null)
        {
            return false; //no possessing
        }
        

        //Set back parents
        ghost.transform.SetParent(initialParent);

        ghost.SetGhostTangibility(true);

        PossessInteraction.OnUnpossess();
        possessedObject = null;

        return true;

    }

    public bool IsPossessing()
    {
        return possessedObject != null;
    }
    

    private void SetGhostVisibility(bool visible)
    {
        ghost.SetCharacterVisible(visible);
    }
}
