using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_3 : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;

    private float speed;

    void Awake() {
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();  
        audioSource = transform.GetComponent<AudioSource>();

        speed = Enemy_Health.R3_speed;   
    }

    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            transform.GetComponent<Enemy_Health>().deploy = false;

            //Reset enemy settings and start enemy movement
            animator.SetBool("Dead", false);
            rig.gravityScale = 0;     
            //StartCoroutine(attack());
        }

        //Orient the Dark Reaper in the correct direction 
        if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
        {
            speed = -Mathf.Abs(speed);
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        else
        {
            speed = Mathf.Abs(speed);
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }

    //cast an orb at the player
    private IEnumerator attack() {

        
        yield return new WaitForSeconds(1);
        StartCoroutine(attack());
    }
}
