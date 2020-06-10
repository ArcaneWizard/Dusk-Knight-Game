using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_1 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    private float speed;
    private bool counter = false;
    private bool once = false;
    private CircleCollider2D blastradius;
    public GameObject Spawn;
    public GameObject Explode;

    void Start()
    {
        animator = transform.GetComponent<Animator>();
        gameObject.AddComponent<AudioSource>();
        blastradius = transform.GetComponent<CircleCollider2D>();
        
    }

    private IEnumerator attack()
    {
        float r = UnityEngine.Random.Range(3.0f, 4.0f);
        yield return new WaitForSeconds(r);
        if (transform.GetComponent<Enemy_Health>().hp > 0)
            animator.SetBool("Attack", true);
        else
            yield break;

        if (transform.GetComponent<Enemy_Health>().hp > 0)
            rig.velocity = new Vector2(0, 0);
        yield return new WaitForSeconds(0.85f);
        if (counter == false && transform.GetComponent<Enemy_Health>().hp > 0)
        {
            animator.SetBool("Attack", false);
            if (transform.GetComponent<Enemy_Health>().isIced == false)
            rig.velocity = new Vector2(speed, 0);
            if (transform.GetComponent<Enemy_Health>().hp > 0)
               StartCoroutine(attack());
        }
    }

    void Update()
    {
        rig = transform.GetComponent<Rigidbody2D>();

        if (transform.GetComponent<Enemy_Health>().deploy == true)
        {
            counter = false;
            animator.SetBool("Dead", false);
            transform.GetComponent<SpriteRenderer>().enabled = false;
            Explode.SetActive(false);
            blastradius.enabled = false;

            speed = Enemy_Health.R1_speed;
            once = false;
            if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            {
                speed = -Mathf.Abs(speed);
                transform.rotation = Quaternion.Euler(0, 180, 0);
            }

            else
            {
                speed = Mathf.Abs(speed);
                transform.rotation = Quaternion.Euler(0, 0, 0);
            }

            StartCoroutine(activate(speed, rig));


            transform.GetComponent<Enemy_Health>().deploy = false;
        }

        if(transform.GetComponent<Enemy_Health>().hp < 0){

        }

    }

    private IEnumerator activate(float speed, Rigidbody2D rig)
    {
        transform.GetComponent<PolygonCollider2D>().enabled = false;
        Spawn.GetComponent<Animator>().enabled = true;
        Spawn.SetActive(true);

        yield return new WaitForSeconds(0.4f);

        Spawn.GetComponent<Animator>().enabled = false;
        Spawn.SetActive(false);
        transform.GetComponent<SpriteRenderer>().enabled = true;
        transform.GetComponent<PolygonCollider2D>().enabled = true;
        rig.velocity = new Vector2(speed, 0);
    }

    private IEnumerator death()
    {
        yield return new WaitForSeconds(0.06f);
        transform.GetComponent<SpriteRenderer>().enabled = false;
        Explode.GetComponent<Animator>().enabled = true;
        Explode.SetActive(true);
        yield return new WaitForSeconds(0.22f);
        Explode.SetActive(false);
        Explode.GetComponent<Animator>().enabled = false;
        transform.GetComponent<Enemy_Health>().hp = 0;
        yield return new WaitForSeconds(0.2f);
        transform.GetComponent<SpriteRenderer>().enabled = true;
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 17 && !blastradius.enabled)
        {
            animator.SetBool("Dead", true);
            transform.GetComponent<PolygonCollider2D>().enabled = false;

            blastradius.enabled = true;
            rig.velocity = new Vector2(0, 0);
            StartCoroutine(death());
        }

        else if(col.gameObject.layer == 17 && blastradius.enabled)
        {

        }

    }
}