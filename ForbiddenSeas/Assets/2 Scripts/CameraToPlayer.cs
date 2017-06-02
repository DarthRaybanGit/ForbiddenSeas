using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraToPlayer : MonoBehaviour {

    public GameObject m_CanvasInGame;
    public GameObject m_CanvasEtichette;



    public void Start()
    {
        LocalGameManager.Instance.m_CanvasHUD = m_CanvasInGame;
        LocalGameManager.Instance.m_CanvasEtichette = m_CanvasEtichette;
        StartCoroutine(waitForPlaying());
    }

    IEnumerator waitForPlaying()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.m_LoadingCompleted);
        GetComponent<Animator>().SetTrigger("Start");
    }

    public void searchPlayer()
    {
        if (!LocalGameManager.Instance.isServer)
        {
            GetComponent<Animator>().SetTrigger("ToShip");
            GameObject player = LocalGameManager.Instance.m_LocalPlayer;
            transform.position = player.transform.position;

            StartCoroutine(GetComponentInParent<MovementCopySmooth>().waitEveryone());
        }

    }
}
