using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvasController : MonoBehaviour {


    public GameObject m_LoadingPane;
    public GameObject m_Wheel;
    public GameObject m_CanvasHUD;
    public GameObject m_CanvasEtichette;



    // Use this for initialization
    void Start () {
        Utility.recursiveSetAlphaChannel(m_CanvasHUD.transform);
        Utility.recursiveSetAlphaChannel(m_CanvasEtichette.transform);
        StartCoroutine(loadingGame());
	}


    IEnumerator loadingGame()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        m_Wheel.GetComponent<Animator>().SetTrigger("Hide");
        m_LoadingPane.GetComponent<Animator>().SetTrigger("Hide");

    }

}
