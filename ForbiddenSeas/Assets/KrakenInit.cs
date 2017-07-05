using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenInit : MonoBehaviour {

	// Use this for initialization
	public bool wait=false;
	void Start () {
		
	}

	public IEnumerator TimeToWait()
	{
		if (!wait) 
		{
			wait = true;
			yield return new WaitForSeconds (2f);
			wait = false;
		}
	}
	// Update is called once per frame
	void Update () {
		
	}
}
