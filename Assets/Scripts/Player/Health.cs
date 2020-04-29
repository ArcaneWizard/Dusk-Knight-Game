﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Health : MonoBehaviour
{
    public static int playerHP;
    public int displayedHP;
    public static int maxPlayerHP;

    private BoxCollider2D boxy;
    private BoxCollider2D boxy2;

    private int stage = 1;
    public static int OrcDmg = 50;
    public static int GobDmg = 20;
    public static int R2Dmg = 25;
    public static int R3Dmg = 8;
    public static int R1Dmg = 10;
    public static int OgreDmg = 12;

    public static int arrow = 40;
    public static int bullet = 15;
    public static int CB = 40;
    public static float flame = 1.3f;
    public static float grenade = 30;
    public static int potion = 40;

    private Image hp;
    private GameObject head;
    private int applyTowerChangeOnce = 1;
    public bool hpBoost = false;
    public bool KillTower;
    public bool resetUpgrades;

    private bool diedOnce;
    public int jewels;


    // Start is called before the first frame update
    void Start()
    {
        maxPlayerHP = 1000;
        playerHP = maxPlayerHP;

        hp = GameObject.Find("Canvas").transform.GetChild(0).transform.GetChild(0).transform.GetComponent<Image>();
        head = GameObject.Find("Head");
    }

    // Update is called once per frame
    void Update()
    {
        if (resetUpgrades == true)
        {
            Shop.ShopInstance.ResetUpgrades();
            resetUpgrades = false;
        }

        displayedHP = playerHP;

        stage = PlayerPrefs.GetInt("Tower") + 1;
        hp.fillAmount = (float)playerHP / maxPlayerHP;
        hp.transform.parent.transform.GetChild(1).transform.GetComponent<Text>().text = stage.ToString();

        if (stage == 1)
            transform.GetChild(2).gameObject.SetActive(true);

        if (stage >= 2 && applyTowerChangeOnce != stage)
        {
            transform.GetChild(stage).gameObject.SetActive(false);
            transform.GetChild(stage + 1).gameObject.SetActive(true);

            applyTowerChangeOnce++;
            playerHP += 1000;
        }

        if (stage == 1) {
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 1.05f);
            maxPlayerHP = 1000; }
        if (stage == 2) { 
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 1.63f);
            maxPlayerHP = 2000; }
        if (stage == 3) { 
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 2.18f);
            maxPlayerHP = 3000; }
        if (stage == 4) { 
            head.transform.localPosition = new Vector2(head.transform.localPosition.x, 2.18f);
            maxPlayerHP = 4000; }

        if (hpBoost == true)
        {
            playerHP += maxPlayerHP / 2;
            hpBoost = false;
        }

        if (KillTower)
            playerHP = 0;

        if (playerHP > maxPlayerHP)
            playerHP = maxPlayerHP;

        if (playerHP <= 0 && diedOnce == false)
        {
            diedOnce = true;
            if (GameObject.Find("Canvas").transform.GetChild(7).gameObject.name == "Game Over")
            {
                GameObject.Find("Canvas").transform.GetChild(7).gameObject.SetActive(true);

                StartCoroutine(relayDeathMsg());
            }
            else
                Debug.LogError("You changed the child positioning of Game Over which was referenced in a script.");


            GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Animator>().enabled = true;
            StartCoroutine(reset_level());
        }
    }

    private IEnumerator relayDeathMsg()
    {
        yield return new WaitForSeconds(0.01f);
        Debug.Log(GameState.time + " " + PlayerPrefs.GetInt("Time"));
        if (GameState.time < PlayerPrefs.GetInt("Time"))
        {
            //second vs seconds
            string timeUnit = " seconds ";
            if (PlayerPrefs.GetInt("Time") - GameState.time == 1) timeUnit = " second ";

            //Set text
            GameObject.Find("Canvas").transform.GetChild(7).transform.GetChild(1).transform.GetComponent<Text>().text =
                "You died " + (PlayerPrefs.GetInt("Time") - GameState.time) + timeUnit + "before your best time of " + System.Math.Round(((float)PlayerPrefs.GetInt("Time") / 60f), 2) + " minutes";
        }
        else
        {
            GameObject.Find("Canvas").transform.GetChild(7).transform.GetChild(1).transform.GetComponent<Text>().text =
                "You died after a valiant effort, setting a new survival time of " + System.Math.Round(((float)PlayerPrefs.GetInt("Time") / 60f), 2) + " minutes";
        }
    }

    private IEnumerator reset_level()
    {
        yield return new WaitForSeconds(2.9f);
        SceneManager.LoadScene(0);
    }

}