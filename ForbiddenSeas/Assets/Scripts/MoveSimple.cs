using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveSimple : MonoBehaviour {

    public float speed, rotSpeed;

	void Update ()
    {
        Vector3 move = Camera.main.transform.forward * Input.GetAxis("Vertical") * Time.deltaTime * speed;
        transform.position += new Vector3(move.x, 0f, move.z);
        transform.Rotate(new Vector3(0f, Input.GetAxis("Horizontal") * Time.deltaTime * rotSpeed, 0f ));
	}
}
