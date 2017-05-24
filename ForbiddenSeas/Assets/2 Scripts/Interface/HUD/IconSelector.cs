using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IconSelector : MonoBehaviour
{

    public Sprite[] icons;
	
    void Start()
    {
        StartCoroutine(WaitforReady());
    }

    IEnumerator WaitforReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        GetComponent<Image>().sprite = icons[(int)LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().shipClass];
    }
}
