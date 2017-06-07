using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class Mina : NetworkBehaviour {

    public int m_Danno;
    public int which;
    public float m_ExplosionStrength;
    public float m_ExplosionRadius;


    private void OnTriggerEnter(Collider other)
    {
        if (isServer)
        {
            if (other.gameObject.GetComponent<Player>() && !other.gameObject.GetComponent<FlagshipStatus>().m_isDead)
            {
                RpcExplode();
                TargetRpcScosta(other.gameObject.GetComponent<NetworkIdentity>().connectionToClient, other.gameObject.GetComponent<NetworkIdentity>().netId);
                other.gameObject.GetComponent<FlagshipStatus>().PrendiDannoDaEnemy(m_Danno);
                StartCoroutine(shutDownMe());

            }
        }
    }

    [TargetRpc]
    public void TargetRpcScosta(NetworkConnection conn, NetworkInstanceId who)
    {
        GameObject go = ClientScene.FindLocalObject(who);
        go.GetComponent<MoveSimple>().DontPush = true;
        go.GetComponent<MoveSimple>().ActualSpeed /= 2f;
        go.GetComponent<Rigidbody>().AddForce(m_ExplosionStrength * transform.forward);
        StartCoroutine(push(go));
    }

    IEnumerator push(GameObject go)
    {
        yield return new WaitForSeconds(1f);
        go.GetComponent<MoveSimple>().DontPush = false;
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
        yield return new WaitForSeconds(2.5f);
        LocalGameManager.Instance.StartCoroutine(LocalGameManager.Instance.c_LoopMines((int)FixedDelayInGame.MINE_SPAWN, which));
        Destroy(gameObject);
    }
}
