using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatusHUD : MonoBehaviour
{
    void Update()
    {
        LocalGameManager.Instance.m_LocalPlayer.GetComponent<FlagshipStatus>();
    }
}
