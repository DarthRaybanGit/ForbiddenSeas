using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToPlayer : MonoBehaviour {

    public void searchPlayer()
    {
        if(!LocalGameManager.Instance.isServer)
            StartCoroutine(GetComponentInParent<MovementCopySmooth>().waitEveryone());
    }
}
