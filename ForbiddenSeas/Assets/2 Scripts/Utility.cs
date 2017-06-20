using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public static class Utility
{
    public static GameObject FindChildWithTag(this GameObject parent, string tag)
    {
        Transform t = parent.transform;
        foreach(Transform tr in t)
        {
            if(tr.tag == tag)
            {
                return tr.gameObject;
            }
        }
        return null;
    }


    public static void recursivePlayAnimation(Transform t, string animation, string optional = "")
    {
        foreach (Transform tt in t)
        {
            if (tt.gameObject.GetComponent<Text>() && tt.gameObject.GetComponent<Animation>())
            {
                if (!tt.gameObject.GetComponent<Animation>().GetClip("Text" + animation))
                    tt.gameObject.GetComponent<Animation>().Play();
                else
                    tt.gameObject.GetComponent<Animation>().Play("Text" + animation);
            }
            else if (tt.gameObject.GetComponent<Image>() && tt.gameObject.GetComponent<Animation>())
            {
                if (!tt.gameObject.GetComponent<Animation>().GetClip(animation))
                {
                    if (optional.Length > 1)
                    {
                        if (tt.gameObject.GetComponent<Animation>().GetClip(animation + optional))
                            tt.gameObject.GetComponent<Animation>().Play(animation + optional);
                    }
                    else
                        tt.gameObject.GetComponent<Animation>().Play();
                }
                else
                    tt.gameObject.GetComponent<Animation>().Play(animation);
            }


            if (tt.childCount > 0)
            {
                recursivePlayAnimation(tt, animation);
            }
        }
    }

    public static void recursiveSetAlphaChannel(Transform t)
    {
        foreach (Transform tt in t)
        {
            if (tt.gameObject.GetComponent<Text>())
            {
                tt.gameObject.GetComponent<Text>().color = new Color(tt.gameObject.GetComponent<Text>().color.r, tt.gameObject.GetComponent<Text>().color.g, tt.gameObject.GetComponent<Text>().color.b, 0);
            }
            else if (tt.gameObject.GetComponent<Image>())
            {
                tt.gameObject.GetComponent<Image>().color = new Color(tt.gameObject.GetComponent<Image>().color.r, tt.gameObject.GetComponent<Image>().color.g, tt.gameObject.GetComponent<Image>().color.b, 0);
            }


            if (tt.childCount > 0)
            {
                if(!tt.gameObject.tag.Equals("Stats"))
                    recursiveSetAlphaChannel(tt);
            }
        }
    }


}
