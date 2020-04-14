using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    private float speed = 2f;
    private float delay = 1.2f;
    private bool AttackedOnce = false;

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
        StartCoroutine(run());
    }

    void Update()
    {
        //transform.GetComponent<SpriteRenderer>().sortingOrder = -(int)Math.Round(transform.position.y * 100);
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goblin Slashing") && transform.GetComponent<Enemy_Health>().hp > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 2f / 12f && AttackedOnce == true)
            {
                AttackedOnce = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 2f / 12f && AttackedOnce == false)
            {
                Health.playerHP -= Health.GobDmg;
                AttackedOnce = true;
            }
        }
    }

    private IEnumerator run()
    {
        yield return new WaitForSeconds(delay);
        animator.SetBool("Run", true);
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
