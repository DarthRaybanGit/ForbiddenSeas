using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scia : MonoBehaviour {

	public void ShutDown(int n)
    {
        transform.GetChild(n).gameObject.SetActive(false);
    }

    public void ShowUp(int n)
    {
        transform.GetChild(n).gameObject.SetActive(true);
    }
}
