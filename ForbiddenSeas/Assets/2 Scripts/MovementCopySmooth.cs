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
    public bool gameStart = false;

    public Animator anim;

	void Start () {
		//StartCoroutine(waitEveryone());

	}


	public IEnumerator waitEveryone()
	{
		yield return new WaitUntil (() => LocalGameManager.Instance.GameCanStart());
        player = LocalGameManager.Instance.m_LocalPlayer;
		transform.position = player.transform.position;
		wait = true;

	}


	// Update is called once per frame
	void FixedUpdate () {
		if (!wait)
			return;
		Vector3 EulerAngles = transform.rotation.eulerAngles;
		if (transform.position==player.transform.position)
			EulerAngles.y = Mathf.SmoothDampAngle (EulerAngles.y, player.transform.eulerAngles.y /*+ Input.GetAxis("Horizontal") * Angle*/, ref velocity, smooth);
		else
			EulerAngles.y = Mathf.SmoothDampAngle (EulerAngles.y, player.transform.eulerAngles.y + Input.GetAxis("Horizontal") * Angle, ref velocity, smooth);
		transform.rotation = Quaternion.Euler (EulerAngles);
		transform.position = player.transform.position;

		if (Input.GetKey (KeyCode.S)) 
		{
			transform.GetChild (0).gameObject.GetComponent<Camera> ().enabled = false;
			transform.GetChild (0).gameObject.tag = "Untagged";
			player.GetComponent<Player> ().m_LocalCamera.SetActive(true);
			player.GetComponent<Player> ().m_LocalCamera.tag="MainCamera";
		}
		if (Input.GetKeyUp (KeyCode.S)) 
		{
			player.GetComponent<Player> ().m_LocalCamera.SetActive (false);
			player.GetComponent<Player> ().m_LocalCamera.tag = "Untagged";
			transform.GetChild (0).gameObject.GetComponent<Camera> ().enabled = true;
			transform.GetChild (0).gameObject.tag = "MainCamera";
		}


	}




}
