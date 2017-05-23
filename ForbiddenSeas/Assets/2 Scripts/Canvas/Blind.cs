using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Blind : MonoBehaviour
{

    public void SetBlind(bool b)
    {
        GetComponent<Image>().enabled = b;
    }
}
