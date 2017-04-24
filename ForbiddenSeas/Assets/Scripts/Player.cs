using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Player : NetworkBehaviour {

    public GameObject[] m_AdmiralList = new GameObject[4];

    private int m_Class = 0;

    public override void OnStartLocalPlayer()
    {
        m_Class = GameManager.Instance.getLocalClass();
        GameObject pl = GameObject.Instantiate(m_AdmiralList[m_Class]);
        pl.transform.SetParent(transform);
    }

    // Update is called once per frame
    void Update () {

	}
}
