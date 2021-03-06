﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

public class StatusHUD : MonoBehaviour
{
    public Sprite[] spriteDebuff;
    public Sprite[] spriteBuff;
    private Image[] neg,pos;

    void Start()
    {
        neg = new Image[2];
        pos = new Image[4];

        neg[0] = transform.GetChild(0).GetComponent<Image>();
        neg[1] = transform.GetChild(1).GetComponent<Image>();

        pos[0] = transform.GetChild(2).GetComponent<Image>();
        pos[1] = transform.GetChild(3).GetComponent<Image>();
        pos[2] = transform.GetChild(4).GetComponent<Image>();
        pos[3] = transform.GetChild(5).GetComponent<Image>();



    }

    public IEnumerator ActivateDebuff(int debuff, float sec, bool check)
    {
        yield return new WaitWhile(() => check.Equals(LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().debuffList[debuff]));
        int max = FlagshipStatus.maxNumberStatus(LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().debuffList);
        neg[max-1].gameObject.SetActive(true);
        neg[max-1].sprite = spriteDebuff[debuff];

        while (sec > 0)
        {
            sec--;
            yield return new WaitForSeconds(1f);
			if (check.Equals (LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus> ().debuffList [debuff]))
				break;
            neg[max - 1].GetComponentInChildren<Text>().text = ((int)sec).ToString();
        }
        neg[max -1].gameObject.SetActive(false);
    }

    public IEnumerator ActivateBuff(int buff, float sec, bool check)
    {
        Debug.Log("activate buff");
        yield return new WaitWhile(() => check.Equals(LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().buffList[buff]));
        int max = FlagshipStatus.maxNumberStatus(LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().buffList);
        pos[max - 1].gameObject.SetActive(true);
        pos[max - 1].sprite = spriteBuff[buff];

        while (sec > 0)
        {
            sec--;
            yield return new WaitForSeconds(1f);
			if (check.Equals (LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus> ().buffList [buff]))
				break;
            pos[max - 1].GetComponentInChildren<Text>().text = ((int)sec).ToString();
        }
        pos[max-1].gameObject.SetActive(false);
    }
}
