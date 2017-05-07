using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour {

	// Use this for initialization
	void Start () {
        StartCoroutine(SearchTreasure());
	}

    IEnumerator SearchTreasure()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered() && LocalGameManager.Instance.m_TreasureIsInGame);

        //Da fare il nuovo while in cui aggiornare la posizione della bussola! NOT every frame.

        GameObject g;
        if (g = LocalGameManager.Instance.WhoAsTheTreasure())
        {
            Vector3 treasure = g.transform.position;

        }
        else
        {
            g = LocalGameManager.Instance.m_Treasure;
        }
    }

}
