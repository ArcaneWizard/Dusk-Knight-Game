using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Goblin : MonoBehaviour
{
    public GameObject hill;
    public GameObject groundedCollider;

    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private Enemy_Health eH;

    private bool canAttack = false;
    private bool dontGetCloser = false;
    private float speed;
    private int arrowIndex = 0;
    private int index;
    private float speedMult;

    void Awake()
    {
        audioSource = transform.GetComponent<AudioSource>();
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        eH = transform.GetComponent<Enemy_Health>();

        speed = -Enemy_Health.goblin_speed;
        speedMult = 1.0f;
    }

    void Update()
    {
        if (eH.deploy == true)
        {
            eH.deploy = false;
            
            //Orient the Goblin in the correct direction 
            transform.rotation = Quaternion.Euler(0, 180, 0);

            //Reset animation bools and the ability to attack
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);
            canAttack = true;

            //Set enemy movement based off hill arrows that outline the hill
            Quaternion initDir = hill.transform.GetChild(0).transform.rotation;
            Quaternion finalDir = hill.transform.GetChild(1).transform.rotation;
            float distance = hill.transform.GetChild(0).transform.position.x - hill.transform.GetChild(1).transform.position.x;
            
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
            rig.gravityScale = 1;
            groundedCollider.SetActive(true);

            //Is able to follow all arrows at the beginning 
            arrowIndex = 0;

            //Keep on following the arrows until you get within range of the tower
            dontGetCloser = false;
        }

        //Do dmg once every attack animation cycle
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goblin Slashing") && eH.hp > 0 && animator.enabled)
        {
            //get what % of the animation has played as a float from 0-1
            float progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

            if (progress > 2f/12f && progress < 0.5f && canAttack) {
                canAttack = false;
                Invoke("reloadAttack", 0.5f/speedMult);

                //do dmg to the player and play a sound
                health.hp -= Enemy_Health.goblinDmg * eH.dmgMultiplier;
                audioSource.PlayOneShot(Manage_Sounds.Instance.goblinAttack, Manage_Sounds.soundMultiplier);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Goblin is next to the tower, wants to start bashing
        if (col.gameObject.layer == 17)
        {
            //start bashing the tower
            animator.SetBool("Attack", true);

            //stop following the arrows
            dontGetCloser = true;
            rig.velocity = new Vector2(0, 0);
        }

        // slow down if collide with rage slow pulse thingy
        if (col.gameObject.layer == 20)
        {
            speedMult = 0.5f;
        }
    }

    void OnCollisionEnter2D(Collision2D col) 
    {
        //Goblin landed on the ground after its motion was somehow disrupted
        if (col.gameObject.layer == 14 && rig.gravityScale != 0)
        {
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);
            groundedCollider.SetActive(false);
            eH.resetPath = true;
            dontGetCloser = false;
            animator.SetBool("Attack", false);
        }
    }

    private void reloadAttack() {
        canAttack = true;
    }

    void OnTriggerStay2D(Collider2D col)
    {
        //Orc is being directed by a movement arrow
        if (col.gameObject.layer == 13 && rig.gravityScale == 0 && dontGetCloser == false && eH.freezeTimer <= 0 && eH.hp > 0)
        {
            //If the enemy movement is disrupted by knockback or something and needs to be reset
            if (eH.resetPath == true)
            {
                //call this if statement only once
                eH.resetPath = false;
                
                //reset arrowIndex to the index of the arrow you land on
                arrowIndex = col.gameObject.transform.GetSiblingIndex();
                index = arrowIndex;

                //Don't change directions if this is the last movement arrow
                if (arrowIndex == col.gameObject.transform.parent.childCount - 1)
                {
                    rig.velocity = col.transform.rotation * -Vector3.right * speed;
                    return;
                }
            }

            else
            {
                //get movement arrow index
                index = col.gameObject.transform.GetSiblingIndex();

                //Don't change directions if this is the last movement arrow
                if (index == col.gameObject.transform.parent.childCount - 1)
                {
                    rig.velocity = col.transform.rotation * -Vector3.right * speed * speedMult;
                    return;
                }

                //If touching two arrows, choose the one that's forward
                if (index > arrowIndex)
                    arrowIndex = index;
                else
                    return;
            }

            //find the current and next direction the enemy should move in
            Quaternion initDir = col.transform.rotation;
            Quaternion finalDir = col.transform.parent.GetChild(index + 1).transform.rotation;

            //find the distance between the two arrow points
            float distance = col.transform.position.x - col.transform.parent.GetChild(index + 1).transform.position.x;

            //Turn the enemy from its current direction to the next direction
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed * speedMult, finalDir * -Vector3.right * speed * speedMult, distance / 20f);
        }

        //If the goblin gets knocked towards the tower somehow
        if (col.gameObject.layer == 17 && !animator.GetBool("Attack")) 
        {
            //start bashing
            animator.SetBool("Attack", true);

            //stop following the arrows
            dontGetCloser = true;
            rig.velocity = new Vector2(0, 0);
        }
    }
}