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

    public float velocity;  

    private Vector3 bottomLeft, topRight, randomPosition;
    private float minX, minY, maxX, maxY;
    private float pX, pY;

    public float moveDelay;
    public float amplitude;
    public Vector2 timeTillThrow;
    private float xRot;
    private float yRot;

    private bool firstTime;

    private Quaternion dir = Quaternion.Euler(0, 0, 0);

    void Awake() {

        //Define components
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();  
        audioSource = transform.GetComponent<AudioSource>();
        eH = transform.GetComponent<Enemy_Health>();
        weaponsCycle = transform.GetComponent<WeaponsCycle>();

        //Set Dark Reaper movement bounds to the upper 30% portion of the screen
        bottomLeft = camera.ViewportToWorldPoint(new Vector2(0.15f, 0.5f));
        topRight = camera.ViewportToWorldPoint(new Vector2(1,1));

        minX = bottomLeft.x;
        minY = bottomLeft.y + 1;
        maxX = topRight.x - 1.4f;
        maxY = topRight.y - 1.8f;

        StartCoroutine(changeDir());
    }

    void Update()
    {
        //Reset enemy settings and start enemy movement
        if (eH.deploy == true)
        {
            eH.deploy = false;
            rig.gravityScale = 0;
            weaponsCycle.reloadAttack = false;
            firstTime = false;

            //reset animator settings
            animator.SetBool("Dead", false);
            animator.SetBool("Attack", false);
            
            //start IEnumerator that handles attacking
            StartCoroutine(castProjectile());

            //Orient the Dark Reaper in the correct direction 
            if (transform.position.x > player.transform.position.x)
                transform.rotation = Quaternion.Euler(0, 180, 0);
        }

        //Fall if frozen
        if (eH.freezeTimer <= 0 && rig.gravityScale != 0)  
            rig.gravityScale = 0;

        movementAlgorithm();
    }

    //Reaper flying movement 
    private void movementAlgorithm() 
    {
        //Bounds to prevent the reaper from flying off the map by making it turn away from the edge
        if (transform.position.x > maxX) {
            xRot = UnityEngine.Random.Range(160f, 200f);
            velocity += UnityEngine.Random.Range(0.7f, 0.71f);
        }
        else if (transform.position.x < minX) {
            xRot = UnityEngine.Random.Range(-20f, 20f);;
            velocity += UnityEngine.Random.Range(0.7f, 0.71f);
        }
        else {
            xRot = -1;

            if (firstTime) {
                velocity = UnityEngine.Random.Range(-2f, 5f);
                dir.eulerAngles = new Vector3(0, 0, UnityEngine.Random.Range(90f, 270f));
                firstTime = false;
            }
        }

        if (transform.position.y > maxY) {
            yRot = UnityEngine.Random.Range(250f, 290f);
            velocity += UnityEngine.Random.Range(0.7f, 0.71f);
        }
        else if (transform.position.y < minY) {
            yRot = UnityEngine.Random.Range(70f, 110f);
            velocity += UnityEngine.Random.Range(0.7f, 0.71f);
        }
        else
            yRot = -1;

        //Max speed limit. If passed, flip velocity in the opposite direction
        if (Mathf.Abs(velocity) > 7 && xRot < 0 && yRot < 0)
            velocity = -7 * Mathf.Sign(velocity);

        //If going off its specified "map cage" horizontally, vertically, or both
        if (xRot >= 0 && yRot < 0)
            dir = Quaternion.Euler(0, 0, xRot);
        else if (xRot < 0 && yRot >= 0)
            dir = Quaternion.Euler(0, 0, yRot);
        else if (xRot >= 0 && yRot >= 0)
            dir = Quaternion.Euler(0, 0, (xRot + yRot)/2);
        else
            velocity += (Mathf.PerlinNoise(Time.deltaTime * 2, transform.GetSiblingIndex() + 5) - 0.5f) * UnityEngine.Random.Range(1f, 3f);

        //set velocity based off direction and velocity
        if (eH.hp > 0 && eH.freezeTimer <= 0) 
            rig.velocity = dir * new Vector2(velocity, 0);
    }

    //Reaper changes direction based off Perlin Noise
    private IEnumerator changeDir() {
        yield return new WaitForSeconds(moveDelay);
        float z = Mathf.PerlinNoise(Time.deltaTime, transform.GetSiblingIndex()) * amplitude;
        dir = Quaternion.Euler(0, 0, z * Mathf.Sign((Mathf.PerlinNoise(Time.deltaTime, transform.GetSiblingIndex())) - 0.5f) + dir.eulerAngles.z);
        StartCoroutine(changeDir());
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
