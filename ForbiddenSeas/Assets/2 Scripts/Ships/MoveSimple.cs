using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MoveSimple : NetworkBehaviour {

	public float maxSpeed;
	public float rotSpeed;
	public Camera cam;
	private int State=0;
	//private float ActualSpeed = 0f;
	public float Acceleration = 10f;
	public float Factor=0;
	private float Scroll = 0;
	//private float forward=1;
	//private float Velocity=0f;
	private float smoothTime=20f;
	private Rigidbody rb;

	/* float waterLevel = 0f;
	public float floatHeight = 2f;
	public float bounceDamp = 0.05f;
	public Vector3 bouyancyCentreOffset;

	private float forceFactor;
	private Vector3 actionPoint;
	private Vector3 upLift;*/

	public override void OnStartLocalPlayer()
	{
		rb = GetComponent<Rigidbody>();
		maxSpeed=GetComponent<FlagshipStatus>().m_maxSpeed;
	}

	void FixedUpdate ()
	{
        if (!isLocalPlayer)
            return;
		if (Scroll == 0) {
			if (Input.GetAxis ("Mouse ScrollWheel") > 0.0) {
				State = State < 3 ? State + 1 : State;
			}
			if (Input.GetAxis ("Mouse ScrollWheel") < 0.0) {
				State = State > 0 ? State - 1 : State;
			}
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
		}
		/*if (actualspeed<speed*factor*forward)
			actualspeed +=acceleration*time.deltatime;*/
		//Vector3 moveVelocity = new Vector3 (0, 0, speed * Factor * forward * -1);
		/*if (rb.velocity.magnitude < speed * Factor ) {*/
		if (rb.velocity.magnitude<maxSpeed*Factor)
			rb.AddForce (transform.forward * maxSpeed * Factor  * -1 * Time.fixedDeltaTime * 0.1f);
		rb.MovePosition(rb.position + transform.forward* maxSpeed * Factor  * -1*Time.fixedDeltaTime*0.1f);
		var desiredRotation=Quaternion.Euler(new Vector3(0f,transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed , Input.GetAxis ("Horizontal") * 10f * Factor * -1));
		transform.rotation= Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothTime);
		rb.angularVelocity = Vector3.zero;
		//rb.AddTorque (transform.up * Input.GetAxis ("Horizontal") * rotSpeed);
		//Debug.Log (rb.velocity.magnitude);
		//GetClosestVertex ();
		/*actionPoint = transform.position + transform.TransformDirection(bouyancyCentreOffset);
		forceFactor = 1f - ((actionPoint.y - waterLevel) / floatHeight);
		if (forceFactor > 0f) 
		{
			upLift = -Physics.gravity * (forceFactor - rb.velocity.y * bounceDamp);
			Debug.Log (upLift);
			rb.AddForceAtPosition (upLift, actionPoint); 
		}*/
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
