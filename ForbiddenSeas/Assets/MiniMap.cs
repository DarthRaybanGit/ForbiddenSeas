﻿using System.Collections;
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

        if (LocalGameManager.Instance.m_LocalPlayer.GetComponent<Player>().playerId == player)
            GetComponent<Image>().color = Color.green;
        else
            GetComponent<Image>().color = Color.red;

        if (isTreasure)
        {
            yield return new WaitUntil(() => LocalGameManager.Instance.m_TreasureIsInGame);
            GetComponent<Image>().color = Color.yellow;
        }
        ready = true;
    }


	void Update ()
    {
        if (ready)
        {
            if (isTreasure)   
                normalizedPlayerPos = (SearchTreasure().position) / 427.5f * 150f;
            else
                normalizedPlayerPos = (LocalGameManager.Instance.GetPlayer(player).transform.position) / 427.5f * 150f;

            GetComponent<RectTransform>().anchoredPosition = new Vector2(normalizedPlayerPos.x, normalizedPlayerPos.z);
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
