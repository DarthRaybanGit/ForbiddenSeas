using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class cameraMovement : MonoBehaviour {

	public GameObject player;
	public Vector3 offset;
	public float smoothTime=20f;
	public float Angle = 5f;
	// Use this for initialization

	void Start () {
		offset = new Vector3 (4f, 2.5f, 4f);
		transform.position = player.transform.position + offset;
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		//transform.LookAt (player.transform, Vector3.up);
		transform.position = player.transform.position + new Vector3(player.transform.forward.x * offset.x + player.transform.right.x* Input.GetAxis("Horizontal") * Angle , offset.y ,player.transform.forward.z * offset.z + player.transform.right.z * Input.GetAxis("Horizontal") * Angle) ;
		//Debug.Log (player.transform.forward);
		//transform.rotation = Quaternion.Euler(new Vector3(20,player.transform.rotation.y,0));
		var desiredRotation=Quaternion.Euler(new Vector3(20,player.transform.rotation.eulerAngles.y - 180 /*+ Input.GetAxis("Horizontal") * Angle*/, 0));
		transform.rotation= Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothTime);
	}
}
