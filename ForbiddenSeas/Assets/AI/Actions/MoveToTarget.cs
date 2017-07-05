using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using RAIN.Action;
using RAIN.Core;

[RAINAction]
public class MoveToTarget : RAINAction
{
   /**/ public override void Start(RAIN.Core.AI ai)
    {
        base.Start(ai);
    }

    public override ActionResult Execute(RAIN.Core.AI ai)
    {
		if (!ai.Body.GetComponent<KrakenInit> ().wait) 
		{
			LocalGameManager.Instance.StartCoroutine (ai.Body.GetComponent<KrakenInit> ().TimeToWait ());
			Vector3 t = Vector3.zero;
			/*int count=0;
			foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) 
			{
				t = t + g.transform.position;
				count++;
			}
			t /=count;
			Debug.Log (t);

			GameObject[] p = GameObject.FindGameObjectsWithTag ("Player");*/
			int numcluster = 3;
			int q = 3;  //num di iterazioni
			//inizializzo 3 cluster
			List<GameObject> cluster1 = new List<GameObject> ();
			List<GameObject> cluster2 = new List<GameObject> ();
			List<GameObject> cluster3 = new List<GameObject> ();
			Vector3 cc1 = new Vector3 (0f, 0f, 60f);
			Vector3 cc2 = new Vector3 (-55f, 0f, -40f);
			Vector3 cc3 = new Vector3 (55f, 0f, -40f);
			//initialize
			foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
				if (Vector3.Distance (g.transform.position, cc1) < Vector3.Distance (g.transform.position, cc2) && Vector3.Distance (g.transform.position, cc1) < Vector3.Distance (g.transform.position, cc3))
					cluster1.Add (g);
				if (Vector3.Distance (g.transform.position, cc2) < Vector3.Distance (g.transform.position, cc1) && Vector3.Distance (g.transform.position, cc2) < Vector3.Distance (g.transform.position, cc3))
					cluster2.Add (g);
				else
					cluster3.Add (g);
			}
			cc1 = Vector3.zero;
			cc2 = Vector3.zero;
			cc3 = Vector3.zero;
			foreach (GameObject e in cluster1) {
				cc1 += e.transform.position / cluster1.Count;
			}
			foreach (GameObject e in cluster2) {
				cc2 += e.transform.position / cluster2.Count;
			}
			foreach (GameObject e in cluster3) {
				cc3 += e.transform.position / cluster3.Count;
			}

			for (int i = 1; i < q; i++) {
				List<GameObject> c1 = new List<GameObject> ();
				List<GameObject> c2 = new List<GameObject> ();
				List<GameObject> c3 = new List<GameObject> ();

				foreach (GameObject g in GameObject.FindGameObjectsWithTag("Player")) {
					if (Vector3.Distance (g.transform.position, cc1) < Vector3.Distance (g.transform.position, cc2) && Vector3.Distance (g.transform.position, cc1) < Vector3.Distance (g.transform.position, cc3))
						c1.Add (g);
					if (Vector3.Distance (g.transform.position, cc2) < Vector3.Distance (g.transform.position, cc1) && Vector3.Distance (g.transform.position, cc2) < Vector3.Distance (g.transform.position, cc3))
						c2.Add (g);
					else
						c3.Add (g);
				}
				cluster1 = c1;
				cluster2 = c2;
				cluster3 = c3;

				cc1 = Vector3.zero;
				cc2 = Vector3.zero;
				cc3 = Vector3.zero;
				foreach (GameObject e in cluster1) {
					cc1 += e.transform.position / cluster1.Count;
				}
				foreach (GameObject e in cluster2) {
					cc2 += e.transform.position / cluster2.Count;
				}
				foreach (GameObject e in cluster3) {
					cc3 += e.transform.position / cluster3.Count;
				}
			}

			if (cluster1.Count >= cluster2.Count && cluster1.Count >= cluster3.Count)
				t = cc1;
			if (cluster2.Count > cluster1.Count && cluster2.Count >= cluster3.Count)
				t = cc2;
			else
				t = cc3;
			Debug.Log ("centro1: " + cc1);
			Debug.Log ("centro2: " + cc2);
			Debug.Log ("centro3: " + cc3);
			Debug.Log ("scelto: " + t);
			ai.WorkingMemory.SetItem ("TargetPos", t);
		}
        return ActionResult.SUCCESS;
    }
		

    public override void Stop(RAIN.Core.AI ai)
    {
        base.Stop(ai);
    }
}