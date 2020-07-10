using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    public GameObject hill;

    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private Enemy_Health enemy_Health;

    private float delay = 1.2f;
    private float turnTime = 20f;

    private bool AttackedOnce = false;

    private float speed = 2f;

    void Awake()
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

            //Reset animation bools  
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);

            //Set enemy movement based off hill arrows that outline the hill
            Quaternion initDir = hill.transform.GetChild(0).transform.rotation;
            Quaternion finalDir = hill.transform.GetChild(1).transform.rotation;
            float distance = hill.transform.GetChild(0).transform.position.x - hill.transform.GetChild(1).transform.position.x;
            
            rig.gravityScale = 0;
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / turnTime);
        }

        //Do dmg once every attack animation cycle
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goblin Slashing") && enemy_Health.hp > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 2f / 12f && AttackedOnce == true)
                AttackedOnce = false;
                
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 2f / 12f && AttackedOnce == false)
            {
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

    void OnTriggerEnter2D(Collider2D col) {
        if (col.gameObject.layer == 13) {
            //get movement arrow index
            int index = col.gameObject.transform.GetSiblingIndex();

            //Don't change directions if this is the last movement arrow
            if (index == col.gameObject.transform.parent.childCount - 1) {
                rig.velocity = col.transform.rotation * -Vector3.right * speed;
                return;
            }

            //find the current and next direction the enemy should move in
            Quaternion initDir = col.transform.rotation;
            Quaternion finalDir = col.transform.parent.GetChild(index + 1).transform.rotation;

            //find the distance between the two rotation points
            float distance = col.transform.position.x - col.transform.parent.GetChild(index + 1).transform.position.x;

            print(distance/turnTime);
            //Turn the enemy from its current direction to the next direction
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / turnTime);
        }
    }
}
