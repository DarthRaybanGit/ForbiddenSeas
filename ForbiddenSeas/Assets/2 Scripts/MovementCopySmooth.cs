using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementCopySmooth : MonoBehaviour {

	// Use this for initialization
	public GameObject player;
	private float velocity=0f;
	public float smooth=0.1f;
	public float Angle = 15f;
	public bool wait = false;

	void Start () {
		StartCoroutine(waitEveryone());

	}

	IEnumerator waitEveryone()
	{
		yield return new WaitUntil (() => LocalGameManager.Instance.IsEveryPlayerRegistered ());
		player = LocalGameManager.Instance.m_LocalPlayer;
		transform.position = player.transform.position;
		wait = true;
	}

	
	// Update is called once per frame
	void FixedUpdate () {
		if (!wait)
			return;
		transform.position = player.transform.position;
		Vector3 EulerAngles = transform.rotation.eulerAngles;
		EulerAngles.y = Mathf.SmoothDampAngle (EulerAngles.y, player.transform.eulerAngles.y + Input.GetAxis("Horizontal") * Angle, ref velocity, smooth);
		transform.rotation = Quaternion.Euler (EulerAngles);
	}


}
