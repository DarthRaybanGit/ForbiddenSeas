using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadingCanvasController : MonoBehaviour {


    public GameObject m_LoadingPane;
    public GameObject m_Wheel;



    // Use this for initialization
    void Start () {
        StartCoroutine(loadingGame());
	}


    IEnumerator loadingGame()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        m_Wheel.GetComponent<Animator>().SetTrigger("Hide");
        m_LoadingPane.GetComponent<Animator>().SetTrigger("Hide");

    }

}
