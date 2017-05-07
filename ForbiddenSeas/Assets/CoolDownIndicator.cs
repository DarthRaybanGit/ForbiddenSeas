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
        int i = 0;
        float amount = 1f;
        float decrease = 0.02f / cd;
        while (amount > 0f)
        {
            yield return new WaitForSeconds(0.02f);
            amount -= decrease;
            Debug.Log("Decrease: "+decrease + "i: "+ (i++));
            GetComponent<Image>().fillAmount = amount;
        }
    }
}
