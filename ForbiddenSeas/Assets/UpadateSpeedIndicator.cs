using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UpadateSpeedIndicator : MonoBehaviour
{

    public float amount = 1f, maxSpeed = 1f, actualSpeed = 1f;
    private Image bar;

    void Start()
    {
        bar = GetComponent<Image>();
        StartCoroutine(WaitforReady());
    }

    IEnumerator WaitforReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        actualSpeed = LocalGameManager.Instance.m_LocalPlayer.GetComponent<MoveSimple>().ActualSpeed;
        maxSpeed = LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>().m_maxSpeed;
    }

    void Update()
    {
        actualSpeed = LocalGameManager.Instance.m_LocalPlayer.GetComponent<MoveSimple>().ActualSpeed;

        amount = 135f / maxSpeed * actualSpeed;

        transform.rotation = Quaternion.Euler(amount * Vector3.forward);
    }
}
