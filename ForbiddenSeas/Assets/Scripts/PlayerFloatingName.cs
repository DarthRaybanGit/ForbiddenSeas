using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFloatingName : MonoBehaviour
{
    [Range(1,4)]
    public int id;
    private Transform target;

    void Start()
    {
        target = OnlineManager.s_Singleton.GetPlayer(id).transform;
    }

    void Update ()
    {
        if(target != null)
        {
            Vector2 targetPos = Camera.main.WorldToScreenPoint(target.position);
            transform.position = targetPos;
            GetComponent<Text>().text = "Player " + id + " " + target.gameObject.GetComponent<FlagshipStatus>().m_Health;
        }

    }
}
