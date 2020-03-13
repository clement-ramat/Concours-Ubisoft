using NodeCanvas.Framework;
using ParadoxNotion.Design;
using UnityEngine;

#if UNITY_5_5_OR_NEWER
using NavMeshAgent = UnityEngine.AI.NavMeshAgent;
#endif

namespace NodeCanvas.Tasks.Actions
{
    public class MoveTowardsHuman : ActionTask<NavMeshAgent>
    {

        [RequiredField]
        public BBParameter<float> speed = 2;
        public BBParameter<float> keepDistance = 0.1f;
        private Vector3? lastRequest;

        private GameObject target;        

        protected override void OnExecute()
        {
            GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();
            if (ghostEnemy.HumanExistInScene())
            {
                target = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>().gameObject;
            }
            else
            {
                EndAction(false);
                return;
            }

            agent.speed = speed.value;
            if (Vector3.Distance(agent.transform.position, target.transform.position) < agent.stoppingDistance + keepDistance.value)
            {
                EndAction(true);
                return;
            }
        }

        protected override void OnUpdate()
        {
            if (target == null) EndAction(false);

            var pos = target.transform.position;
            if (lastRequest != pos)
            {
                if (!agent.SetDestination(pos))
                {
                    EndAction(false);
                    return;
                }
            }

            lastRequest = pos;

            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance + keepDistance.value)
            {
                EndAction(true);
            }
        }


        protected override void OnStop()
        {
            if (lastRequest != null && agent.gameObject.activeSelf)
            {
                agent.ResetPath();
            }
            lastRequest = null;
        }

        protected override void OnPause()
        {
            OnStop();
        }

        public override void OnDrawGizmosSelected()
        {
            if (target != null)
            {
                Gizmos.DrawWireSphere(target.transform.position, keepDistance.value);
            }
        }



    }
}





