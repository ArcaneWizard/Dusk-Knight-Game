using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static int playerHP;
    public static int maxPlayerHP;



    public BoxCollider2D boxy;
    public BoxCollider2D boxy2;
    public int stage = 1;
    public static int OrcDmg = 20;
    public static int GobDmg = 15;
    public static int R2Dmg = 15;

    private Image hp;

    // Start is called before the first frame update
    void Start()
    {
        maxPlayerHP = 700;
        playerHP = maxPlayerHP;

        hp = GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Image>();
    }

    // Update is called once per frame
    void Update()
    {
        hp.fillAmount = (float) playerHP / maxPlayerHP;

        Debug.Log(maxPlayerHP);
        Debug.Log(stage);
    }

    void OnCollisionEnter2D (Collision2D col)
    {
        if (col.gameObject.layer == 23 || col.gameObject.layer == 24)
        {
            if (col.gameObject.tag == "Witch orb")
                playerHP -= 10;

            if (col.gameObject.tag == "Reaper orb")
                playerHP -= 20;

            if (col.gameObject.tag == "Boulder")
                playerHP -= 25;
        }
    }

    public void Reset()
    {
        maxPlayerHP = 700;
        playerHP = maxPlayerHP;
        boxy.offset = new Vector2(0.02191818f, -0.5f);
        boxy.size = new Vector2(2.344175f, 1.9f);
        boxy2.offset = new Vector2(0.03646278f, 0.9f);
        boxy2.size = new Vector2(2.458501f, 0.7449135f);
        transform.GetChild(2).GetChild(5).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(6).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(7).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(3).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(4).gameObject.SetActive(false);
        transform.GetChild(2).GetChild(0).gameObject.SetActive(true);
        transform.GetChild(2).GetChild(1).gameObject.SetActive(true);
        transform.GetChild(2).GetChild(2).gameObject.SetActive(true);
    }

    public void Upgrade()
    {
        stage++;

        if (stage == 2)
        {
            boxy.offset = new Vector2(0.02191818f, -0.3f);
            boxy.size = new Vector2(2.344175f, 2.5f);
            boxy2.offset = new Vector2(0.03646278f, 1.4f);
            boxy2.size = new Vector2(2.458501f, 0.7449135f);
            transform.GetChild(2).GetChild(3).gameObject.SetActive(true);
            maxPlayerHP += 100;
        }
        if (stage == 3)
        {
            boxy.offset = new Vector2(0.02191818f, 0f);
            boxy.size = new Vector2(2.344175f, 3f);
            boxy2.offset = new Vector2(0.03646278f, 2f);
            boxy2.size = new Vector2(2.458501f, 0.7449135f);
            transform.GetChild(2).GetChild(4).gameObject.SetActive(true);
            maxPlayerHP += 100;
        }
        if (stage == 4)
        {
            boxy.offset = new Vector2(0.02191818f, -0.5f);
            boxy.size = new Vector2(2.344175f, 1.9f);
            boxy2.offset = new Vector2(0.03646278f, 0.9f);
            boxy2.size = new Vector2(2.458501f, 0.7449135f);
            transform.GetChild(2).GetChild(5).gameObject.SetActive(true);
            transform.GetChild(2).GetChild(6).gameObject.SetActive(true);
            transform.GetChild(2).GetChild(7).gameObject.SetActive(true);
            maxPlayerHP += 100;
        }
    }

}
