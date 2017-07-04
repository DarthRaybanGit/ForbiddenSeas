using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;
using System;

[RAINAction]
public class KrakenDamage : RAINAction
{
	private bool wait=false;

    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		//prendidannodaenemy
		IList<RAIN.Entities.Aspects.RAINAspect> lista = ai.Senses.Match("AttackRange","Player");
		foreach (RAIN.Entities.Aspects.RAINAspect r in lista) 
		{
			LocalGameManager.Instance.StartCoroutine (DoDamage (r.Entity.Form,ai));
		}

        return ActionResult.SUCCESS;
    }

	IEnumerator DoDamage(GameObject g, RAIN.Core.AI ai)
	{		
		if (!ai.Body.GetComponent<KrakenInit>().wait) 
		{
			g.GetComponent<FlagshipStatus> ().PrendiDannoDaEnemy (20);
			ai.Body.GetComponent<KrakenInit>().wait = true;
			yield return new WaitForSeconds (1f);
			ai.Body.GetComponent<KrakenInit>().wait = false;
		}

	}


    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}