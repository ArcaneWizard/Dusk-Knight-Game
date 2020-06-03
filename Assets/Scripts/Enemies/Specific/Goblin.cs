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

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            animator = transform.GetComponent<Animator>();
            animator.SetBool("Run", false);
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);

            speed = Enemy_Health.goblin_speed;
            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            {
                speed *= -1;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            rig = transform.GetComponent<Rigidbody2D>();
            rig.velocity = new Vector2(speed, 0);
            StartCoroutine(run());
            transform.GetComponent<Enemy_Health>().deploy = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goblin Slashing") && transform.GetComponent<Enemy_Health>().hp > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 2f / 12f && AttackedOnce == true)
            {
                AttackedOnce = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 2f / 12f && AttackedOnce == false)
            {
                Health.playerHP -= Mathf.RoundToInt(Health.GobDmg * transform.GetComponent<Enemy_Health>().dmgMultiplier);
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
            StartCoroutine(playSound());
            rig.velocity = new Vector2(0, 0);
        }
    }

    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(0.22f);
        transform.GetComponent<AudioSource>().PlayOneShot(Manage_Sounds.Instance.goblinAttack, 1f * Manage_Sounds.soundMultiplier);
        yield return new WaitForSeconds(0.78f);
        StartCoroutine(playSound());
    }
}
