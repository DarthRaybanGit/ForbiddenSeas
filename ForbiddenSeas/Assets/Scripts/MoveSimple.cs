using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MoveSimple : NetworkBehaviour {

	public float speed;
	public float rotSpeed;
	public Camera cam;
	private int State=0;
	private float ActualSpeed = 0f;
	public float Acceleration = 10f;
	public float Factor=0;
	private float Scroll = 0;
	private float forward=1;
	private float Velocity=0f;
	private float smoothTime=20f;
	private Rigidbody rb;
	private float Vm;
	//public GameObject Sea;
	//private Transform SeaPlane;
	//private Cloth planeCloth;
	private int index=-1;

	public override void OnStartLocalPlayer()
	{
		rb = GetComponent<Rigidbody>();
	}

	void Update ()
	{
		if (!isLocalPlayer)
		{
			Destroy(cam.GetComponent<AudioListener>());
			return;
		}
		if (Scroll == 0) {
			if (Input.GetAxis ("Mouse ScrollWheel") > 0.0) {
				State = State < 3 ? State + 1 : State;
				/*if ((State)<3f)
					State+=1f;*/
			}
			if (Input.GetAxis ("Mouse ScrollWheel") < 0.0) {
				State = State > -1 ? State - 1 : State;
				/*if((State)>0f)
					State-=1f;*/
			}
			if (State < 0)
				forward = -1;		
			else
				forward = 1;
			Scroll = Input.GetAxis ("Mouse ScrollWheel");
		} else {
			Scroll = Input.GetAxis ("Mouse ScrollWheel");
		}
		switch(State)
		{
		case 0:
			Factor = SpeedLevel.STOP;
			break;
		case 1:
			Factor = SpeedLevel.SLOW;
			break;
		case 2:
			Factor = SpeedLevel.HALF;
			break;
		case 3:
			Factor = SpeedLevel.FULL;
			break;
		case -1:
			Factor = SpeedLevel.SLOW;
			break;
		}
		Vm = rb.velocity.magnitude;
		Debug.Log (Vm);
		if (rb.velocity.magnitude < speed * Factor ) {
			rb.AddForce (transform.forward * Acceleration * forward * -1f);
		}
		//float newRotation = Mathf.SmoothDamp (transform.rotation.z, Input.GetAxis ("Horizontal") * 10f * Factor, ref Velocity, smoothTime);
		//Debug.Log (newRotation);
		//Debug.Log (Velocity);
		var desiredRotation=Quaternion.Euler(new Vector3(0f,transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed , Input.GetAxis ("Horizontal") * 10f * Factor));
		transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothTime);
		//GetClosestVertex ();
	}

	/*void GetClosestVertex()
	{
		for (int i = 0; i < planeCloth.vertices.Length; i++) {
			if (index == -1)
				index = i;
			float Distance=Vector3.Distance(planeCloth.vertices[i],transform.position);
			float closestDistance = Vector3.Distance (planeCloth.vertices [index],transform.position);
			if (Distance < closestDistance)
				index = i;
		}
		transform.localPosition = new Vector3 (transform.localPosition.x, planeCloth.vertices [index].y, transform.localPosition.z);
	}*/

}
