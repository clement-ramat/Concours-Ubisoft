﻿using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;
using System.Collections.Generic;

#if UNITY_5_5_OR_NEWER
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
using NavMesh = UnityEngine.AI.NavMesh;
using NavMeshHit = UnityEngine.AI.NavMeshHit;
#endif

namespace NodeCanvas.Tasks.Actions
{

    public class FleeFromHuman : ActionTask<NavMeshAgent>
    { 

        [RequiredField]
        public BBParameter<float> speed = 4f;
        public BBParameter<float> fledDistance = 10f;
        public BBParameter<float> lookAhead = 2f;

        private GameObject target;
        //private NavMeshHit hit;
        //private float maxWallDistance = 6f;

        protected override void OnExecute()
        {
            GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();
            if (ghostEnemy.HumanExistInScene())
            {
                target = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>().gameObject;
            }
            else
            {
                //Debug.Log("pas de human");
                EndAction(false);
                return;
            }

            agent.speed = speed.value;
            if ((agent.transform.position - target.transform.position).magnitude >= fledDistance.value)
            {
                EndAction(true);
                return;
            }

        }

        protected override void OnUpdate()
        {
            var targetPos = target.transform.position;
            if ((agent.transform.position - targetPos).magnitude >= fledDistance.value)
            {
                EndAction(true);
                return;
            }

            var fleePos = targetPos + (agent.transform.position - targetPos).normalized * (fledDistance.value + lookAhead.value + agent.stoppingDistance);
            if (!agent.SetDestination(fleePos))
            {
                EndAction(false);
            }
        }

        //protected override void OnUpdate()
        //{
        //    var targetPos = target.transform.position;
        //    if ((agent.transform.position - targetPos).magnitude >= fledDistance.value)
        //    {
        //        agent.GetComponent<CharacterMovement>().MoveInput = Vector3.zero;
        //        EndAction(true);
        //        return;
        //    }

        //    // direction from player to AI
        //    Vector3 fleePos = (agent.transform.position - targetPos).normalized;

        //    RaycastHit hit;
        //    // if there is a close wall in this direction
        //    if (Physics.Raycast(agent.transform.position, fleePos, out hit, maxWallDistance))
        //    {
        //        if(Vector3.Distance(agent.transform.position, targetPos) > 3.5f)
        //        {
        //            // also use raycasts to dodge walls
        //            float wallsWeight = 1.10f;
        //            Vector3 fleeWalls = ComputeWallsExitDirection();
        //            fleePos = (fleePos + (fleeWalls * wallsWeight)).normalized;

        //            //Debug.Log("----- COMPUTING -----");
        //            //Debug.Log("Player: " + ((agent.transform.position - targetPos).normalized));
        //            //Debug.Log("Walls: " + fleeWalls);
        //            //Debug.Log("Result: " + fleePos);
        //            //Debug.Log("---------------------");
        //            //Debug.DrawRay(agent.transform.position, (agent.transform.position - targetPos) * 20, Color.blue);
        //            //Debug.DrawRay(agent.transform.position, fleePos * 20, Color.red);
        //            //Debug.DrawRay(agent.transform.position, fleeWalls * 20, Color.green);
        //        }
        //        else
        //        {
        //            // AI is very close to both a wall and a player
        //            Vector3 v = GetFarthestWallDir().normalized;
        //            fleePos = (fleePos + v).normalized;
        //        }

        //        Debug.DrawRay(agent.transform.position, (agent.transform.position - targetPos) * 20, Color.blue);
        //        Debug.DrawRay(agent.transform.position, fleePos * 20, Color.red);

        //    }

        //    fleePos.y = 0f;

        //    // Move in this direction
        //    agent.GetComponent<CharacterMovement>().MoveInput = new Vector3(fleePos.x, fleePos.z,0);
        //}


        //private Vector3 ComputeWallsExitDirection()
        //{
        //    //Debug.Log("----- COMPUTING -----");

        //    List<System.Tuple<RaycastHit, Vector3>> raycastTuples = RaycastSurroundingWalls(16);

        //    Vector3 exitDir = new Vector3(0, 0, 0);

        //    int veryCloseWalls = 0;
        //    List<Vector3> veryImportantVectors = new List<Vector3>();

        //    foreach(System.Tuple<RaycastHit, Vector3> t in raycastTuples)
        //    {
        //        float rayWeight = 0.15f;
        //        //Debug.Log("Distance: " + t.Item1.distance);

        //        // compute the weight of the collisioned wall
        //        if ((t.Item1.distance < (maxWallDistance * 0.5f)) && (t.Item1.distance > 0f))
        //        {
        //            rayWeight = (1f / t.Item1.distance);

        //            // if AI is touching a wall
        //            if (t.Item1.distance < 0.8f)
        //            {
        //                // trying to detect if the AI is in a corner
        //                veryCloseWalls++;
        //                veryImportantVectors.Add(t.Item2);
        //                rayWeight *= 2;
        //            }
        //            else
        //            {
        //                rayWeight = Mathf.Clamp(rayWeight, 0.1f, 0.5f);
        //            }
        //        }

        //        exitDir += (t.Item2.normalized * rayWeight);

        //        //Debug.DrawLine(agent.transform.position, agent.transform.position + t.Item2.normalized * rayWeight * 10f, Color.cyan);
        //    }

        //    // if AI is in a corner
        //    if (veryCloseWalls >= 4)
        //    {
        //        foreach(Vector3 v in veryImportantVectors)
        //        {
        //            //Debug.Log("+ : " + v);
        //            exitDir += v.normalized;
        //        }
        //        //Debug.Log("*** new V: " + exitDir);
        //    }

        //    exitDir = exitDir.normalized * (-1);
        //    //Debug.Log("exit dir: " + exitDir);
        //   // Debug.Log("----------");
        //    return exitDir;

        //}

        //private Vector3 GetFarthestWallDir()
        //{
        //    List<System.Tuple<RaycastHit, Vector3>> raycastTuples = RaycastSurroundingWalls(8);

        //    Vector3 dir = Vector3.zero;
        //    float maxDist = 0;
        //    foreach (System.Tuple<RaycastHit, Vector3> t in raycastTuples)
        //    {
        //        if (dir == null)
        //        {
        //            dir = t.Item2;
        //            maxDist = Vector3.Distance(agent.transform.position, t.Item2);
        //        }
        //        else
        //        {
        //            float tmp = Vector3.Distance(agent.transform.position, t.Item2);
        //            if (tmp > maxDist)
        //            {
        //                dir = t.Item2;
        //                maxDist = tmp;
        //            }
        //        }
        //    }

        //    return dir;
        //}

        //private List<System.Tuple<RaycastHit, Vector3>> RaycastSurroundingWalls(int nbRays)
        //{
        //    float degreeOffset = 360f / nbRays;

        //    Vector3 startingDirection = agent.transform.position.normalized;

        //    float startingDegree = Vector3.SignedAngle(Vector3.left, startingDirection, Vector3.up);

        //    List<System.Tuple<RaycastHit, Vector3>> raycastTuples;
        //    raycastTuples = new List<System.Tuple<RaycastHit, Vector3>>();

        //    // Make raycasts in circle
        //    for (int i = 0; i < nbRays; i++)
        //    {
        //        float curDegree = ((degreeOffset * i) + startingDegree) % 360;

        //        float incrRad = curDegree * (Mathf.PI / 180);

        //        Vector3 direction = (new Vector3(-Mathf.Cos(incrRad), 0, Mathf.Sin(incrRad)).normalized);

        //        RaycastHit hit;

        //        // If we hit a wall, store this raycast and its direction
        //        if (Physics.Raycast(agent.transform.position, direction, out hit, maxWallDistance))
        //        {
        //            raycastTuples.Add(new System.Tuple<RaycastHit, Vector3>(hit, direction));
        //        }
        //    }

        //    return raycastTuples;
        //}


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