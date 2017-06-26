using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpadateBars : MonoBehaviour
{
    public bool isHPbar = true;
    public float amount = 1f, total = 1f;
    private Image bar;
    private bool ready = false;

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
            total = 100f;
            amount = LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_yohoho;
        }
        ready = true;
    }



	void Update ()
    {
        if (ready)
        {
            if (isHPbar)
                amount = (float)LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_Health;
            else
            {
                amount = LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_yohoho;
                if(amount == 100 && !LocalGameManager.Instance.yohoho_icon)
                {
                    LocalGameManager.Instance.yohoho_icon = true;
                    GameObject yohoho = GameObject.FindGameObjectWithTag("YohohoTag");
                    //Da rendere un po più bello
                    yohoho.transform.GetChild(0).GetChild(0).GetComponent<Image>().color = new Color(yohoho.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.r, yohoho.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.g, yohoho.transform.GetChild(0).GetChild(0).GetComponent<Image>().color.b, 1f);
                    yohoho.transform.GetChild(0).GetChild(1).GetComponent<Image>().color = new Color(yohoho.transform.GetChild(0).GetChild(1).GetComponent<Image>().color.r, yohoho.transform.GetChild(0).GetChild(1).GetComponent<Image>().color.g, yohoho.transform.GetChild(0).GetChild(1).GetComponent<Image>().color.b, 154/255);
                    yohoho.transform.GetChild(0).GetChild(2).GetComponent<Text>().color = new Color(yohoho.transform.GetChild(0).GetChild(2).GetComponent<Text>().color.r, yohoho.transform.GetChild(0).GetChild(2).GetComponent<Text>().color.g, yohoho.transform.GetChild(0).GetChild(2).GetComponent<Text>().color.b, 1f);
                    yohoho.transform.GetChild(0).gameObject.SetActive(true);
                }
            }


            bar.fillAmount = 1f / total * amount;
        }
	}
}
