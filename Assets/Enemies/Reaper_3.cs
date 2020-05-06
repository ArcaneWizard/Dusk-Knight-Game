﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_3 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    private float speed = 0;
    private bool switchDir = false;

    private float height_Add;
    private float speed_y = 0.04f;
    private float height;

    private float x = 0;

    private float bound = 13.3f;
    private bool counter = false;
    private float ampMultiplier = 1;

    private IEnumerator varyHeight()
    {
        if (counter == false) {
            x += 0.02f;
            if (x > 1)
            {
                counter = true;
                speed = UnityEngine.Random.Range(1.4f, 3.0f);
                ampMultiplier = UnityEngine.Random.Range(0.6f, 1.15f);
            }
        }
        if (counter == true)
        {
            x -= 0.02f;
            if (x < 0)
            {
                counter = false;
                speed = UnityEngine.Random.Range(1.4f, 3.0f);
                ampMultiplier = UnityEngine.Random.Range(0.6f, 1.15f);
            }
        }        
        yield return new WaitForSeconds(speed_y);
        height = ampMultiplier * (float) Math.Sin(Math.PI * x) + 1.83f;
        height += height_Add;

        if (transform.GetComponent<Enemy_Health>().hp > 0)
        {
            transform.position = new Vector2(transform.position.x, height);
            StartCoroutine(varyHeight());
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            animator = transform.GetComponent<Animator>();
            animator.SetBool("Dead", false);

            speed = UnityEngine.Random.Range(2.0f, 3.0f);
            height_Add = UnityEngine.Random.Range(-0.2f, 0.1f);
            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            {
                speed *= -1;
                bound = -1.8f;
                transform.rotation = Quaternion.Euler(new Vector2(0, 180));
            }
            rig = transform.GetComponent<Rigidbody2D>();
            rig.velocity = new Vector2(speed, rig.velocity.y);
            StartCoroutine(varyHeight());
            transform.GetComponent<Enemy_Health>().deploy = false;
        }

        if (transform.GetComponent<Enemy_Health>().hp > 0 && rig != null)
        {
            if (bound == 13.3f && transform.position.x > 13.3f)
            {
                if (rig.velocity.x > 0)
                  bound = -1.8f;
                if (rig != null)
                rig.velocity = new Vector2(-speed, rig.velocity.y);
            }

            if (bound == -1.8f && transform.position.x < -1.8f)
            {
                if (rig.velocity.x < 0)
                    bound = 13.3f;
                rig.velocity = new Vector2(speed, rig.velocity.y);
            }

            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
                transform.rotation = Quaternion.Euler(new Vector2(0, 180));

            if (transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x)
                transform.rotation = Quaternion.Euler(new Vector2(0, 0));
        }
    }
}