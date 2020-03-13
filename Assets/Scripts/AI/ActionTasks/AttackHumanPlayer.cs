using UnityEngine;
using NodeCanvas.Framework;
using ParadoxNotion.Design;

public class AttackHumanPlayer : ActionTask
{
    public BBParameter<float> power = 5000f;

    public BBParameter<float> stunDuration = 0.5f;
    public BBParameter<float> animationDelay = 0.5f;

    protected override void OnExecute()
    {
        GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();
        ghostEnemy.PlayAttackAnimation();

        return;
    }


    protected override void OnUpdate()
    {
        base.OnUpdate();

        if (elapsedTime >= animationDelay.value)
        {
            GhostEnemy ghostEnemy = agent.GetComponent<GhostEnemy>();
            //On vérifie s'il y a un humain dans la scène.
            if (ghostEnemy.HumanExistInScene())
            {
                Human h = ghostEnemy.HumanPlayerGameObject.GetComponent<Human>();

                float distance = Vector3.Distance(h.transform.position, agent.transform.position);
                if (distance < 2f)
                {
                    h.HandleAttack(agent.transform, power.value, stunDuration.value);
                    EndAction(true);
                }
                else
                {
                    EndAction(false);
                }
            }
            else
            {
                //Pas d'humain, on abandonne la Task et re-continue de parcourir l'arbre.
                Debug.Log("Cant Attack player! there's none!");
                EndAction(false);
            }
        }
    }
}




