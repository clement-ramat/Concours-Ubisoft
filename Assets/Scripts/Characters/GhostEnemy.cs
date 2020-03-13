using Mirror;
using NodeCanvas.BehaviourTrees;
using NodeCanvas.Framework;
using Sirenix.OdinInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;
using ThirteenPixels.Soda;
[System.Serializable]
public class MyEventBool : UnityEvent<bool> { }

public class GhostEnemy : Ghost
{
    public enum AI_Type
    {
        Cowardly,
        Aggressive
        //Vengeful
    }

    [Title("Ghost Enemy Settings")]

    [Title("", "AI Behaviour")]
    [SerializeField]
    public AI_Type aiType = AI_Type.Cowardly;

    [SerializeField]
    private NavMeshAgent meshAgent;

    [SerializeField]
    [ReadOnly]
    [SyncVar(hook = nameof(OnClientIsRevealedChange))]
    private bool isRevealed = false;

    [SerializeField]
    private float revealTime = 5f;

    [Title("", "Flashing")]
    [SerializeField]
    [ReadOnly]
    private bool isFlashed = false;

    [SerializeField]
    private float reviveTime = 3f;

    [SerializeField]
    private GhostRespawner ghostRespawner = null;

    [SerializeField]
    private GameObject deathParticleEffect = null;

    [SerializeField]
    private GameObject fleeParticleEffect = null;


    [SerializeField]
    private bool hasLoot = false;

    [Title("", "Global References")]
    [SerializeField]
    private GlobalPlayerGhost ghostPlayer = null;
     
    [SerializeField]
    private GlobalPlayerHuman humanPlayer = null;

    [SerializeField]
    private GlobalBool hasBeenCornered;


    [SerializeField]
    private GameObject graphRef = null;
    private Graph graph = null;

    [Title("", "Events")]
    [SerializeField]
    private MyEventBool OnRevealedChangedEvent;

    private float revealTimeLeft = 0.0f;

    private NetworkAnimator networkAnimator;
    private Animator animator;

    private List<ParticleSystem> playingParticles;

    private void Start()
    {
        networkAnimator = GetComponentInChildren<NetworkAnimator>();
        animator = GetComponentInChildren<Animator>();
    }


    public override void OnStartClient()
    {
        base.OnStartClient();

        if (isClientOnly)
        {
            GetComponent<BehaviourTreeOwner>().StopBehaviour();
            GetComponent<NavMeshAgent>().enabled = false;
            GetComponent<CharacterMovement>().enabled = false;
        }
    }

    /// <summary>
    /// Reference vers le GameObject GhostPlayer, utilisé par NodeCanvas (bind)
    /// </summary>
    public GameObject GhostPlayerGameObject
    {
        get
        {
            return ghostPlayer.componentCache.gameObject;
        }
    }

    public GameObject HumanPlayerGameObject
    {
        get
        {
            //if (humanPlayer != null)
                return humanPlayer.componentCache.gameObject;
            // for test purposes
            //else return ghostPlayer.componentCache.gameObject;
        }
    }

    public bool HasBeenCorneredBool
    {
        get
        {
            return hasBeenCornered.value;
        }

        set
        {
            hasBeenCornered.value = value;
        }
    }

    public Graph Graph
    {
        get
        {
            if (graph == null) GenerateGraph();
            return graph;
        }
    }

    [Server]
    public void GenerateGraph()
    {
        if(graphRef != null)
        {
            graph = new Graph();
            graph.nodes = new List<Node>();
            Node[] nodes = graphRef.GetComponentsInChildren<Node>();

            foreach(Node n in nodes)
            {
                n.edges = GetEdges(n);
                graph.nodes.Add(n);
            }
        }
    }

    [Server]
    private List<Edge> GetEdges (Node n)
    {
        List<Edge> nodeEdges = new List<Edge>();
        if (graphRef != null)
        {
            Edge[] edges = graphRef.GetComponentsInChildren<Edge>();

            foreach(Edge e in edges)
            {
                if ((e.nodeA == n) || (e.nodeB == n)) nodeEdges.Add(e);
            }
        }
          
        return nodeEdges;
    }

    public GhostRespawner GhostRespawner
    {
        get
        {
            return ghostRespawner;
        }

        set
        {
            ghostRespawner = value;
        }
    }

    private Coroutine flashTimerCoroutine = null;

    public bool IsFlashed { get => isFlashed; }

    public bool IsRevealed { get => isRevealed; }
     

    private void Awake()
    {
        playingParticles = new List<ParticleSystem>();
        if (meshAgent == null) meshAgent = GetComponent<NavMeshAgent>();

        //if(isServer)
        //{
        //    if(hasBeenCornered.value)
        //    {
        //        // coming back to the room
        //        RpcSetGhostActive(true);
        //    }
        //}
    }

    //public override void OnStartClient()
    //{
    //    base.OnStartClient();

    //    NavMeshAgent nma = GetComponent<NavMeshAgent>();
    //    if (nma != null)
    //    {
    //        nma.enabled = false;
    //    }

    //   // GetComponent<BehaviourTreeOwner>().StopBehaviour();
    //}

    [Server]
    public bool IsPossessedLight()
    {
        if (possesser.PossessedObject.GetComponent<LightPossessInteraction>() != null)
        {
            return true;
        }
        else
            return false;
    }

    [Server]
    public bool HumanExistInScene()
    {
        return humanPlayer.componentCache != null;
    }

    [Server]
    public bool GhostExistInScene()
    {
        return ghostPlayer.componentCache != null;
    }


    public void Flash()
    {
        if (isRevealed && !isFlashed) 
        {
            isFlashed = true;

            // death anim
            if ((deathParticleEffect != null) && isServer)
            {
                PlayDeathParticles();
                RpcPlayDeathParticles();
            }

            if (isServer && hasLoot)
            {
                GetComponent<EnigmeReward>().DropReward();
            }

            if(ghostRespawner != null)
            {
                // Stop Behaviour Tree
                GetComponent<BehaviourTreeOwner>().StopBehaviour();

                // revive 
                isFlashed = false;
                if(isServer) ghostRespawner.ReviveGhost(gameObject, reviveTime);
            }else
            {
                RpcResetParticles();
                NetworkServer.Destroy(gameObject);
            }
        }
    }


    public void PlayDeathParticles()
    {
        ParticleSystem[] deathParticles = deathParticleEffect.GetComponentsInChildren<ParticleSystem>();

        foreach (ParticleSystem particle in deathParticles)
        {
            ParticleSystem ps = Instantiate(particle, transform.position, Quaternion.identity);
            playingParticles.Add(ps);
            ps.Play();
        }
    }

    [ClientRpc]
    public void RpcPlayDeathParticles()
    {
        PlayDeathParticles();
    }


    public void ResetParticles()
    {
        foreach (ParticleSystem ps in playingParticles)
        {
            ps.Stop();
        }

        playingParticles.Clear();
    }


    [ClientRpc]
    public void RpcResetParticles()
    {
        ResetParticles();
    }


    [ClientRpc]
    public void RpcSetGhostActive(bool active)
    {
        if(active)
        {
            GetComponent<BehaviourTreeOwner>().StartBehaviour();
            gameObject.SetActive(true);
        }
        else
        {
            GetComponent<BehaviourTreeOwner>().StopBehaviour();

            if (fleeParticleEffect != null)
            {
                GameObject go = Instantiate(fleeParticleEffect, transform.position, Quaternion.identity);
                go.GetComponent<ParticleSystem>().Play();

                // disappear in the middle of the smoke animation
                StartCoroutine(EndAfterSeconds(go.GetComponent<ParticleSystem>().main.duration / 2));
            }
            else
            {
                HasBeenCorneredBool = true;
                gameObject.SetActive(false);
            }
        }
    }

    private IEnumerator EndAfterSeconds(float seconds)
    {
        yield return new WaitForSeconds(seconds);

        HasBeenCorneredBool = true;
        gameObject.SetActive(false);
    }

    //public IEnumerator ReviveGhost(float reviveTime)
    //{
    //    //kill
    //    isFlashed = false;
    //    gameObject.SetActive(false);

    //    // revive
    //    yield return new WaitForSeconds(reviveTime);
    //    gameObject.SetActive(true);
    //    GetComponent<BehaviourTreeOwner>().StartBehaviour();
    //}

    [Server][Button]
    public void Reveal()
    {
        isRevealed = true;
        revealTimeLeft = revealTime;

        //Debug.Log("Reveal   IsPossessing == " + (possesser.PossessedObject != null));
        if (IsPossessing()) ForceUnPossess();
    }

    [Server]
    public void UseObjectMeshAgent()
    {
        //Disable ghost agent
        meshAgent.enabled = false;

        //Debug.Log("use object mesh");
        if (possesser.PossessedObject.GetComponent<Rigidbody>())
            possesser.PossessedObject.GetComponent<Rigidbody>().isKinematic = true;

        if (possesser.PossessedObject.GetComponent<LightPossessInteraction>() != null)
        {
            possesser.PossessedObject.GetComponent<NavMeshObstacle>().enabled = false;
            possesser.PossessedObject.GetComponent<NavMeshAgent>().enabled = true;

            meshAgent = possesser.PossessedObject.GetComponent<NavMeshAgent>();
        }

    }

    [Server]
    public bool Possess()
    {
        if (possesser.IsPossessing() || interactionArea.InteractableObject == null)
        {

            return false;
        }

        PossessInteraction possessInteraction = interactionArea.InteractableObject.GetComponent<PossessInteraction>();

        if (!possessInteraction)
            return false;


        return Possess(possessInteraction);
    }


    [Server]
    public void UseGhostMeshAgent()
    {
        //Debug.Log("use ghost mesh");
        if (possesser.PossessedObject.GetComponent<Rigidbody>())
            possesser.PossessedObject.GetComponent<Rigidbody>().isKinematic = false;

        if (possesser.PossessedObject.GetComponent<LightPossessInteraction>() != null)
        {
            possesser.PossessedObject.GetComponent<NavMeshAgent>().enabled = false;
            possesser.PossessedObject.GetComponent<NavMeshObstacle>().enabled = true;
        }

        meshAgent = GetComponent<NavMeshAgent>();

        //Re-enable ghost agent
        meshAgent.enabled = true;
    }


    private void OnClientIsRevealedChange(bool oldValue, bool newValue)
    {
        if(OnRevealedChangedEvent != null)
        {
            OnRevealedChangedEvent.Invoke(newValue);
        }
    }

    [Server]
    protected override void Update()
    {
        base.Update();

        revealTimeLeft -= Time.deltaTime;
        if(revealTimeLeft < 0)
        {
            isRevealed = false;
        }

        if(animator != null)
        {
            animator.SetFloat("Velocity", meshAgent.velocity.magnitude / meshAgent.speed);
        }
    }

    [Server]
    public void PlayAttackAnimation()
    {
        if(networkAnimator != null)
        {
            networkAnimator.SetTrigger("Attack");
        }
    }
}
