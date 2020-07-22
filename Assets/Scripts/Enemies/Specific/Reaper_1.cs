using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_1 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;

    public GameObject hill;
    private float speed;
    private bool counter = false;
    private int arrowIndex = 0;
    private AudioSource audioSource;
    private Enemy_Health enemy_Health;
    private bool dontGetCloser = false;
    private bool begin_motion = false;
    public GameObject spawn_anim;

    void Awake()
    {
        audioSource = transform.GetComponent<AudioSource>();
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        enemy_Health = transform.GetComponent<Enemy_Health>();

        speed = Enemy_Health.R1_speed;

    }

    void Update()
    {
        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            transform.GetComponent<Enemy_Health>().deploy = false;

            //Orient Reaper to the left 
            speed = -Mathf.Abs(speed);
            transform.rotation = Quaternion.Euler(0, 180, 0);

            StartCoroutine(activate());
        }

        if(begin_motion)
        {
            //Set enemy movement based off hill arrows that outline the hill
            Quaternion initDir = hill.transform.GetChild(0).transform.rotation;
            Quaternion finalDir = hill.transform.GetChild(1).transform.rotation;
            float distance = hill.transform.GetChild(0).transform.position.x - hill.transform.GetChild(1).transform.position.x;

            rig.gravityScale = 0;
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);

            //Can start following any arrow at the start, but as arrowIndex goes up, the enemy can't refollow the arrow at a lower index 
            arrowIndex = 0;

            begin_motion = false;
        }
    }

    private IEnumerator activate()
    {
        spawn_anim.transform.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<SpriteRenderer>().enabled = false;
        spawn_anim.transform.GetComponent<Animator>().SetBool("Spawn", true);


        yield return new WaitForSeconds(0.41f);


        spawn_anim.transform.GetComponent<SpriteRenderer>().enabled = false;
        transform.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<Animator>().enabled = true;
        spawn_anim.transform.GetComponent<Animator>().SetBool("Spawn", false);


        begin_motion = true;
    }

    void OnTriggerStay2D(Collider2D col)
    {

        //Ogre is being directed by a movement arrow
        if (col.gameObject.layer == 13 && rig.gravityScale == 0)
        {

            //get movement arrow index
            int index = col.gameObject.transform.GetSiblingIndex();

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

            //find the current and next direction the enemy should move in
            Quaternion initDir = col.transform.rotation;
            Quaternion finalDir = col.transform.parent.GetChild(index + 1).transform.rotation;

            //find the distance between the two arrow points
            float distance = col.transform.position.x - col.transform.parent.GetChild(index + 1).transform.position.x;

            //Turn the enemy from its current direction to the next direction
            rig.velocity = Vector3.Lerp(initDir * -Vector3.right * speed, finalDir * -Vector3.right * speed, distance / 20f);
        }
    }

    private IEnumerator explode()
    {
        spawn_anim.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<SpriteRenderer>().enabled = false;
        spawn_anim.GetComponent<Animator>().SetBool("Boom", true);
        StartCoroutine(playSound());
        yield return new WaitForSeconds(0.21f);
        spawn_anim.GetComponent<Animator>().SetBool("Boom", false);
        transform.GetComponent<Enemy_Health>().hp = 0;
    }

    private IEnumerator playSound()
    {
        yield return new WaitForSeconds(0.22f);
        audioSource.PlayOneShot(Manage_Sounds.Instance.goblinAttack, Manage_Sounds.soundMultiplier);
        yield return new WaitForSeconds(0.78f);
        StartCoroutine(playSound());
    }

    void OnTriggerEnter2D(Collider2D col)
    {
        //Goblin is next to the tower, wants to start bashing
        if (col.gameObject.layer == 17)
        {
            //stop following the arrows
            dontGetCloser = true;
            rig.velocity = new Vector2(0, 0);

            StartCoroutine(explode());

        }
    }
}