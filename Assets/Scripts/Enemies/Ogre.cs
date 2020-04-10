﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ogre : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    public float speed = 0.7f;
    private bool stopMoving = false;
    private bool quit = false;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
        {
            speed *= -1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(speed, 0);
        StartCoroutine(stageChange());
    }

    void Update()
    {
        transform.GetComponent<SpriteRenderer>().sortingOrder = -(int)Math.Round(transform.position.y * 100);
    }

    private IEnumerator stageChange()
    {

        if (stopMoving == false)
        {
            float r = UnityEngine.Random.Range(2.0f, 3.1f);
            yield return new WaitForSeconds(r);
            if (stopMoving == false)
            {
                animator.SetInteger("Stage", 1);
                rig.velocity = new Vector2(0, 0);
                yield return new WaitForSeconds(0.82f);
                if (stopMoving == true)
                    animator.SetInteger("Stage", 2);
            }
            if (stopMoving == false)
            {
                animator.SetInteger("Stage", 0);
                rig.velocity = new Vector2(speed, 0);
                if (stopMoving == true)
                    animator.SetInteger("Stage", 2);
                StartCoroutine(stageChange());
            }
        }
    }

    private IEnumerator newAction()
    {
        rig.velocity = new Vector2(0, 0);
        animator.SetInteger("Stage", 2);
        float r = UnityEngine.Random.Range(2.0f, 3.1f);
        yield return new WaitForSeconds(r);
        animator.SetInteger("Stage", 1);
        yield return new WaitForSeconds(0.82f);
        animator.SetInteger("Stage", 2);

        StartCoroutine(newAction());
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Range activation 2"))
        {
            stopMoving = true;
            StartCoroutine(newAction());
        }
    }
}
