using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mina : NetworkBehaviour {

    public int m_Danno;

    private void OnCollisionEnter(Collision collision)
    {
        if (isServer)
        {
            if (collision.gameObject.GetComponent<Player>() && !collision.gameObject.GetComponent<FlagshipStatus>().m_isDead)
            {
                GetComponent<MeshRenderer>().enabled = false;
                transform.GetChild(0).gameObject.SetActive(true);
                GetComponent<Collider>().enabled = false;
                collision.gameObject.GetComponent<FlagshipStatus>().PrendiDannoDaEnemy(m_Danno);
                StartCoroutine(shutDownMe());

            }
        }
    }

    IEnumerator shutDownMe()
    {
        yield return new WaitForSeconds(2.5f);
        Destroy(gameObject);
    }
}
