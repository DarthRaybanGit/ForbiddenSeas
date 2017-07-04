using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class MoveToTarget : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		Vector3 t = Vector3.zero;
		int count=0;
		foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) 
		{
			t = t + g.transform.position;
			count++;
		}
		t /=count;
		Debug.Log (t);
		ai.WorkingMemory.SetItem ("TargetPos", t);
        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}