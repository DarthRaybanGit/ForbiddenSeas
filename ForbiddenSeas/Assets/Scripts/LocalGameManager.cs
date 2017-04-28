using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class LocalGameManager : MonoBehaviour {

    public static LocalGameManager Instance = null;

    public GameObject m_LocalPlayer;

    public GameObject[] m_LocalClassViewer;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        DontDestroyOnLoad(transform.gameObject);

    }

	// Update is called once per frame
	void Update () {

	}
}
