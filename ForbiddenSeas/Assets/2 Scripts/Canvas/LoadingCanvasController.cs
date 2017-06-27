using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingCanvasController : MonoBehaviour {


    public GameObject m_LoadingPane;
    public GameObject m_Wheel;
    public GameObject m_CanvasHUD;
    public GameObject m_CanvasEtichette;


    private void Awake()
    {
        recursiveActiveImage(transform);
    }

    public void recursiveActiveImage(Transform t)
    {
        foreach (Transform tt in t)
        {
            if (tt.gameObject.GetComponent<Image>())
                tt.gameObject.GetComponent<Image>().enabled = true;

            if (tt.childCount > 0)
            {
                recursiveActiveImage(tt);
            }
        }
    }


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
		Camera.main.gameObject.GetComponent<AudioSource> ().Play();

    }

}
