using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LogoStart : MonoBehaviour {

    public GameObject m_ConnectButtons;

    public void UnHideButtons()
    {
        m_ConnectButtons.SetActive(true);
    }


}
