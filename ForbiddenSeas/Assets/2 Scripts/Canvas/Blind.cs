using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityStandardAssets.ImageEffects;

public class Blind : MonoBehaviour
{

    public void SetBlind(bool b)
    {
        GetComponent<Image>().enabled = b;
        GetComponent<Image>().color = b ? new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 214f / 255f) : new Color(GetComponent<Image>().color.r, GetComponent<Image>().color.g, GetComponent<Image>().color.b, 0f);
        Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = b;
    }
}
