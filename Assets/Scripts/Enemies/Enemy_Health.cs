﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy_Health : MonoBehaviour
{
    //[HideInInspector]
    public int hp = 1;
    private int lastHP;
    private int maxHP;

    private int orc = 120;
    private int ogre = 70;
    private int goblin = 50;
    private int reaper_1 = 60;
    private int reaper_2 = 80;
    private int reaper_3 = 80;

    public static float orc_speed = 0.4f;
    public static float ogre_speed = 0.5f;
    public static float goblin_speed = 0.9f;
    public static float R1_speed = 0.5f;
    public static float R2_speed = 0.7f;

    Animator animator;
    private bool death = false;
    public bool deploy = true;

    private Vector2 localPos;
    private float ogScaleX;
    private float ogScaleY;

    // Start is called before the first frame update
    void Awake()
    {
        setHP();
        ogScaleX = transform.localScale.x;
        ogScaleY = transform.localScale.y;
        animator = transform.GetComponent<Animator>();
    }

    public void setHP()
    {
        if (gameObject.layer == 8)
            hp = orc;

        if (gameObject.layer == 9)
            hp = ogre;

        if (gameObject.layer == 11)
            hp = goblin;

        if (gameObject.layer == 19)
            hp = reaper_1;

        if (gameObject.layer == 20)
            hp = reaper_2;

        if (gameObject.layer == 21)
            hp = reaper_3;

        lastHP = hp;
        maxHP = hp;
        death = false;

        if (gameObject.tag == "Ranged Shooter")
            transform.localPosition = new Vector2(0, 0);
    }

    void Update()
    {
        if (hp <= 0 && death == false)
        {
            checkDeath();
            death = true;
        }

        if (lastHP != hp)
        {
            lastHP = hp;
            StartCoroutine(alter());
        }

        /*if (hp > 0 && hp <= maxHP/2)
            gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(255, 137, 137, 255);*/

    }

    private IEnumerator fade()
    {
        yield return new WaitForSeconds(0.1f);
        float scale = 0.93f;
        float rotSpeed = 25f;

        for (int i = 17; i >= 0; i--)
        {
            transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, (byte)(15*i));
            transform.localScale *= scale;
            transform.Rotate(new Vector3(0, 0, rotSpeed));

            yield return new WaitForSeconds(0.05f); 
        }

        if (gameObject.tag != "Ranged Shooter")
            gameObject.SetActive(false);
        else
            gameObject.transform.parent.gameObject.SetActive(false);
    }

    public void undoFade()
    {
        transform.rotation = Quaternion.Euler(0, 0, 0);
        transform.localScale = new Vector2(ogScaleX, ogScaleY);
        transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }

    private void checkDeath()
    {
        animator.SetBool("Dead", true);

        transform.GetComponent<PolygonCollider2D>().enabled = false;
        transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        if (gameObject.layer == 21)
            transform.GetChild(0).gameObject.SetActive(false);

        StartCoroutine(fade());
    }

    private IEnumerator alter()
    {
        gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(245, 0, 0, 255);
        yield return new WaitForSeconds(0.1f);
        gameObject.transform.GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 255);
    }
}
