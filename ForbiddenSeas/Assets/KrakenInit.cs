using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class KrakenInit : NetworkBehaviour {

	// Use this for initialization
	public bool wait=false;
	private Transform myTransform;
	private Vector3 lastPos;
	private Vector3 lastRot;
	private float threshold = 0.5f;

	public float rotThreshold;

	public float syncPosX;
	public float syncPosZ;
	public float syncRotY;

	public float lerpRate = 5;
	private float smoothTime=20f;

	void Start () {
		myTransform = GetComponent<Transform> ();
		lastPos = myTransform.position;
		lastRot = myTransform.rotation.eulerAngles;
		syncPosX = GetComponent<Transform>().position.x;
		syncPosZ = GetComponent<Transform>().position.z;
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


	void FixedUpdate () {

		if (!isServer)
			LerpPosition();
		else
			TransmitPosition ();
	
	}




	public void TransmitPosition()
	{
		//Debug.Log("###########"+Vector3.Distance(myTransform.position, lastPos));
		if (Vector3.Distance(myTransform.position, lastPos) > threshold || Vector3.Distance(transform.rotation.eulerAngles, lastRot) > rotThreshold)
		{
			Rpc_SetPosition(myTransform.position.x, myTransform.position.z, transform.rotation.eulerAngles.y);
			lastPos = myTransform.position;
			lastRot = transform.rotation.eulerAngles;
		}
	}

	[ClientRpc]
	public void Rpc_SetPosition(float x, float z, float rot)
	{
		syncPosX = x;
		syncPosZ = z;
		syncRotY = rot;
	}

	void LerpPosition()
	{
		//Debug.Log("Sono " + netId + " mi sto spostando in " + transform.position);
		transform.position = Vector3.Lerp(transform.position, new Vector3(syncPosX, 0, syncPosZ), lerpRate);
		transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, syncRotY, 0f)), smoothTime);
	}
	IEnumerator updateOnServer()
	{
		yield return new WaitForFixedUpdate();
		LerpPosition();
	}

}
