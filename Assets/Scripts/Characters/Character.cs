using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Parent class to all characters (players, ghost, human)
/// </summary>
public abstract class Character : MonoBehaviour
{
   
    [Title("References", "Visuals")]
    [SerializeField]
    [Required]
    protected Transform visualsParent;

    [Title("", "Mechanics")]

    [SerializeField]
    [Required]
    protected Rigidbody rb;


    [SerializeField]
    [Required]
    protected CharacterMovement characterMovement;

    [SerializeField]
    [Required]
    protected InteractionArea interactionArea;
    
    public void SetCharacterVisible(bool visible)
    {
        MeshRenderer[] meshes = GetComponentsInChildren<MeshRenderer>();

        for (int i = 0; i < meshes.Length; i++)
        {
            meshes[i].enabled = visible;
        }
    }




}
