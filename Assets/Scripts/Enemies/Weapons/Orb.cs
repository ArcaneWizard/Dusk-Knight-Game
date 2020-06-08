﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private Vector2 dir;
    private float randomX;
    private float randomY;

    private AudioSource audioSource;
    private Rigidbody2D rig;
    public float volume;

    public GameObject tower;
    public float speed = 5f;
    public float variance = 2f;

    void Start()
    {
        //define components
        audioSource = transform.GetComponent<AudioSource>();
        rig = transform.GetComponent<Rigidbody2D>();

        //set direction and velocity of orb
        dir = tower.transform.position - transform.position;
        dir = new Vector2(dir.x / 1.4f, 0);

        //add randomness to the direction, with a bias towards undershooting
        randomX = Random.Range(-2.5f * variance, 2 * variance) / 3f;
        if (randomX >= -1.7f && randomX <= -0.83f)
        {
            randomX = Random.Range(-2.5f * variance, 0) / 3f;
        }
        randomY = Random.Range(-variance, variance) / 3f;        
        rig.velocity = dir * speed + new Vector2(dir.x * randomX, dir.y * randomY);

        //play cast orb sound effect
        audioSource.PlayOneShot(Manage_Sounds.Instance.R1Attack, volume * Manage_Sounds.soundMultiplier);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //disable orbs that hit the ground
        if (col.gameObject.layer == 22)
            gameObject.SetActive(false);

        //damage the player if they connect (+ audio)
        if (col.gameObject.layer == 10)
        {
            Health.playerHP -= Mathf.RoundToInt(Health.R3Dmg);
            Manage_Sounds.Instance.playHitSound(Manage_Sounds.Instance.orbConnect, 0.4f);
            gameObject.SetActive(false);
        }
    }
}
