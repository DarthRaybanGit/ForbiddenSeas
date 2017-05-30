using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AttacksIconSelector : MonoBehaviour
{
    public bool isMain = true;
    public Sprite[] iconsMain;
    public Sprite[] iconsSpec;

    void Start()
    {
        StartCoroutine(WaitforReady());
    }

    IEnumerator WaitforReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());

        if(isMain)
            GetComponent<Image>().sprite = iconsMain[(int)LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().shipClass];
        else
            GetComponent<Image>().sprite = iconsSpec[(int)LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().shipClass];
        
    }
}
