using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour {

	public GameObject player;
	public Vector3 offset;
	// Use this for initialization

	void Start () {
		offset = new Vector3 (0, 2.5f, 3.7f);
		transform.position = player.transform.position + offset;
	}
	
	// Update is called once per frame
	void Update () {
		transform.position = player.transform.position + offset;
	}
}
