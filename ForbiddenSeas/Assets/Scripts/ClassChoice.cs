using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ClassChoice : NetworkBehaviour {

    public void chosePlayerClass(int n)
    {
        if(n < 4)
            GameManager.Instance.setLocalClass(n);
    }

}
