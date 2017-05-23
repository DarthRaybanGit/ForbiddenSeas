using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class PowerUp : NetworkBehaviour {

    public PowerUP type;
    public Vector3 m_rot = new Vector3(15, 30, 45);

    public NetworkInstanceId playerInside;

    [SyncVar]
    public float m_health = 5000;

    // Update is called once per frame
    void LateUpdate()
    {
        transform.Rotate(m_rot * Time.deltaTime, Space.World);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (type == PowerUP.REGEN || type == PowerUP.SPEED_UP)
            {
                if (other.tag.Equals("mainAttack") || other.tag.Equals("specialAttack"))
                {
                    Debug.Log(gameObject.name + "Preso danno da " + other.name + other.GetComponentInParent<FlagshipStatus>().m_main);
                    int dmg = 0;
                    if (other.tag.Equals("mainAttack"))
                        dmg = other.GetComponentInParent<FlagshipStatus>().m_main;
                    else
                        dmg = other.GetComponentInParent<FlagshipStatus>().m_special;

                    Debug.Log("################## " + other.transform.parent.gameObject);
                    m_health -= dmg;

                    if(m_health <= 0)
                    {
                        Debug.Log("Toccato un powerUp da " + other.gameObject.GetComponent<Player>().netId);
                        other.gameObject.GetComponent<Player>().CatchAPowerUp(type);
                        LocalGameManager.Instance.m_PowerUp[(int)type] = false;
                        killMe();
                    }
                }
            }
            else if(type == PowerUP.DAMAGE_UP && other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<FlagshipStatus>().m_Health > 0)
            {
                Debug.Log("Toccato un powerUp da " + other.gameObject.GetComponent<Player>().netId);
                powerUpFill(other.gameObject.GetComponent<NetworkIdentity>().netId);
            }
        }
    }

    public void OnTriggerStay(Collider other)
    {

    }

    IEnumerator powerUpFill(NetworkInstanceId who)
    {
        yield return null;

    }

    private void killMe()
    {
        Destroy(gameObject);
    }
}
