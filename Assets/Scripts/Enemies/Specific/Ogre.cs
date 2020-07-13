using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ogre : MonoBehaviour
{
    public GameObject hill;

    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private Enemy_Health enemy_Health;

    private bool AttackedOnce = false;

    private float speed = 2f;
    private int arrowIndex = 0;

    void Awake()
    {
        audioSource = transform.GetComponent<AudioSource>();
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        enemy_Health = transform.GetComponent<Enemy_Health>();

        speed = Enemy_Health.ogre_speed;
    }

    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            transform.GetComponent<Enemy_Health>().deploy = false;

            //Orient the Ogre in the correct direction 
            speed = -Mathf.Abs(speed);
            transform.rotation = Quaternion.Euler(0, 180, 0);
            
            //Reset animation bools  
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);

            //Set enemy movement based off hill arrows that outline the hill
            Quaternion initDir = hill.transform.GetChild(0).transform.rotation;
            Quaternion finalDir = hill.transform.GetChild(1).transform.rotation;
            float distance = hill.transform.GetChild(0).transform.position.x - hill.transform.GetChild(1).transform.position.x;
            
            rig.gravityScale = 0;
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
       
            //Can start following any arrow at the start, but as arrowIndex goes up, the enemy can't refollow the arrow at a lower index 
            arrowIndex = 0;
       }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Enemy has gotten close enough to the tower to stop moving
        if (col.gameObject.layer == 18)
            rig.velocity = new Vector2(0, 0);
    }

    void OnTriggerStay2D(Collider2D col) {
       
        //Ogre is being directed by a movement arrow
        if (col.gameObject.layer == 13 && rig.gravityScale == 0) {

            //get movement arrow index
            int index = col.gameObject.transform.GetSiblingIndex();

            //Don't change directions if this is the last movement arrow
            if (index == col.gameObject.transform.parent.childCount - 1) {
                rig.velocity = col.transform.rotation * -Vector3.right * speed;
                return;
            }

            //If touching two arrows, choose the one that's forward
            if (index > arrowIndex)
                arrowIndex = index;
            else    
                return;

            //find the current and next direction the enemy should move in
            Quaternion initDir = col.transform.rotation;
            Quaternion finalDir = col.transform.parent.GetChild(index + 1).transform.rotation;

            //find the distance between the two arrow points
            float distance = col.transform.position.x - col.transform.parent.GetChild(index + 1).transform.position.x;

            //Turn the enemy from its current direction to the next direction
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
        }
    }
}
