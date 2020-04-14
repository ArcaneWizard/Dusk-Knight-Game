using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static int playerHP;
    public static int maxPlayerHP;

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
}
