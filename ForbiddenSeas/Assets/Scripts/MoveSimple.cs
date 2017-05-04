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

	public override void OnStartLocalPlayer()
	{
		rb = GetComponent<Rigidbody>();
	}

	void FixedUpdate ()
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
		/*if (actualspeed<speed*factor*forward)
			actualspeed +=acceleration*time.deltatime;*/
		//Vector3 moveVelocity = new Vector3 (0, 0, speed * Factor * forward * -1); 
		/*if (rb.velocity.magnitude < speed * Factor ) {*/
		rb.MovePosition(rb.position +transform.forward* speed * Factor * forward * -1*Time.fixedDeltaTime);
		var desiredRotation=Quaternion.Euler(new Vector3(0f,transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed , Input.GetAxis ("Horizontal") * 10f * Factor));
		rb.MoveRotation (Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothTime));
		//rb.AddTorque (transform.up * Input.GetAxis ("Horizontal") * rotSpeed);
		//Debug.Log (rb.velocity.magnitude);
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
