using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VikingBlazeForGameplayTrailer : MonoBehaviour {


    public float level = 0;
    public GameObject Particle;


	// Use this for initialization
	void Start () {


	}

	// Update is called once per frame
	void Update () {


        if (Input.GetKeyDown(KeyCode.Space))
        {
            GetComponent<Animator>().SetTrigger("MainAttack");
        }

        if (Input.GetKeyDown(KeyCode.W))
        {
            level += 0.1f;
            if (level > 1)
                level = 1;


        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            level -= 0.1f;
            if (level < 0)
                level = 0;
        }

        GetComponent<Animator>().SetFloat("Speed", level);
    }

    public void SparoMain()
    {
        Debug.Log("SparoMain");
        Particle.SetActive(false);
        Particle.SetActive(true);
    }
}
