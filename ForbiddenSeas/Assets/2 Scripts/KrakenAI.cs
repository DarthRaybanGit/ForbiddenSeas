using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KrakenAI : MonoBehaviour {

	public GameObject players;
	public Vector3 targetPos;
	public Quaternion DesRot;
	public float speed= 0.1f;
	public bool onMove=false;
	public bool onChase = false;
	private Rigidbody rb;
	public float SpeedFactor = 1.2f;
	public float ActualSpeed =0f;
	public float UnderwaterSpeed = 7f;
	public float AfloatSpeed = 4f;
	public float AttackRange = 15f;
	public float ChaseRange = 30f;
	public bool underwater = true;
	public float maxSpeed;
	public float Acceleration = 0.03f;
	public float deceleration = 0.5f;

	// Use this for initialization
	void Start () {
		//StartCoroutine (PlayerPos());
		targetPos=transform.position;
		DesRot = transform.rotation;
		rb = GetComponent<Rigidbody> ();
	}
		
	void FixedUpdate () {
		if (!onMove && !onChase)			//guarda alla posizione dove deve andare
		{
			//transform.LookAt(players.transform.position);
			//targetPos = players.transform.position;
			StartCoroutine (TargetPos ());
			onMove = true;
		}

		if (underwater)						//imposto la maxspeed in base allo stato attuale
			maxSpeed = UnderwaterSpeed;
		else
			maxSpeed = AfloatSpeed;
		
		if (ActualSpeed < maxSpeed )		//lo faccio muovere
			ActualSpeed = Mathf.Lerp (ActualSpeed, maxSpeed , Acceleration);
		else
		{
			if (ActualSpeed > maxSpeed)
				ActualSpeed = Mathf.Lerp (ActualSpeed, maxSpeed , deceleration);
			else
				ActualSpeed = 0;
		}

		rb.AddForce (transform.forward * ActualSpeed * SpeedFactor);
		rb.velocity = transform.forward * rb.velocity.magnitude;
		//DesRot = Quaternion.LookRotation (targetPos);
		//transform.rotation = Quaternion.Lerp (transform.rotation, DesRot, Time.deltaTime * 20f);
			//transform.position = Vector3.Lerp (transform.position, targetPos, speed * Time.deltaTime);

		Debug.Log (Vector3.Distance (transform.position, targetPos));
		if (Vector3.Distance (transform.position, targetPos) < 5f) 
		{
			onMove = false;
			maxSpeed = 0f;
		}
		rb.angularVelocity = Vector3.zero;
	}

	IEnumerator TargetPos()
	{
		yield return new WaitForSeconds (5f);
		transform.LookAt(players.transform.position);
		targetPos = players.transform.position;
		onMove = false;

	}

}
