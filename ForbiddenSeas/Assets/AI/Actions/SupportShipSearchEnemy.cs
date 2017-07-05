using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class SupportShipSearchEnemy : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
        ai.Sense();

        List<RAIN.Entities.Aspects.RAINAspect> enemiesInRange = new List<RAIN.Entities.Aspects.RAINAspect>();

        foreach (RAIN.Entities.Aspects.RAINAspect ra in ai.Senses.Match("FlagShipView", "Player"))
        {

            if(ra.Entity.Form.GetComponent<Player>().playerId != ai.Body.GetComponent<SupportShip>().m_Flagship.GetComponent<Player>().playerId)
            {
                enemiesInRange.Add(ra);
            }
            /*
            if (ra.Entity.Form.GetInstanceID() != ai.Body.GetComponent<SupportShip>().m_Flagship.GetInstanceID())
            {
                enemiesInRange.Add(ra);
            }
            */
        }

        float minDistance = 99999;

        GameObject target = null;

        foreach(RAIN.Entities.Aspects.RAINAspect raq in enemiesInRange)
        {
            if(Vector3.Distance(ai.Body.GetComponent<SupportShip>().m_Flagship.transform.position, raq.Position) < minDistance)
            {
                target = raq.Entity.Form;
            }
        }

        if(target != null)
            ai.WorkingMemory.SetItem("EnemyPosition", target.transform.position, typeof(Vector3));


        ai.WorkingMemory.SetItem("EnemySeen", target, typeof(GameObject));

        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}