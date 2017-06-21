using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChangeShip : MonoBehaviour
{

    public OutGameCanvasControl ogcc;

	void Update ()
    {
        if (ogcc.vai)
        {
            if (Input.GetKeyDown(KeyCode.LeftArrow))
            {
                ogcc.prevClassSelect();   
            }

            if (Input.GetKeyDown(KeyCode.RightArrow))
            {
                ogcc.nextClassSelect();
            }
        }
	}
}
