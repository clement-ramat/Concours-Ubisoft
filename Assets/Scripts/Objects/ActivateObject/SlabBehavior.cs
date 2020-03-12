using Mirror;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// The slab activate itself when nbGhostsAndHumansToActivate are been on top for timeToActivate seconds.
/// TODO : Change the visual representation for more feedbacks (maybe when on player is on top, the slab show progress half way ?)
/// </summary>
public class SlabBehavior : ActivateObject
{

    [Title("Slab Settings")]

    [SerializeField]
    private int nbGhostsAndHumansToActivate = 2;

    [Title("Slab Animation")]
    [SerializeField][ChildGameObjectsOnly]
    private Transform slabModelTransform;

    [SerializeField]
    private float animationSpeed = 1.5f;

    [SerializeField]
    private float offsetY;

    [SerializeField]
    [ReadOnly]
    private List<Character> playersOnTop = new List<Character>();

    [SyncVar]
    [SerializeField]
    [ReadOnly]
    private int nbPeopleOnTop;

    private Vector3 slabInitalPosition;

    private void Start()
    {
        slabInitalPosition = slabModelTransform.position;
    }

    public override void Activate()
    {
        Activated = true;
    }

    private void Update()
    {

        VerifyDepossess();

        
        if (isServer)
        {
            nbPeopleOnTop = playersOnTop.Count;
        }

        // --- Animate the slab ---
        Vector3 target = !Activated ? slabInitalPosition : new Vector3(slabInitalPosition.x, slabInitalPosition.y + offsetY, slabInitalPosition.z);
        slabModelTransform.position = Vector3.Lerp(slabModelTransform.position, target, Time.deltaTime * animationSpeed);
        // ------------------------
    }

    [Server]
    private void OnTriggerEnter(Collider other)
    {
        if (!CanBeActivate)
        {
            return;
        }
        

        if (IsPlayerCharacter(other.gameObject))
        {
            Character character = GetCharacter(other.gameObject);
            if (!playersOnTop.Contains(character))
            {
                playersOnTop.Add(character);

                if (playersOnTop.Count >= nbGhostsAndHumansToActivate)
                {
                    Activate();
                }
            }
            
        }
    }


    [Server]
    private void OnTriggerExit(Collider other)
    {
        if (!CanBeActivate)
        {
            return;
        }

        if (IsPlayerCharacter(other.gameObject))
        {
            Character character = GetCharacter(other.gameObject);

            playersOnTop.Remove(character);
            if (playersOnTop.Count < nbGhostsAndHumansToActivate)
            {
                Activated = false;
                Debug.Log("Slab On Trigger Exit good");
            }

        }
    }

    private void VerifyDepossess()
    {
        foreach (Character character in playersOnTop)
        {
            if (character is GhostPlayer)
            {
                GhostPlayer ghostPlayer = (GhostPlayer)character;

                if (ghostPlayer.IsPossessing())
                {
                    playersOnTop.Remove(character);

                    if (playersOnTop.Count < nbGhostsAndHumansToActivate)
                    {
                        Activated = false;
                    }

                }
            }
        }
    }

    private Character GetCharacter(GameObject go)
    {
        Character component;

        component = go.GetComponent<Character>();

        if (component != null)
        {
            return component;
        }
        else
        {
            component = go.GetComponentInParent<Character>();
            return component;
        }
    }
    private bool IsPlayerCharacter(GameObject go)
    {
        return HasComponent<Human>(go) || HasComponent<GhostPlayer>(go);
    }
    
    private bool HasComponent<T>(GameObject go) where T : Character
    {
        T component;

        component = go.GetComponent<T>();

        if (component != null)
        {
            return true;
        }
        else
        {
            component = go.GetComponentInParent<T>();
            return component != null;
        }
    }
}
