using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_1 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    private float speed = 0.4f;
    private bool counter = false;

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
    }

    private IEnumerator attack()
    {
        float r = UnityEngine.Random.Range(3.0f, 4.0f);
        yield return new WaitForSeconds(r);
        if (transform.GetComponent<Enemy_Health>().hp > 0)
            animator.SetBool("Attack", true);
        else
            yield break;

        if (transform.GetComponent<Enemy_Health>().hp > 0)
            rig.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(0.85f);
        if (counter == false && transform.GetComponent<Enemy_Health>().hp > 0)
        {
            animator.SetBool("Attack", false);
            if (transform.GetComponent<Enemy_Health>().isIced == false)
            rig.velocity = new Vector2(speed, 0);
            if (transform.GetComponent<Enemy_Health>().hp > 0)
               StartCoroutine(attack());
        }
    }

    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            counter = false;
            animator = transform.GetComponent<Animator>();
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);

            speed = Enemy_Health.R1_speed;
            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            {
                speed *= -1;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            rig = transform.GetComponent<Rigidbody2D>();
            rig.velocity = new Vector2(speed, 0);
            StartCoroutine(attack());
            transform.GetComponent<Enemy_Health>().deploy = false;
        }

        if (animator.GetBool("Attack") == false && counter == true)
        {
            animator.SetBool("Attack", true);
            rig.velocity = new Vector2(0, 0);
        }

        if (rig.velocity.x == speed && counter == true)
        {
            animator.SetBool("Attack", true);
            rig.velocity = new Vector2(0, 0);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Range activation"))
        {
            animator.SetBool("Attack", true);
            rig.velocity = new Vector2(0, 0);
            counter = true;
        }
    }
}
