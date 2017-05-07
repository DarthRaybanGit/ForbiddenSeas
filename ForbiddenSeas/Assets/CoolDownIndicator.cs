using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownIndicator : MonoBehaviour
{

    public void OnCoolDown(float cd)
    {
        StartCoroutine(StartCoolDown(cd));
    }

    IEnumerator StartCoolDown(float cd)
    {
        GetComponent<Image>().fillAmount = 1f;
        float amount = 1f;
        float decrease = cd * 0.1f;
        while (GetComponent<Image>().fillAmount > 0f)
        {
            yield return new WaitForSeconds(0.1f);
            amount -= decrease;
            Debug.Log("Amount: "+amount);
            GetComponent<Image>().fillAmount = amount;
        }
    }
}
