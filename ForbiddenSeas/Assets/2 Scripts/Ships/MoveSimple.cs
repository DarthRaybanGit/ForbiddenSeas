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
	//private float Scroll = 0;
	private float smoothTime=20f;
	private Rigidbody rb;
	public float numberOfScroll = 20f;
	public Animator animator;


    private Transform myTransform;
    [SerializeField]
    public float lerpRate = 5;

    public float syncPosX;
    public float syncPosZ;


    private Vector3 lastPos;
    public float threshold = 0.5f;




    public float syncRotY;
    private Vector3 lastRot;
    public float rotThreshold;
    [Range(1,5)]
    public float RotSpeedFactor;

    public bool DontPush = false;

    public void Start()
    {
        syncPosX = GetComponent<Transform>().position.x;
        syncPosZ = GetComponent<Transform>().position.z;
    }

    public override void OnStartLocalPlayer()
	{
		rb = GetComponent<Rigidbody>();
        myTransform = GetComponent<Transform>();

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
        {
            LerpPosition();
            return;
        }

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

        if (!GetComponent<FlagshipStatus>().m_isDead && LocalGameManager.Instance.GameCanStart() && !DontPush)
        {
            rb.AddForce(transform.forward * ActualSpeed * -1);

            rb.velocity = transform.forward * -1 * rb.velocity.magnitude;



            /*
            var desiredRotation=Quaternion.Euler(new Vector3(0f,transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed * maneuvrability , Input.GetAxis ("Horizontal") * 10f * Factor * -1));
            transform.rotation= Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothTime);
            */


            var desiredRotation = Quaternion.Euler(new Vector3(0f, transform.rotation.eulerAngles.y + Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed * maneuvrability / ((Mathf.Clamp(ActualSpeed, maxSpeed/RotSpeedFactor, maxSpeed)/maxSpeed) ), 0f));
            transform.rotation = Quaternion.Lerp(transform.rotation, desiredRotation, Time.deltaTime * smoothTime);

            transform.GetChild(0).rotation = Quaternion.Lerp(transform.GetChild(0).rotation, Quaternion.Euler(new Vector3(transform.GetChild(0).rotation.eulerAngles.x, transform.GetChild(0).rotation.eulerAngles.y, Input.GetAxis("Horizontal") * 10f * Factor)), Time.deltaTime * smoothTime);

        }

        rb.angularVelocity = Vector3.zero;
        if(animator)
            animator.SetFloat ("Speed", ActualSpeed / maxSpeed);
        //Debug.Log (State / numberOfScroll);

        TransmitPosition();

    }



    void LerpPosition()
    {
        //Debug.Log("Sono " + netId + " mi sto spostando in " + transform.position);
        transform.position = Vector3.Lerp(transform.position, new Vector3(syncPosX, 0, syncPosZ), Time.fixedDeltaTime * lerpRate);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(new Vector3(0f, syncRotY, 0f)), Time.deltaTime * smoothTime);
    }

    [Command]
    void Cmd_ProvidePositionToServer(float x, float z, float rotation)
    {
        //Debug.Log("####### " + x + " ###### " + z );
        syncPosX = x;
        syncPosZ = z;
        syncRotY = rotation;
        Rpc_SetPosition(x, z, rotation);


    }

    IEnumerator updateOnServer()
    {
        yield return new WaitForFixedUpdate();
        LerpPosition();
    }

    [ClientRpc]
    public void Rpc_SetPosition(float x, float z, float rot)
    {
        syncPosX = x;
        syncPosZ = z;
        syncRotY = rot;
    }

    void TransmitPosition()
    {
        //Debug.Log("###########"+Vector3.Distance(myTransform.position, lastPos));
        if (isLocalPlayer && (Vector3.Distance(myTransform.position, lastPos) > threshold || Vector3.Distance(transform.rotation.eulerAngles, lastRot) > rotThreshold))
        {
            Cmd_ProvidePositionToServer(myTransform.position.x, myTransform.position.z, transform.rotation.eulerAngles.y);
            lastPos = myTransform.position;
            lastRot = transform.rotation.eulerAngles;
        }
    }

}
