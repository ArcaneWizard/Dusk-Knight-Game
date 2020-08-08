using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flying_Reaper : MonoBehaviour
{
    private Animator animator;
    private Rigidbody2D rig;
    private AudioSource audioSource;
    private Enemy_Health eH;
    private WeaponsCycle weaponsCycle;

    public GameObject player;
    public Camera camera;

    private float speed;
    public float speedModifier;  

    private Vector3 bottomLeft, topRight, randomPosition;
    private float minX, minY, maxX, maxY;
    private float pX, pY;
    private bool waiting;

    public float moveDelay;
    public Vector2 timeTillThrow;

    void Awake() {

        //Define components
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();  
        audioSource = transform.GetComponent<AudioSource>();
        eH = transform.GetComponent<Enemy_Health>();
        weaponsCycle = transform.GetComponent<WeaponsCycle>();

        speed = Enemy_Health.R3_speed;  

        //Set Dark Reaper movement bounds to the upper 30% portion of the screen
        bottomLeft = camera.ViewportToWorldPoint(new Vector2(0.15f, 0.7f));
        topRight = camera.ViewportToWorldPoint(new Vector2(1,1));

        minX = bottomLeft.x + 1;
        minY = bottomLeft.y + 1;
        maxX = topRight.x - 3f;
        maxY = topRight.y - 2.5f;
    }

    void Update()
    {
        //Reset enemy settings and start enemy movement
        if (eH.deploy == true)
        {
            eH.deploy = false;
            rig.gravityScale = 0;
            weaponsCycle.reloadAttack = false;
            waiting = false;

            //reset animator settings
            animator.SetBool("Dead", false);
            animator.SetBool("Attack", false);
            
            //start IEnumerator that handles attacking
            StartCoroutine(castProjectile());
        }

        //Orient the Dark Reaper in the correct direction 
        if (transform.position.x > player.transform.position.x && eH.hp > 0)
            transform.rotation = Quaternion.Euler(0, 180, 0);
            
        else if (eH.hp > 0)
            transform.rotation = Quaternion.Euler(0, 0, 0);    

        //Never fall unless frozen
        if (eH.freezeTimer <= 0 && rig.gravityScale != 0)  
            rig.gravityScale = 0;
    }

    //Move the reaper to its new position
    void FixedUpdate() 
    {
        //request for a new random position to head to when done waiting
        if (waiting == false && eH.freezeTimer <= 0)
            SendMessage("movement");

        //move to that new position
        if (eH.hp > 0 && eH.freezeTimer <= 0)
            transform.position = Vector3.Lerp(transform.position, randomPosition, Time.deltaTime * speed * speedModifier);
    }

    //Throw a projectile
    private IEnumerator castProjectile() 
    {
        //trigger casting animation after a random number of seconds
        yield return new WaitForSeconds(UnityEngine.Random.Range(timeTillThrow.x, timeTillThrow.y));
        animator.SetBool("Attack", true);

        //only run that animation once
        yield return new WaitForSeconds(0.3f);
        animator.SetBool("Attack", false);

        //restart this method to attack in the future
        StartCoroutine(castProjectile());
    }

    //update a new position for the reaper to head to
    private IEnumerator movement() 
    {
        //constrain Dark Reaper to the upper 30% of the screen
        pX = UnityEngine.Random.Range(minX, maxX);
        pY = UnityEngine.Random.Range(minY, maxY);

        //set a new position for the reaper to head to
        randomPosition = new Vector3(pX, pY, 0);  
        waiting = true;
        yield return new WaitForSeconds(moveDelay);
        waiting = false;
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Enemy landed on the ground after its motion was somehow disrupted (ex. frozen mid-air)
        if (col.gameObject.layer == 14 && rig.gravityScale != 0)
        {
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {
        //Enemy landed on the ground after its motion was somehow disrupted (ex. frozen mid-air)
        if (col.gameObject.layer == 14 && rig.gravityScale != 0)
        {
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);
        }
    }
}
