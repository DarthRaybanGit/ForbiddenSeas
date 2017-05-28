using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerFloatingName : MonoBehaviour
{
    [Range(1,4)]
    public int id;
    public Transform target;
    public Vector2 offset;
    Vector2 targetPos;
    public float amount = 1f, total = 1f;
    Image bar;

    public bool trovato = false;

    void Start()
    {
        StartCoroutine(WaitForReady());
        bar = transform.GetChild(0).GetChild(0).GetComponent<Image>();
    }

    void Update ()
    {
        if(trovato && target)
        {
            if (LocalGameManager.Instance.GetPlayer(id).GetComponent<Player>().isThisPlayerLocal())
            {
                targetPos = Camera.main.WorldToScreenPoint(target.position);
                transform.position = targetPos + offset;
                transform.GetChild(1).GetComponent<Text>().text = "L.Player " + id; //da sostituire con player name
                transform.GetChild(2).GetComponent<Text>().text = "L.Player " + id;

            }
            else
            {
                targetPos = Camera.main.WorldToScreenPoint(target.position);
                targetPos += offset;
                if (targetPos.y > (Camera.main.pixelHeight * 0.85f))
                    targetPos = new Vector2(targetPos.x, 0.85f * Camera.main.pixelHeight);
                if (Camera.main.WorldToScreenPoint(target.position).z < 0)
                    targetPos = new Vector2(2f * Camera.main.pixelWidth, 2f * Camera.main.pixelHeight);
                transform.position = targetPos;
                transform.GetChild(1).GetComponent<Text>().text = "Player " + id; //da sostituire con player name
                transform.GetChild(2).GetComponent<Text>().text = "Player " + id;
            }
            FillBar();
            transform.GetChild(3).GetComponent<Text>().text = "" + target.gameObject.GetComponent<FlagshipStatus>().m_Health;

        }
    }

    IEnumerator WaitForReady()
    {
        yield return new WaitUntil(() => LocalGameManager.Instance.IsEveryPlayerRegistered());


        Debug.Log("Sto cercando i player!");

        if (id > LocalGameManager.Instance.m_Players.Length)
        {
            gameObject.SetActive(false);
        }
        else
        {
            target = LocalGameManager.Instance.GetPlayer(id).transform;
            Debug.Log("Ho trovato il player " + target.name + " " + target.GetComponent<Player>().netId);
            trovato = true;
            total = (float)target.GetComponent<FlagshipStatus>().m_Health;
        }
    }

    void FillBar()
    {
        amount = (float)target.gameObject.GetComponent<FlagshipStatus>().m_Health;
        if (amount <= total * 0.2f)
        {
            bar.color = new Color(1f, 54f/255f, 54f/255f);
        }
        else
        {
            bar.color = new Color(167f/255f, 251f/255f, 109f/255f);
        }

        bar.fillAmount = 1f / total * amount;

    }
}
