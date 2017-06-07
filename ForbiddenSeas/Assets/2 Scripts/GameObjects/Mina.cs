using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mina : NetworkBehaviour {

    public int m_Danno;
    public int which;


    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.gameObject.GetComponent<Player>() && !other.gameObject.GetComponent<FlagshipStatus>().m_isDead)
            {
                RpcExplode();
                other.gameObject.GetComponent<FlagshipStatus>().PrendiDannoDaEnemy(m_Danno);
                StartCoroutine(shutDownMe());

            }
        }
    }

    [ClientRpc]
    public void RpcExplode()
    {
        GetComponentInChildren<MeshRenderer>().enabled = false;
        transform.GetChild(0).gameObject.SetActive(true);
        GetComponentInChildren<Collider>().enabled = false;
    }

    IEnumerator shutDownMe()
    {
        yield return new WaitForSeconds(3f);
        LocalGameManager.Instance.StartCoroutine(LocalGameManager.Instance.c_LoopMines((int)FixedDelayInGame.MINE_SPAWN, which));
        Destroy(gameObject);
    }
}
