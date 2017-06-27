using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerMiniUpMap : MonoBehaviour
{
    [Range(0,2)]
    public int p_up;
    private bool ready = false;
    private Vector3 normalizedPowerPos;

    void Start()
    {
        StartCoroutine(WaitforReady());
    }

    IEnumerator WaitforReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        yield return new WaitForSeconds(0.2f);
        SetPos();
    }
        
    void SetPos ()
    {
        normalizedPowerPos = (OnlineManager.s_Singleton.m_PowerUpSpawnPosition[p_up].transform.position) / 427.5f * 150f;
        GetComponent<RectTransform>().anchoredPosition = new Vector2(normalizedPowerPos.x, normalizedPowerPos.z);
    }
}
