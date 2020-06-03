﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    Vector2 dir;
    [HideInInspector]
    public bool switchOrbs = false;

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
    }
    // Start is called before the first frame update
    void Update()
    {
        if (switchOrbs == true)
        {
            dir = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            if (gameObject.name == "witch orbs")
                transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 5f + new Vector2(0, UnityEngine.Random.Range(-1.5f, 1.5f));
            else if (gameObject.name == "blue orbs")
                transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 5f + new Vector2(0, UnityEngine.Random.Range(0, 4));
            else
                Debug.Log("orbs' name was changed. Error in the Orb script");

            transform.GetComponent<AudioSource>().PlayOneShot(Manage_Sounds.Instance.R1Attack, 0.12f * Manage_Sounds.soundMultiplier);
            switchOrbs = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 22)
            gameObject.SetActive(false);

        if (col.gameObject.layer == 10)
        {
            Transform enemy = transform.parent.transform.GetChild(0);

            if (gameObject.tag == "Witch orb")
                Health.playerHP -= Mathf.RoundToInt(Health.R3Dmg * enemy.GetComponent<Enemy_Health>().dmgMultiplier);

            if (gameObject.tag == "Reaper orb")
                Health.playerHP -= Mathf.RoundToInt(Health.R1Dmg * enemy.GetComponent<Enemy_Health>().dmgMultiplier);

            Manage_Sounds.Instance.playHitSound(Manage_Sounds.Instance.orbConnect, 0.4f);
            gameObject.SetActive(false);
        }
    }
}