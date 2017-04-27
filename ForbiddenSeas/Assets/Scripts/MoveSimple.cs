using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class MoveSimple : NetworkBehaviour {

    public float speed;
    public float rotSpeed;

	void Update ()
    {
        if (!isLocalPlayer)
            return;
        
        Vector3 move = Camera.main.transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.position += new Vector3(move.x, 0f, move.z);
        transform.Rotate(new Vector3(0f, Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed, 0f ));
	}
}
