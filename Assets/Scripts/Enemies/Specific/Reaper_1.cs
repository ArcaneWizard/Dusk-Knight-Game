﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_1 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;

    public GameObject hill;
    private float speed;
    private bool counter = false;
    private int arrowIndex = 0;
    private AudioSource audioSource;
    private Enemy_Health eH;
    private bool dontGetCloser = false;
    private bool begin_motion = false;
    public GameObject spawn_anim;
    private SpriteRenderer sr;
    private bool explode_once;

    void Awake()
    {
        //defining components
        audioSource = transform.GetComponent<AudioSource>();
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        eH = transform.GetComponent<Enemy_Health>();
        sr = transform.GetComponent<SpriteRenderer>();

        speed = Enemy_Health.R1_speed;

    }

    void Update()
    {
        if (eH.deploy == true)
        {
            eH.deploy = false;

            //reset explosion conditions
            explode_once = false;

            //Orient Reaper to the left 
            speed = -Mathf.Abs(speed);
            transform.rotation = Quaternion.Euler(0, 180, 0);

            
            StartCoroutine(activate());
        }

        if(begin_motion)
        {
            //Set enemy movement based off hill arrows that outline the hill
            Quaternion initDir = hill.transform.GetChild(0).transform.rotation;
            Quaternion finalDir = hill.transform.GetChild(1).transform.rotation;
            float distance = hill.transform.GetChild(0).transform.position.x - hill.transform.GetChild(1).transform.position.x;

            rig.gravityScale = 0;
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);

            //Can start following any arrow at the start, but as arrowIndex goes up, the enemy can't refollow the arrow at a lower index 
            arrowIndex = 0;

            begin_motion = false;
        }

        //explode whenever the reaper dies
        if (eH.hp <= 0 && !explode_once)
        {
            explode_once = true;
            StartCoroutine(explode());
        }
    }

    //plays the spawn animation
    private IEnumerator activate()
    {
        //turn off reaper sprite, turn on spawn animation
        sr.enabled = false;
        spawn_anim.GetComponent<Animator>().SetBool("Spawn", true);

        yield return new WaitForSeconds(0.5f);

        //turn off spawn animation, turn on reaper sprite
        sr.enabled = true;
        spawn_anim.GetComponent<Animator>().SetBool("Spawn", false);

        spawn_anim.GetComponent<SpriteRenderer>().sprite = null;

        begin_motion = true;
    }

    void OnTriggerStay2D(Collider2D col)
    {

        //Repaer is being directed by a movement arrow
        if (col.gameObject.layer == 13 && rig.gravityScale == 0)
        {

            //get movement arrow index
            int index = col.gameObject.transform.GetSiblingIndex();

            //Don't change directions if this is the last movement arrow
            if (index == col.gameObject.transform.parent.childCount - 1)
            {
                rig.velocity = col.transform.rotation * -Vector3.right * speed;
                return;
            }

            //If touching two arrows, choose the one that's forward
            if (index > arrowIndex)
                arrowIndex = index;
            else
                return;

            //find the current and next direction the enemy should move in
            Quaternion initDir = col.transform.rotation;
            Quaternion finalDir = col.transform.parent.GetChild(index + 1).transform.rotation;

            //find the distance between the two arrow points
            float distance = col.transform.position.x - col.transform.parent.GetChild(index + 1).transform.position.x;

            //Turn the enemy from its current direction to the next direction
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
        }
    }

    private IEnumerator explode()
    {
        yield return new WaitForSeconds(0.2f);
        //makes reaper disappear
        sr.enabled = false;

        //show explosion
        spawn_anim.GetComponent<Animator>().SetBool("Boom", true);
        yield return new WaitForSeconds(0.15f);
        spawn_anim.GetComponent<Animator>().SetBool("Boom", false);

    }

    //plays explosion sound
    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(0.22f);
        audioSource.PlayOneShot(Manage_Sounds.Instance.goblinAttack, Manage_Sounds.soundMultiplier);
        yield return new WaitForSeconds(0.78f);
        StartCoroutine(playSound());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Reaper is next to the tower
        if (col.gameObject.layer == 17)
        {
            //stop following the arrows
            dontGetCloser = true;
            rig.velocity = new Vector2(0, 0);

            //kill reaper which will make it explode from the update function
            eH.hp = 0;

            //do damage
            health.hp -= Enemy_Health.R1Dmg;

        }
    }
}