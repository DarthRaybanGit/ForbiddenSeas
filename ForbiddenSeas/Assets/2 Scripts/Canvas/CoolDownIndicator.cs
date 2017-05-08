using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoolDownIndicator : MonoBehaviour
{
    Animation anim;

    void Start()
    {
        anim = GetComponent<Animation>();
    }

    public void OnCoolDown(float cd)
    {
        int cdswitch = (int)(cd * 10f);
        switch (cdswitch)
        {
            case (15):
                anim.Play("CoolDown1_5sec");
                break;
            case (25):
                anim.Play("CoolDown2_5sec");
            case (35):
                anim.Play("CoolDown3_5sec");
            case (45):
                anim.Play("CoolDown4_5sec");
                break;
            case (50):
                anim.Play("CoolDown5sec");
                break;
            default:
                return;
        }
    }
}
