using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MoveSimple : NetworkBehaviour {

	public float maxSpeed;
	public float maneuvrability;
	public float rotSpeed;
	public Camera cam;
	public int State=0;
	public float ActualSpeed = 0f;
	private float Acceleration = 0.03f;
	private float deceleration = 0.1f;
	public float Factor=0;
	private float Scroll = 0;
	private float smoothTime=20f;
	private Rigidbody rb;
	public float numberOfScroll = 20f;
	public Animator animator;



    public float spinta;

	public override void OnStartLocalPlayer()
	{
		rb = GetComponent<Rigidbody>();
        StartCoroutine(waitForStat());
	}

    IEnumerator waitForStat()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        maxSpeed = GetComponent<FlagshipStatus>().m_maxSpeed;
        maneuvrability = GetComponent<FlagshipStatus>().m_Maneuvrability;
		animator = GetComponent<Animator>();
    }

	void FixedUpdate ()
	{
        if (!isLocalPlayer)
            return;
		if (Input.GetAxis ("Mouse ScrollWheel") > 0.0) {
			State = State < numberOfScroll ? State + 1 : State;
		}
		if (Input.GetAxis ("Mouse ScrollWheel") < 0.0) {
			State = State > 0 ? State - 1 : State;
		}
		if (State / numberOfScroll == SpeedLevel.STOP)
			Factor = SpeedLevel.STOP;
		else
		{
			if (State / numberOfScroll == SpeedLevel.FULL)
				Factor = SpeedLevel.FULL;
			else
			{
				if (State / numberOfScroll >= SpeedLevel.HALF)
					Factor = SpeedLevel.HALF;
				else
					Factor = SpeedLevel.SLOW;
			}
		}
		if (ActualSpeed < maxSpeed * State / numberOfScroll)
			ActualSpeed = Mathf.Lerp (ActualSpeed, maxSpeed * State / numberOfScroll, Acceleration);
		else
		{
			if (ActualSpeed > 0.9f)
				ActualSpeed = Mathf.Lerp (ActualSpeed, maxSpeed * State / numberOfScroll, deceleration);
			else
				ActualSpeed = 0;
		}
        //rb.MovePosition(rb.position + transform.forward* ActualSpeed  * -1*Time.fixedDeltaTime*0.1f);

        rb.AddForce(transform.forward * ActualSpeed * -1);

        rb.velocity = transform.forward * -1 * rb.velocity.magnitude;

        var desiredRotation=Quaternion.Euler(new Vector3(0f,transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed * maneuvrability , Input.GetAxis ("Horizontal") * 10f * Factor * -1));
		transform.rotation= Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothTime);
		rb.angularVelocity = Vector3.zero;
        if(animator)
            animator.SetFloat ("Speed", ActualSpeed / maxSpeed);
		//Debug.Log (State / numberOfScroll);
	}

}
