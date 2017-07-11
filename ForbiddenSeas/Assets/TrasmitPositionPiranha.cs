﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class TrasmitPositionPiranha : NetworkBehaviour
{

	// Use this for initialization
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

	void Start ()
    {
        myTransform = transform.GetChild(0);
		lastPos = myTransform.position;
		lastRot = myTransform.rotation.eulerAngles;
        syncPosX = transform.GetChild(0).position.x;
        syncPosZ = transform.GetChild(0).position.z;
	}

	void FixedUpdate ()
    {

        if (!isServer)
            LerpPosition();
        else
            TransmitPosition();
	}
        
	public void TransmitPosition()
	{
		//Debug.Log("###########"+Vector3.Distance(myTransform.position, lastPos));
        if (Vector3.Distance(myTransform.position, lastPos) > threshold || Vector3.Distance(transform.GetChild(0).rotation.eulerAngles, lastRot) > rotThreshold)
		{
            Rpc_SetPosition(myTransform.position.x, myTransform.position.z, transform.GetChild(0).rotation.eulerAngles.y);
			lastPos = myTransform.position;
            lastRot = transform.GetChild(0).rotation.eulerAngles;
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
		Debug.Log("Sono " + netId + " mi sto spostando in " + transform.position);
        transform.GetChild(0).position = Vector3.Lerp(transform.GetChild(0).position, new Vector3(syncPosX, 0, syncPosZ), lerpRate);
        transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.Euler(new Vector3(0f, syncRotY, 0f)), smoothTime);
	}
}