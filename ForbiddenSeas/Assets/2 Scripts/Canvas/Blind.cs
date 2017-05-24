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
        Camera.main.GetComponent<VignetteAndChromaticAberration>().enabled = b;
    }
}
