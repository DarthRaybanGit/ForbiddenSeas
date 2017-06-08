using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PowerUpFloatingBar : MonoBehaviour
{
    public bool isSpeedUp = false;
    public float distance = 30f;
    public Transform target;
    public Vector2 offset;
    Vector2 targetPos;
    public float amount = 1f, total = 1f;
    Image bar;

    void Start()
    {
        if (!LocalGameManager.Instance.isServer)
        {
            target = transform.parent;
            transform.SetParent(GameObject.Find("Etichette").transform);
            bar = transform.GetChild(0).GetChild(0).GetChild(0).GetComponent<Image>();
            total = target.gameObject.GetComponent<PowerUp>().m_maxHealth;
        }
    }

    void Update ()
    {
        if (LocalGameManager.Instance.isServer)
            return;

        if (!target)
            Destroy(gameObject);

        bool isClose = Vector3.Distance(LocalGameManager.Instance.m_LocalPlayer.transform.position, target.position) < distance;
        transform.GetChild(0).gameObject.SetActive(isClose);
        if (!isClose)
            return;
        targetPos = Camera.main.WorldToScreenPoint(target.position);
        targetPos += offset;

        if (targetPos.y > (Camera.main.pixelHeight * 0.85f))
            targetPos = new Vector2(targetPos.x, 0.85f * Camera.main.pixelHeight);

        if (Camera.main.WorldToScreenPoint(target.position).z < 0)
        {
            targetPos = new Vector2(2f * Camera.main.pixelWidth, 2f * Camera.main.pixelHeight);
        }

        transform.position = targetPos;

        FillBar();
        transform.GetChild(0).GetChild(3).GetComponent<Text>().text = "" + target.gameObject.GetComponent<PowerUp>().m_health;
        transform.GetChild(0).GetChild(4).GetComponent<Text>().text = "" + target.gameObject.GetComponent<PowerUp>().m_health;
    }

    void FillBar()
    {
        amount = (float)target.gameObject.GetComponent<PowerUp>().m_health;
        if (amount <= total * 0.2f)
        {
            bar.color = new Color(1f, 54f / 255f, 54f / 255f, bar.color.a);
        }
        else
        {
            bar.color = new Color(1f, 141f / 255f, 0f / 255f, bar.color.a);
        }

        bar.fillAmount = 1f / total * amount;

    }
}
