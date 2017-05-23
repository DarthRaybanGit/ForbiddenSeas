using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour {

    public PowerUP type;
    public Vector3 m_rot = new Vector3(15, 30, 45);

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(m_rot * Time.deltaTime, Space.World);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato un powerUp da " + other.gameObject.GetComponent<Player>().netId);
                other.gameObject.GetComponent<Player>().CatchTheTreasure();
            }
        }
    }
}
