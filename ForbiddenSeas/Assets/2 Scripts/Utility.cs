using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
}
