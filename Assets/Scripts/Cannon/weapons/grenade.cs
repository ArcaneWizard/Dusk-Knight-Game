using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class grenade : MonoBehaviour
{
    private float speed = 3f;
    Animator animator;
    private bool stop = false;
    public Sprite thing;
    public bool oneLaunch = false;
    private Rigidbody2D rig;

    void Start()
    {
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        animator.SetBool("blowup", false);
        //gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        //Set bounds + reset settings when its being chosen from the array
        if ((Mathf.Abs(transform.position.x) < 10f || Mathf.Abs(transform.position.y) < 9f) && stop == false)
        {
            if (oneLaunch == false)
            {
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                transform.GetComponent<Rigidbody2D>().AddForce(transform.up * speed * 400 * shooting.touchPercent);
                oneLaunch = true;
            }
        }

        else if (stop == false)
            transform.gameObject.SetActive(false);

        //This bit of code makes the grenade (or arrow) rotate as it falls
        float rot = Mathf.Atan2(rig.velocity.y, rig.velocity.x) * Mathf.Rad2Deg;
        if (rig.velocity != new Vector2(0, 0))
            transform.rotation = Quaternion.Euler(0f, 0f, rot + 90f);

        //Stop moving
        if (stop == true)
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

        if (rig.velocity != new Vector2(0, 0))
        {
            gameObject.transform.GetComponent<SpriteRenderer>().enabled = true;
            for (int i = 0; i < gameObject.transform.childCount; i++)
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
        }
    }

        private void OnTriggerEnter2D(Collider2D col)
    {
        stop = true;
        StartCoroutine(boom());

        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
         {
            col.gameObject.transform.GetComponent<Enemy_Health>().hp -= Health.grenade;
        }
    }

    private IEnumerator boom()
    {
        //Collider needs to be larger (whenever the grenade hits the ground, enemies close to the land spot are not always reached by the explosion)
        transform.GetComponent<CircleCollider2D>().enabled = true;
        animator.SetBool("blowup", true);
        yield return new WaitForSeconds(0.03f);
        transform.GetComponent<CircleCollider2D>().radius = 0.3f;
        yield return new WaitForSeconds(0.04f);
        transform.GetComponent<CircleCollider2D>().radius = 0.5f;
        yield return new WaitForSeconds(0.05f);
        transform.GetComponent<CircleCollider2D>().radius = 0.7f;
        yield return new WaitForSeconds(0.03f);
        transform.GetComponent<CircleCollider2D>().radius = 1f;

        animator.SetBool("blowup", false);
        transform.GetComponent<CircleCollider2D>().radius = 0.0001f;
        transform.GetComponent<SpriteRenderer>().sprite = thing;
        transform.gameObject.SetActive(false);
        stop = false;
    }
}
