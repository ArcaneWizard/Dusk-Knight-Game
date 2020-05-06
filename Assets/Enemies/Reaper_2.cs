using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_2 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    private float speed = 1.6f;
    private bool AttackedOnce = false;

    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            animator = transform.GetComponent<Animator>();
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);
            animator.SetBool("Grounded", false);

            speed = Enemy_Health.R2_speed;
            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            {
                speed *= -1;
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            rig = transform.GetComponent<Rigidbody2D>();
            rig.velocity = new Vector2(speed, 0);
            transform.GetComponent<Enemy_Health>().deploy = false;
        }

        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Reaper 2 Slashing") && transform.GetComponent<Enemy_Health>().hp > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 2f / 12f && AttackedOnce == true)
            {
                AttackedOnce = false;
            }
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 2f / 12f && AttackedOnce == false)
            {
                Health.playerHP -= Mathf.RoundToInt(Health.R2Dmg * transform.GetComponent<Enemy_Health>().dmgMultiplier);
                AttackedOnce = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == LayerMask.NameToLayer("Ground"))
        {
            animator.SetBool("Grounded", true);
            if (transform.GetComponent<Enemy_Health>().isIced == false)
            rig.velocity = new Vector2(speed, 0);
        }
        if (col.gameObject.layer == LayerMask.NameToLayer("Range activation"))
        {
            animator.SetBool("Attack", true);
            StartCoroutine(playSound());
        }
    }
    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(0.22f);
        transform.GetComponent<AudioSource>().PlayOneShot(Manage_Sounds.Instance.R2Attack, 1f * Manage_Sounds.soundMultiplier);
        yield return new WaitForSeconds(0.78f);
        StartCoroutine(playSound());
    }
}
