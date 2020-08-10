using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private Enemy_Health eH;

    public Vector2 jumpForce;
    public Vector2 jumpVariance;
    public Vector2 timeTillJump;
    public Vector2 blinkDuration;
    public float jumpMotionDelay;
    public float pushOffDuration;
    public float undoDuration;  

    private bool canAttack;
    public bool dontGetCloser;
    private float speed;
    private int arrowIndex = 0;
    private int index;
    private float speedMult;

    public AudioClip launch;
    public float launchVolume;
    public AudioClip clobber;
    public float clobberVolume;
    
    public GameObject hill;
    public GameObject groundedCollider;

    void Awake() 
    {
        //defining components
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();  
        audioSource = transform.GetComponent<AudioSource>();
        eH = transform.GetComponent<Enemy_Health>();

        //move and jump to the left
        speed = -Enemy_Health.orc_speed;
        speedMult = 1.0f;
        jumpForce.x = -jumpForce.x;
    }

    void Update()
    {
        //Enemy was just deployed
        if (eH.deploy == true)
        {
            eH.deploy = false;

            //Orient the Orc in the correct direction 
            transform.rotation = Quaternion.Euler(0, 180, 0);

            //Reset animation bools and start enemy movement 
            animator.SetInteger("Attack", 0);
            animator.SetInteger("Jump", 0);    
            animator.SetBool("Dead", false);  
            animator.SetBool("Hurt", false);  
            animator.SetFloat("spring speed", 0.7f); 
            
            StartCoroutine(Jump());    

            //Set enemy movement based off hill arrows that outline the hill
            Quaternion initDir = hill.transform.GetChild(0).transform.rotation;
            Quaternion finalDir = hill.transform.GetChild(1).transform.rotation;
            float distance = hill.transform.GetChild(0).transform.position.x - hill.transform.GetChild(1).transform.position.x;

            groundedCollider.SetActive(true); 
            rig.gravityScale = 1;
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
            
            //Is able to follow all arrows at the beginning 
            arrowIndex = 0;

            //Keep on following the arrows until you get within range of the tower
            dontGetCloser = false;

        }   

        //Do dmg once every attack animation cycle
        if (animator.GetCurrentAnimatorStateInfo(0).IsName("Orc Slashing") && eH.hp > 0 && animator.enabled)
        {
            //get what % of the animation has played as a float from 0-1
            float progress = animator.GetCurrentAnimatorStateInfo(0).normalizedTime % 1;

            if (progress > 2f/12f && progress < 0.5f && canAttack) {
                canAttack = false;
                Invoke("reloadAttack", 0.5f/speedMult);

                //do dmg to the player and play a sound
                health.hp -= Enemy_Health.orcDmg * eH.dmgMultiplier;
                audioSource.PlayOneShot(Manage_Sounds.Instance.orcAttack, Manage_Sounds.soundMultiplier);
            }
        }

    }

    private void reloadAttack() {
        canAttack = true;
    }

    //Orc jumps a random height after a few seconds
    private IEnumerator Jump() {

        //jump after a random number of seconds
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeTillJump.x, timeTillJump.y)/speedMult);
        
        //Wait till the orc isn't frozen
        while (eH.freezeTimer > 0) {
            yield return new WaitForSeconds(0.5f);
        }

        //sync jump animation to start before jump
        animator.SetInteger("Jump", 1);
        yield return new WaitForSeconds(jumpMotionDelay); 

        //random force upwards for the jump
        float varianceX = UnityEngine.Random.Range(-jumpVariance.x, jumpVariance.x);
        float varianceY = UnityEngine.Random.Range(-jumpVariance.y, jumpVariance.y);
        Vector2 variance = new Vector2(varianceX, varianceY);
        rig.AddForce(jumpForce + variance);  

        rig.gravityScale = 1;

        yield return new WaitForSeconds(0.07f);
        groundedCollider.SetActive(true);      

        //play launch sound
        audioSource.PlayOneShot(launch, launchVolume * Manage_Sounds.soundMultiplier);

        //stop and reverse enemy animation back to walking
        animator.SetFloat("spring speed", 0);
        yield return new WaitForSeconds(pushOffDuration);  
        animator.SetFloat("spring speed", -1);    
        yield return new WaitForSeconds(undoDuration);         
        animator.SetInteger("Jump", 2);             
        animator.SetFloat("spring speed", 0);    
    }

    private IEnumerator blink() 
    {
        //Wait out the attack animation 
        yield return new WaitForSeconds(1f);

        //Start blinking for 1.2 to 4 seconds
        if (dontGetCloser)
            animator.SetInteger("Attack", 2);

        yield return new WaitForSeconds(UnityEngine.Random.Range(blinkDuration.x, blinkDuration.y));

        //Reattack
        if (dontGetCloser)
        {
            animator.SetInteger("Attack", 1);
            StartCoroutine(blink());
        }
    }

    void OnTriggerEnter2D(Collider2D col) 
    {
        //Orc is next to the tower, wants to start bashing
        if (col.gameObject.layer == 17) 
        {
            //start bashing
            animator.SetInteger("Attack", 1);
            StartCoroutine(blink());

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
        //Orc landed on the ground after its motion was somehow disrupted     
        if (col.gameObject.layer == 14 && rig.gravityScale != 0)
        {
            //reset motion
            groundedCollider.SetActive(false);
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);
            dontGetCloser = false;
            eH.resetPath = true;
            animator.SetInteger("Attack", 0);
            StopCoroutine(blink());
        }
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
        
        //Orc jumped at the tower
        if (col.gameObject.layer == 17 && animator.GetInteger("Attack") == 0) 
        {
            //start bashing
            animator.SetInteger("Attack", 1);
            StartCoroutine(blink());

            //stop following the arrows
            dontGetCloser = true;
            rig.velocity = new Vector2(0, 0);
        }
    }
}
