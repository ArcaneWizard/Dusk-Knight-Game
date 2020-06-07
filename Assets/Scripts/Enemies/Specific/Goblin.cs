using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private Enemy_Health enemy_Health;

    private float delay = 1.2f;
    private bool AttackedOnce = false;

    private float speed = 2f;

    void Start()
    {
        audioSource = transform.GetComponent<AudioSource>();
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        enemy_Health = transform.GetComponent<Enemy_Health>();

        speed = Enemy_Health.goblin_speed;
    }

    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            transform.GetComponent<Enemy_Health>().deploy = false;
            
            //Orient the Goblin in the correct direction 
            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x) {
                speed = -Mathf.Abs(speed);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else {
                speed = Mathf.Abs(speed);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            //Reset animation bools and start enemy movement      
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);

            rig.velocity = new Vector2(speed, 0);
        }

        //Do dmg once every attack animation cycle
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goblin Slashing") && enemy_Health.hp > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 2f / 12f && AttackedOnce == true)
                AttackedOnce = false;
                
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 2f / 12f && AttackedOnce == false)
            {
                Health.playerHP -= Health.GobDmg;
                AttackedOnce = true;
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Goblin is next to the tower, wants to start bashing
        if (col.gameObject.layer == 17)
        {
            animator.SetBool("Attack", true);
            StartCoroutine(playSound());
            rig.velocity = new Vector2(0, 0);
        }
    }

    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(0.22f);
        audioSource.PlayOneShot(Manage_Sounds.Instance.goblinAttack, Manage_Sounds.soundMultiplier);
        yield return new WaitForSeconds(0.78f);
        StartCoroutine(playSound());
    }
}
