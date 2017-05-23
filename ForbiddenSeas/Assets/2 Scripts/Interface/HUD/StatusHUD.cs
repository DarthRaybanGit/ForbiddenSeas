using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StatusHUD : MonoBehaviour
{
    public Sprite[] spriteDebuff;
    public Sprite[] spriteBuff;
    private Image[] neg,pos;

    void Start()
    {
        neg = new Image[2];
        pos = new Image[3];

        neg[0] = transform.GetChild(0).GetComponent<Image>();
        neg[1] = transform.GetChild(1).GetComponent<Image>();

        pos[0] = transform.GetChild(2).GetComponent<Image>();
        pos[1] = transform.GetChild(3).GetComponent<Image>();
        pos[2] = transform.GetChild(4).GetComponent<Image>();

    }

    public IEnumerator ActivateDebuff(int debuff, int max, float sec)
    {
        neg[max - 1].gameObject.SetActive(true);
        neg[max - 1].sprite = spriteDebuff[debuff];
        yield return new WaitForSeconds(sec);
        neg[max-1].gameObject.SetActive(false);
    }

    public IEnumerator ActivateBuff(int buff, int max, float sec)
    {
        pos[max - 1].gameObject.SetActive(true);
        pos[max - 1].sprite = spriteBuff[buff];
        yield return new WaitForSeconds(sec);
        pos[max-1].gameObject.SetActive(false);
    }


}
