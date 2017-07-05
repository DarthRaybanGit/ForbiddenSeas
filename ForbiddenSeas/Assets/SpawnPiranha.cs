using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class SpawnPiranha : NetworkBehaviour
{
    public GameObject piranhaNW;

	void Start ()
    {
        if(isServer)
        {
            GameObject nw = GameObject.Instantiate(OnlineManager.s_Singleton.spawnPrefabs.ToArray()[(int)SpawnIndex.PIRANHA_NW], transform);
            nw.transform.localPosition = new Vector3(-6f, 0f, -43f);
            NetworkServer.Spawn(nw);
        }
	}
}
