using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Compass : MonoBehaviour
{
    Transform treasure;
    Vector3 northDirection;
    bool start = false;

	void Start ()
    {
        StartCoroutine(StartSearchTreasure());
	}

    IEnumerator StartSearchTreasure()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered() && LocalGameManager.Instance.m_TreasureIsInGame);

        GameObject g;
        if (g = LocalGameManager.Instance.WhoAsTheTreasure())
        {
            treasure = g.transform;

        }
        else
        {
            treasure = LocalGameManager.Instance.m_Treasure.transform;
        }
        start = true;
    }

    void Update()
    {
        if (start)
        {
            SearchTreasure();
            UpdateCompass();
        }
    }

    void SearchTreasure()
    {
        GameObject g;
        if (g = LocalGameManager.Instance.WhoAsTheTreasure())
        {
            treasure = g.transform;
        }
        else
        {
            treasure = LocalGameManager.Instance.m_Treasure.transform;
        }
    }

    void UpdateCompass()
    {
        northDirection.z = LocalGameManager.Instance.m_LocalPlayer.transform.eulerAngles.y;
        Vector3 diff = LocalGameManager.Instance.m_LocalPlayer.transform.position - treasure.position;

        Quaternion direction = Quaternion.LookRotation(diff);

        direction.z = -direction.y;
        direction.y = 0f;
        direction.x = 0f;

        transform.localRotation = direction * Quaternion.Euler(northDirection);
    }
}
