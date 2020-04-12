using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    private float speed = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
        {
            speed *= -1;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent < Rigidbody2D>();
        rig.velocity = new Vector2(speed, 0);
    }

    void Update()
    {
        //transform.GetComponent<SpriteRenderer>().sortingOrder = -(int)Math.Round(transform.position.y * 100);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Range activation"))
        {
            animator.SetBool("Attack", true);
            rig.velocity = new Vector2(0, 0);
        }
    }
}
