using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;
using NodeCanvas.BehaviourTrees;

#if UNITY_5_5_OR_NEWER
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
using NavMesh = UnityEngine.AI.NavMesh;
using NavMeshHit = UnityEngine.AI.NavMeshHit;
#endif

namespace NodeCanvas.Tasks.Actions
{
    public class FleeFromPlayersGraph : ActionTask<NavMeshAgent>
    {
        [RequiredField]
        public BBParameter<float> speed = 4f;
        public BBParameter<float> fledDistance = 10f;
        public BBParameter<float> lookAhead = 2f;
        public BBParameter<Graph> graph;

        private GameObject ghostPlayer;
        private GameObject humanPlayer;

        private GhostEnemy ghostEnemy;

        private Transform destination;
        private int criticalDistance = 2;

        private Node ghostNode;
        private Node humanNode;
        private Node aiNode;
      
        private bool isCornered = false;

        protected override void OnExecute()
        {
            ghostEnemy = agent.GetComponent<GhostEnemy>();
            if (ghostEnemy.HumanExistInScene())
            {
                humanPlayer = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>().gameObject;
                if (ghostEnemy.GhostExistInScene())
                {
                    ghostPlayer = ghostEnemy.GhostPlayerGameObject.GetComponent<GhostPlayer>().gameObject;
                } else
                {
                    // !!! for test purposes only !!!
                    //ghostPlayer = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>().gameObject;

                    EndAction(false);
                    return; 
                }
            }
            else
            {
                EndAction(false);
                return;
            }

            agent.speed = speed.value;
        }

        protected override void OnUpdate()
        {
            if((!isCornered && !ghostEnemy.HasBeenCorneredBool) || ghostEnemy.HasBeenCorneredBool)
            {
                if (!agent.pathPending && agent.remainingDistance < 2f)
                {
                    ResetNodes();
                    UpdatePositions();

                    // if there are players in a "radius" of 2 nodes
                    if (SearchPlayers())
                    {
                        ResetNodes();
                        destination = ChooseNextPoint();
                        GotoPoint(destination);
                    }
                }
            }else
            {
                if(!ghostEnemy.HasBeenCorneredBool)
                {
                    // isCornered first time
                    //ghostEnemy.HasBeenCorneredBool = true;
                    agent.GetComponent<GhostEnemy>().RpcSetGhostActive(false);
                }else
                {
                    // back in the room
                    if(isCornered) isCornered = false;
                }
            }
        }


        private void UpdatePositions()
        {
            aiNode = GetNearestNode(agent.transform);
            ghostNode = GetNearestNode(ghostPlayer.transform);
            humanNode = GetNearestNode(humanPlayer.transform);

            //Debug.Log("AI node : " + aiNode.id);
            //Debug.Log("ghost node : " + ghostNode.id);
            //Debug.Log("human node : " + humanNode.id);
        }

        private bool SearchPlayers(int maxDist = 2)
        {
            Node agentNode = aiNode;
            agentNode.explored = true;

            foreach (Edge e in agentNode.edges)
            {
                if (SearchPlayer(e.nodeA)) return true;
                else
                {
                    foreach (Edge eA in e.nodeA.edges)
                    {
                        if (SearchPlayer(eA.nodeA)) return true;
                        if (SearchPlayer(eA.nodeB)) return true;
                    }
                }
                if (SearchPlayer(e.nodeB)) return true;
                else
                {
                    foreach (Edge eA in e.nodeB.edges)
                    {
                        if (SearchPlayer(eA.nodeA)) return true;
                        if (SearchPlayer(eA.nodeB)) return true;
                    }
                }
            }

            return false;
        }

        private List<Node> SearchPlayersPaths()
        {
            List<Node> paths = new List<Node>();

            Node agentNode = aiNode;
            agentNode.explored = true;
      
            // methode hard codee, distance = 2
            // on adaptera ce code en recursif si besoin
            foreach (Edge e in agentNode.edges)
            {
                // since we are looking for several paths
                // we need to check if the nodes are already contained
                if (SearchPlayer(e.nodeA, true) && (!paths.Contains(e.nodeA))) paths.Add(e.nodeA);
                else
                {
                    foreach(Edge eA in e.nodeA.edges)
                    {
                        if (SearchPlayer(eA.nodeA, true))
                        {
                            if (!paths.Contains(eA.nodeA)) paths.Add(eA.nodeA);
                            if (!paths.Contains(eA.nodeB)) paths.Add(eA.nodeB);
                        }
                        if (SearchPlayer(eA.nodeB, true))
                        {
                            if (!paths.Contains(eA.nodeA)) paths.Add(eA.nodeA);
                            if (!paths.Contains(eA.nodeB)) paths.Add(eA.nodeB);
                        }
                    }
                }
                if (SearchPlayer(e.nodeB, true) && (!paths.Contains(e.nodeB))) paths.Add(e.nodeB);
                else
                {
                    foreach (Edge eB in e.nodeB.edges)
                    {
                        if (SearchPlayer(eB.nodeA, true))
                        {
                            if (!paths.Contains(eB.nodeA)) paths.Add(eB.nodeA);
                            if (!paths.Contains(eB.nodeB)) paths.Add(eB.nodeB);
                        }
                        if (SearchPlayer(eB.nodeB, true))
                        {
                            if (!paths.Contains(eB.nodeA)) paths.Add(eB.nodeA);
                            if (!paths.Contains(eB.nodeB)) paths.Add(eB.nodeB);
                        }
                    }
                }
            }

            return paths;
        }

        // Explore a node and look for a player on it
        private bool SearchPlayer(Node n, bool lookForSeveralPaths = false)
        {
            if (!n.explored)
            {
                if ((n == ghostNode) || (n == humanNode))
                {
                    // not setting "n.explored" to True allows to discover several possible paths !
                    if(!lookForSeveralPaths) n.explored = true;
                    return true;
                }else
                {
                    n.explored = true;
                }

            }

            return false;
        }

        // Same but add neighbours to a frontier
        private bool SearchPlayer(Node n, ref List<Node> frontier)
        {
            if (!n.explored)
            {
                n.explored = true;
                if ((n == ghostNode) || (n == humanNode)) return true;
                else
                {
                    foreach(Edge e in n.edges)
                    {
                        frontier.Add(e.nodeA);
                        frontier.Add(e.nodeB);
                    }
                }
            }

            return false;
        }

        // Returns the nearest node from the transform (can give the position of a player on the graph for example)
        private Node GetNearestNode(Transform t)
        {
            Node nearest = null;
            float min = -1;
            foreach(Node n in graph.value.nodes)
            {
                float tmp = Vector3.Distance(t.position, n.transform.position);

                if((min == -1) || (min > tmp))
                {
                    min = tmp;
                    nearest = n;
                }
            }

            return nearest;
        }

        private void ResetNodes(bool resetExploration = true, bool resetDesirability = true)
        {
            foreach (Node n in graph.value.nodes)
            {
                if(resetExploration) n.explored = false;
                if(resetDesirability) n.desirable = true;
            }
        }

        private Transform ChooseNextPoint()
        {
            // on elimine de nos possibilites les chemins que les joueurs pourraient choisir
            List<Node> undesirableNodes = SearchPlayersPaths();
            //Debug.Log("----");
            //Debug.Log("AI : " + aiNode.id);
            //Debug.Log("Ghost : " + ghostNode.id);
            //Debug.Log("Human : " + humanNode.id);
            foreach (Node n in undesirableNodes)
            {
                //Debug.Log("UNDESIRABLE: " + n.id);
                graph.value.nodes.Find(node => node.id == n.id).desirable = false;
            }

            List<Node> accessibleNodes = GetNeighbouringNodes(aiNode);

            // s'il y a des voisins accessibles on bouge
            if (accessibleNodes.Count > 0)
            {
                Node rnd = accessibleNodes[Random.Range(0, accessibleNodes.Count - 1)];
                Debug.Log("Goto: "+rnd.id);
                return rnd.transform;
                
            }else
            {
                Debug.Log("cornered");
                isCornered = true;
                Node free = LookForFreeNode();
                if (free != null) return free.transform;
                else return aiNode.transform;
            }
        }

        private Node LookForFreeNode()
        {
            foreach(Edge e in aiNode.edges)
            {
                if((e.nodeA != aiNode) && (e.nodeA != humanNode) && (e.nodeA != ghostNode))
                {
                    return e.nodeA;
                }
                else if ((e.nodeB != aiNode) && (e.nodeB != humanNode) && (e.nodeB != ghostNode))
                {
                    return e.nodeB;
                }
            }

            return null;
        }

        private List<Node> GetNeighbouringNodes(Node n, bool filterDesirability = true)
        {
            List<Node> neighbours = new List<Node>();

            foreach(Edge e in n.edges)
            {
                // NODE A
                // if the node isnt already stored and isnt the base node   
                if (!neighbours.Contains(e.nodeA) && (e.nodeA != n))
                {
                    // if we are only looking for desirable nodes
                    if (filterDesirability)
                    {
                        if(e.nodeA.desirable) neighbours.Add(e.nodeA);
                    }
                    else
                    {
                        neighbours.Add(e.nodeA);
                    }
                }

                // NODE B
                if (!neighbours.Contains(e.nodeB) && (e.nodeB != n))
                {
                    // if we are only looking for desirable nodes
                    if (filterDesirability)
                    {
                        if (e.nodeB.desirable) neighbours.Add(e.nodeB);
                    }
                    else
                    {
                        neighbours.Add(e.nodeB);
                    }
                }
            }

            return neighbours;
        }


        private void GotoPoint(Transform t)
        {
            agent.SetDestination(t.position);
        }

    
        protected override void OnPause() { OnStop(); }
        protected override void OnStop()
        {
            if (agent.gameObject.activeSelf)
            {
                agent.ResetPath();
            }
        }
    }
}

