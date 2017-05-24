using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpadateBars : MonoBehaviour
{
    public bool isHPbar = true;
    public float amount = 1f, total = 1f;
    private Image bar;

    void Start()
    {
        bar = GetComponent<Image>();
        StartCoroutine(WaitforReady());
    }

    IEnumerator WaitforReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        if (isHPbar)
        {
            amount = (float)LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_Health;
            total = amount;
        }
        else
        {
            total = 120f;
            amount = LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_yohoho;
        }
    }

	void Update ()
    {
        if (isHPbar)
            amount = (float)LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_Health;
        else

            amount = LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_yohoho;

        bar.fillAmount = 1f / total * amount;
	}
}
