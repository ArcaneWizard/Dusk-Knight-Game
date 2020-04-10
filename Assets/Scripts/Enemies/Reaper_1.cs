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
        StartCoroutine(attack());
    }

    private IEnumerator attack()
    {
        float r = UnityEngine.Random.Range(3.0f, 4.0f);
        yield return new WaitForSeconds(r);
        animator.SetBool("Attack", true);
        rig.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(0.85f);
        if (counter == false)
        {
            animator.SetBool("Attack", false);
            rig.velocity = new Vector2(speed, 0);
            StartCoroutine(attack());
        }
    }

    void Update()
    {
        transform.GetComponent<SpriteRenderer>().sortingOrder = -(int)Math.Round(transform.position.y * 100);

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
