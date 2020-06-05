using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orc : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rig;

    public Vector2 jumpForce;
    public Vector2 jumpVariance;
    public Vector2 timeTillJump;
    public Vector2 blinkDuration;
    public float jumpMotionDelay;
    public float pushOffDuration;
    public float undoDuration;  
    private float speed = 1.0f;

    public AudioClip launch;
    public float launchVolume;
    public AudioClip clobber;
    public float clobberVolume;
    private AudioSource audioSource;

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
            animator.SetBool("Dead", false);  
            animator.SetInteger("Jump", 0);    
            animator.SetFloat("spring speed", 0.7f);    

            rig.velocity = new Vector2(speed, 0);
            StartCoroutine(Jump()); 
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
        rig.AddForce(jumpForce + variance);  

        //play launch sound
        audioSource.PlayOneShot(launch, launchVolume * Manage_Sounds.soundMultiplier);

        //stop and reverse enemy animation back to walking
        animator.SetFloat("spring speed", 0);
        yield return new WaitForSeconds(pushOffDuration);        
        animator.SetFloat("spring speed", -1);    
        yield return new WaitForSeconds(undoDuration);         
        animator.SetInteger("Jump", 2);      
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //Orc just landed after a big leap
        if (col.gameObject.layer == 22) {            
            rig.velocity = new Vector2(speed, 0);
        }

        //Orc is next to the tower, wants to start bashing
        if (col.gameObject.layer == 17) {
            animator.SetInteger("Attack", 1);
            StartCoroutine(blink());
        }
    }

    private IEnumerator blink() {
        //Play sound and do dmg during the attack animation 
        yield return new WaitForSeconds(0.22f);
        audioSource.PlayOneShot(clobber, clobberVolume * Manage_Sounds.soundMultiplier);
        Health.playerHP -= Health.OrcDmg;
        yield return new WaitForSeconds(0.78f);
        
        //Start blinking for 1.2 to 4 seconds
        animator.SetInteger("Attack", 2);
        yield return new WaitForSeconds(UnityEngine.Random.Range(blinkDuration.x, blinkDuration.y));

        //Reattack
        animator.SetInteger("Attack", 1);
        StartCoroutine(blink());
    }
}
