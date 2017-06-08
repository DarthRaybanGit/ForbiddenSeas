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
            GetComponent<Image>().color = new Color(1f, 0.92f, 0.016f, GetComponent<Image>().color.a);
        }
        else
        {
            if (LocalGameManager.Instance.GetPlayerId(LocalGameManager.Instance.m_LocalPlayer) == player)
            {
                GetComponent<Image>().color = new Color(0,1,0, GetComponent<Image>().color.a);
                transform.GetChild(0).GetComponent<Image>().color = new Color(0, 1, 0, transform.GetChild(0).GetComponent<Image>().color.a);
            }
            else
            {
                GetComponent<Image>().color = new Color(1, 0, 0, GetComponent<Image>().color.a);
                transform.GetChild(0).GetComponent<Image>().color = new Color(1, 0, 0, transform.GetChild(0).GetComponent<Image>().color.a);
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
            GetComponent<Image>().color = new Color(1f, 0f, 1f, GetComponent<Image>().color.a);
            return g.transform;
        }
        else
        {
            GetComponent<Image>().color = new Color(1f, 0.92f, 0.016f, GetComponent<Image>().color.a);
            return LocalGameManager.Instance.m_Treasure ? LocalGameManager.Instance.m_Treasure.transform : LocalGameManager.Instance.transform;
        }
    }
}
