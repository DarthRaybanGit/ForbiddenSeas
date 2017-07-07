using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class SupportShipSensor : RAINAction
{
    public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {

        ai.WorkingMemory.SetItem("PlayerPos", ai.Body.gameObject.GetComponent<SupportShip>().m_Flagship.transform.position, typeof(Vector3));
        ai.WorkingMemory.SetItem("MyDestination", ai.Body.gameObject.GetComponent<SupportShip>().m_Destination.position, typeof(Vector3));
        ai.Senses.GetSensor("FlagShipView").MountPoint = ai.Body.gameObject.GetComponent<SupportShip>().m_Flagship.transform;
        ai.Senses.GetSensor("ChasingBoundaries").MountPoint = ai.Body.gameObject.GetComponent<SupportShip>().m_Flagship.transform;
        ai.WorkingMemory.SetItem("Speed", ai.Body.gameObject.GetComponent<SupportShip>().m_Flagship.GetComponent<FlagshipStatus>().m_maxSpeed, typeof(float));




        return ActionResult.SUCCESS;
    }

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}