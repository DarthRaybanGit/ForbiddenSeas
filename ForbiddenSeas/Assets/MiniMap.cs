using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MiniMap : MonoBehaviour
{
    public bool isTreasure = false;
    [Range(1,4)]
    public int player;
    private bool ready = false;
    private Vector3 normalizedPlayerPos;

    void Start()
    {
        StartCoroutine(WaitforReady());
    }

    IEnumerator WaitforReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());
        yield return new WaitForSeconds(0.2f);

        if (player > LocalGameManager.Instance.m_Players.Length)
            Destroy(gameObject);
        
        if (isTreasure)
        {
            yield return new WaitUntil(() => LocalGameManager.Instance.m_TreasureIsInGame);
            GetComponent<Image>().color = Color.yellow;
        }
        else
        {
            if (LocalGameManager.Instance.GetPlayerId(LocalGameManager.Instance.m_LocalPlayer) == player)
            {
                GetComponent<Image>().color = Color.green;
                transform.GetChild(0).GetComponent<Image>().color = Color.green;
            }
            else
            {
                GetComponent<Image>().color = Color.red;
                transform.GetChild(0).GetComponent<Image>().color = Color.red;
            }
        }
        ready = true;
    }


	void Update ()
    {
        if (ready)
        {
            Quaternion rot;
            if (isTreasure)
            {
                normalizedPlayerPos = (SearchTreasure().position) / 427.5f * 150f;
                rot = SearchTreasure().rotation;
            }
            else
            {
                normalizedPlayerPos = (LocalGameManager.Instance.GetPlayer(player).transform.position) / 427.5f * 150f;
                rot = LocalGameManager.Instance.GetPlayer(player).transform.rotation;
            }

            GetComponent<RectTransform>().anchoredPosition = new Vector2(normalizedPlayerPos.x, normalizedPlayerPos.z);
            rot.z = -rot.y;
            rot.y = 0f;
            rot.x = 0f;

            GetComponent<RectTransform>().rotation = rot;
        }
	}


    private Transform SearchTreasure()
    {
        GameObject g;
        if (g = LocalGameManager.Instance.WhoAsTheTreasure())
        {
            GetComponent<Image>().color = Color.magenta;
            return g.transform;
        }
        else
        {
            GetComponent<Image>().color = Color.yellow;
            return LocalGameManager.Instance.m_Treasure.transform;
        }
    }
}
