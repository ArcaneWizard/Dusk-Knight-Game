using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Ogre : MonoBehaviour
{
    public GameObject groundedCollider;

    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private Enemy_Health eH;

    public Vector2 timeTillThrow;
    private bool walking;

    private bool AttackedOnce = false;

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

        speed = -Enemy_Health.ogre_speed;
        speedMult = 1.0f;

        //when walking = false, the stand still and blink animation will be played over the walking animation
        walking = true;
    }

    void Update()
    {
        if (eH.deploy == true)
        {
            eH.deploy = false;

            //Orient the Ogre in the correct direction 
            transform.rotation = Quaternion.Euler(0, 180, 0);
            
            //Reset animation bools  
            animator.SetInteger("Stage", 0);
            animator.SetBool("Dead", false);
            StartCoroutine(ThrowProjectile());
           
           //Reset its physics and motion
            rig.velocity = new Vector2(0, 0);
            rig.gravityScale = 1;
            speedMult = 1;
            groundedCollider.SetActive(true);

            //Is able to follow all arrows at the beginning 
            arrowIndex = 0;

            //Keep on following the arrows until you get within range of the tower
            dontGetCloser = false;
       }
    }

    //Ogre throws a projectile after a few seconds
    private IEnumerator ThrowProjectile() {
        
        //enemy stops  after a random number of seconds
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeTillThrow.x, timeTillThrow.y)/speedMult);
        rig.velocity = new Vector2(0, 0);
        animator.SetInteger("Stage", 2);

        //enemy starts throwing a second after stopping
        yield return new WaitForSeconds(0.12f);
        animator.SetInteger("Stage", 1);

        //wait out the throwing animation and then switch back to walking or standing still afterwards
        yield return new WaitForSeconds(1f);
        animator.SetInteger("Stage", (walking == true) ? 0 : 2);
        eH.resetPath = true;
        rig.WakeUp();

        StartCoroutine(ThrowProjectile());
    }

    void OnCollisionEnter2D(Collision2D col) 
    {
        //Ogre landed on the ground after knockback
        if (col.gameObject.layer == 14 && rig.gravityScale != 0)
        {
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);
            eH.resetPath = true;
            groundedCollider.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Enemy has gotten close enough to the tower and should soon stop moving
        if (col.gameObject.layer == 18) 
            Invoke("stopApproachingTower", UnityEngine.Random.Range(0f, 14f));

        //slow down if collide with rage slow pulse thingy
        if (col.gameObject.layer == 20)
            speedMult = 0.5f;
    } 
    
    void OnTriggerExit2D(Collider2D col)
    {
        //Enemy has gotten as close enough to the tower as possible. Stop now.
        if (col.gameObject.layer == 18) 
            stopApproachingTower();
    }

    void OnTriggerStay2D(Collider2D col)
    {
        //Ogre is being directed by a movement arrow
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
    }

    //stop getting closer to the tower
    private void stopApproachingTower()
    {
        //stop following arrows
        dontGetCloser = true;
        rig.velocity = new Vector2(0, 0);

        //From now on, the ogre animation stands still and blinks instead of walking
        animator.SetInteger("Stage", 2);
        walking = false;
    }
}
