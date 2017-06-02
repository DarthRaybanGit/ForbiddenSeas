using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenAI : MonoBehaviour {

	public GameObject players;
	public Vector3 targetPos;
	public Quaternion DesRot;
	public float speed= 0.1f;

	// Use this for initialization
	void Start () {
		//StartCoroutine (PlayerPos());
		targetPos=transform.position;
		DesRot = transform.rotation;
	}

	/*IEnumerator PlayerPos()
	{
		yield return new WaitUntil (() => LocalGameManager.Instance.IsEveryPlayerRegistered ());
		players = LocalGameManager.Instance.m_Players;
	}*/
	// Update is called once per frame
	void FixedUpdate () {
		StartCoroutine (TargetPos ());
		DesRot=Quaternion.LookRotation(targetPos);
		transform.rotation=Quaternion.Lerp(transform.rotation,DesRot,Time.deltaTime*20f);

		transform.position = Vector3.Lerp (transform.position, targetPos, speed * Time.deltaTime);
	}

	IEnumerator TargetPos()
	{
		if (Vector3.Distance (transform.position, targetPos) < 5) 
		{
			targetPos = players.transform.position;
			Debug.Log ("i'm in"+targetPos);
		}
		yield return new WaitForSeconds(10f);
	}

}
