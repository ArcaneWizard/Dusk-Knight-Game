using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;

    public Vector2 jumpForce;
    public Vector2 jumpVariance;
    public Vector2 timeTillJump;
    public Vector2 blinkDuration;
    public float jumpMotionDelay;
    public float pushOffDuration;
    public float undoDuration;  
    private float speed = 1.0f;

    private float turnTime = 20f;
    private bool landedJump;
    private float arrowIndex = 0;

    public AudioClip launch;
    public float launchVolume;
    public AudioClip clobber;
    public float clobberVolume;
    
    public GameObject hill;

    void Awake() {
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();  
        audioSource = transform.GetComponent<AudioSource>();

        speed = Enemy_Health.orc_speed;   
    }

    void Update()
    {
        //Enemy was just deployed
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            transform.GetComponent<Enemy_Health>().deploy = false;

            //Orient the Orc in the correct direction 
            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x) {
                speed = -Mathf.Abs(speed);
                jumpForce.x = -Mathf.Abs(jumpForce.x);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }
            else {
                speed = Mathf.Abs(speed);
                jumpForce.x = Mathf.Abs(jumpForce.x);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

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
            
            rig.gravityScale = 0;
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / turnTime);
            arrowIndex = 0;
        }   
    }

    //Orc jumps a random height after a few seconds
    private IEnumerator Jump() {
        //jump after a random number of seconds
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeTillJump.x, timeTillJump.y));
        
        //sync jump animation to start before jump
        animator.SetInteger("Jump", 1);
        yield return new WaitForSeconds(jumpMotionDelay); 

        //random force upwards for the jimp
        float varianceX = UnityEngine.Random.Range(-jumpVariance.x, jumpVariance.x);
        float varianceY = UnityEngine.Random.Range(-jumpVariance.y, jumpVariance.y);
        Vector2 variance = new Vector2(varianceX, varianceY);
        rig.gravityScale = 1;
        rig.AddForce(jumpForce + variance);  

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

    void OnCollisionEnter2D(Collision2D col)
    {
        //Orc is next to the tower, wants to start bashing
        if (col.gameObject.layer == 17) {
            animator.SetInteger("Attack", 1);
            StartCoroutine(blink());
        }
    }

    private IEnumerator blink() {
        //If not taking dmg, then attack or blink
        if (!animator.GetBool("Hurt"))
        {
            //Play sound and do dmg during the attack animation 
            yield return new WaitForSeconds(0.22f);
            audioSource.PlayOneShot(clobber, clobberVolume * Manage_Sounds.soundMultiplier);
            yield return new WaitForSeconds(0.65f);

            //Start blinking for 1.2 to 4 seconds
            animator.SetInteger("Attack", 2);
            yield return new WaitForSeconds(UnityEngine.Random.Range(blinkDuration.x, blinkDuration.y));

            //Reattack
            animator.SetInteger("Attack", 1);
            StartCoroutine(blink());
        }

        else {
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(blink());
        }
    }

    void OnTriggerEnter2D(Collider2D col) {

        //Orc got hit by a player weapon
        if (col.gameObject.layer == 25) {
            
            animator.SetBool("Hurt", true);   
            Invoke("flinch", 0.1f);
        }

        //Orc is on the ground
        if (col.gameObject.layer == 14) {
            rig.gravityScale = 0;
            
            if (Mathf.Abs(rig.velocity.magnitude - speed) > 4f) {
                rig.velocity = new Vector2(0, 0);
                landedJump = true;
            }
        }
    }

    void OnTriggerStay2D(Collider2D col) {

        //Orc is landing on a movement arrow after its jump
        if (col.gameObject.layer == 13 && landedJump == true) {

            //call this only once            
            landedJump = false;

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

            //find the distance between the two arrow points
            float distance = col.transform.position.x - col.transform.parent.GetChild(index + 1).transform.position.x;

            //Turn the enemy from its current direction to the next direction
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / turnTime);
        }
        
        //Orc is being directed by a movement arrow
        if (col.gameObject.layer == 13 && rig.gravityScale == 0) {

            //get movement arrow index
            int index = col.gameObject.transform.GetSiblingIndex();

            //Don't change directions if this is the last movement arrow
            if (index == col.gameObject.transform.parent.childCount - 1) {
                rig.velocity = col.transform.rotation * -Vector3.right * speed;
                return;
            }

            //If touching two arrows, choose the one that's forward
            if (index >= arrowIndex)
                arrowIndex = index;
            else    
                return;

            //find the current and next direction the enemy should move in
            Quaternion initDir = col.transform.rotation;
            Quaternion finalDir = col.transform.parent.GetChild(index + 1).transform.rotation;

            //find the distance between the two arrow points
            float distance = col.transform.position.x - col.transform.parent.GetChild(index + 1).transform.position.x;

            //Turn the enemy from its current direction to the next direction
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / turnTime);
        }

    }
    
    //Orc flinches when hit sub-method
    private void flinch() {
        animator.SetBool("Hurt", false);
    }
}
