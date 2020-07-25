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
    private Enemy_Health eH;

    private bool AttackedOnce = false;

    private bool dontGetCloser = false;
    private float speed;
    private int arrowIndex = 0;
    private int index;

    void Awake()
    {
        audioSource = transform.GetComponent<AudioSource>();
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        eH = transform.GetComponent<Enemy_Health>();

        speed = -Enemy_Health.goblin_speed;
    }

    void Update()
    {
        if (eH.deploy == true)
        {
            eH.deploy = false;
            
            //Orient the Goblin in the correct direction 
            transform.rotation = Quaternion.Euler(0, 180, 0);

            //Reset animation bools  
            animator.SetBool("Attack", false);
            animator.SetBool("Dead", false);

            //Set enemy movement based off hill arrows that outline the hill
            Quaternion initDir = hill.transform.GetChild(0).transform.rotation;
            Quaternion finalDir = hill.transform.GetChild(1).transform.rotation;
            float distance = hill.transform.GetChild(0).transform.position.x - hill.transform.GetChild(1).transform.position.x;
            
            print("ya");
            rig.gravityScale = 0;
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
           
            //Is able to follow all arrows at the beginning 
            arrowIndex = 0;

            //Keep on following the arrows until you get within range of the tower
            dontGetCloser = false;
        }

        //Do dmg once every attack animation cycle
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Goblin Slashing") && eH.hp > 0)
        {
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 < 2f / 12f && AttackedOnce == true)
                AttackedOnce = false;
                
            if (animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1 >= 2f / 12f && AttackedOnce == false)
            {
                AttackedOnce = true;
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
            StartCoroutine(playSound());

            //stop following the arrows
            dontGetCloser = true;
            rig.velocity = new Vector2(0, 0);
        }

        //Goblin landed on the ground after its motion was somehow disrupted
        if (col.gameObject.layer == 14 && rig.gravityScale != 0)
        {
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);
            eH.resetPath = true;
        }
    }

    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(0.22f);
        health.hp -= Enemy_Health.goblinDmg * eH.dmgMultiplier;
        audioSource.PlayOneShot(Manage_Sounds.Instance.goblinAttack, Manage_Sounds.soundMultiplier);
        yield return new WaitForSeconds(0.78f);
        StartCoroutine(playSound());
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
                    rig.velocity = col.transform.rotation * -Vector3.right * speed;
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
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
        }
    }
}