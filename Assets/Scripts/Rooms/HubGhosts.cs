using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using ThirteenPixels.Soda;
using NodeCanvas.BehaviourTrees;

public class HubGhosts : MonoBehaviour
{
    [Title("", "Ghosts")]

    [SerializeField]
    private GameObject Ghost_Baptiste;

    [SerializeField]
    private GameObject Ghost_Clemence;

    [SerializeField]
    private GameObject Ghost_Clement;

    [SerializeField]
    private GameObject Ghost_Dorian;

    [SerializeField]
    private GameObject Ghost_Elie;

    [SerializeField]
    private GameObject Ghost_Laura;

    [SerializeField]
    private GameObject Ghost_Raph;

    [SerializeField]
    private GameObject Ghost_Victor;

    [Title("", "Global variables")]

    [SerializeField]
    private GlobalBool painting1;

    [SerializeField]
    private GlobalBool painting2;

    [SerializeField]
    private GlobalBool painting3;

    private bool Painting1Bool
    {
        get
        {
            return painting1.value;
        }
    }

    private bool Painting2Bool
    {
        get
        {
            return painting2.value;
        }
    }

    private bool Painting3Bool
    {
        get
        {
            return painting3.value;
        }
    }

    void Awake()
    {
        if(Painting1Bool)
        {
            // clemence, raph, clement
            Ghost_Clemence.SetActive(true);
            Ghost_Clemence.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();

            Ghost_Raph.SetActive(true);
            Ghost_Raph.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();

            Ghost_Clement.SetActive(true);
            Ghost_Clement.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();
        }

        if(Painting2Bool)
        {
            // victor, dorian, elie
            Ghost_Victor.SetActive(true);
            Ghost_Victor.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();

            Ghost_Dorian.SetActive(true);
            Ghost_Dorian.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();

            Ghost_Elie.SetActive(true);
            Ghost_Elie.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();
        }

        if(Painting3Bool)
        {
            // baptiste, laura
            Ghost_Baptiste.SetActive(true);
            Ghost_Baptiste.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();

            Ghost_Laura.SetActive(true);
            Ghost_Laura.GetComponent<GhostEnemy>().GetComponent<BehaviourTreeOwner>().StartBehaviour();
        }
    }


}
