using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class grenade : MonoBehaviour
{
    private float speed = 3f;
    Animator animator;
    public Sprite thing;
    public bool oneLaunch = false;
    private Rigidbody2D rig;

    private bool boomOnAlready = false;

    void Start()
    {
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        animator.SetBool("blowup", false);
        gameObject.AddComponent<AudioSource>();
    }

    void Update()
    {
        //Set bounds + reset settings when its being chosen from the array
        if ((Mathf.Abs(transform.position.x) < 10f && Mathf.Abs(transform.position.y) < 9f))
        {
            if (oneLaunch == false)
            {
                transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
                transform.GetComponent<Rigidbody2D>().AddForce(transform.up * speed * 400 * shooting.touchPercent);
                oneLaunch = true;
            }
        }


        //This bit of code makes the grenade (or arrow) rotate as it falls
        float rot = Mathf.Atan2(rig.velocity.y, rig.velocity.x) * Mathf.Rad2Deg;
        if (rig.velocity != new Vector2(0, 0))
            transform.rotation = Quaternion.Euler(0f, 0f, rot + 90f);

        if (rig.velocity != new Vector2(0, 0))
        {
            gameObject.transform.GetComponent<SpriteRenderer>().enabled = true;
            for (int i = 0; i < gameObject.transform.childCount; i++)
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
        }
    }

        private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {          
            col.gameObject.transform.GetComponent<Enemy_Health>().hp -= Health.grenade;
        }
        if (boomOnAlready == false)
        yesGoBoom();
    }

    private void yesGoBoom() {
        rig.velocity = new Vector2(0, 0);
        StartCoroutine(boom());
        Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
        transform.GetComponent<AudioSource>().PlayOneShot(m.explode, 0.7f * Manage_Sounds.soundMultiplier);
        boomOnAlready = true;
    }
    private IEnumerator boom()
    {
        //Collider needs to be larger (whenever the grenade hits the ground, enemies close to the land spot are not always reached by the explosion)
        rig.velocity = new Vector2(0, 0);
        transform.GetComponent<CircleCollider2D>().enabled = true;
        transform.GetComponent<PolygonCollider2D>().enabled = false;
        yield return new WaitForSeconds(0.01f);
        animator.SetBool("blowup", true);
        yield return new WaitForSeconds(0.03f);
        transform.GetComponent<CircleCollider2D>().radius = 0.3f;
        yield return new WaitForSeconds(0.04f);
        transform.GetComponent<CircleCollider2D>().radius = 0.5f;
        yield return new WaitForSeconds(0.05f);
        transform.GetComponent<CircleCollider2D>().radius = 0.7f;
        yield return new WaitForSeconds(0.03f);
        transform.GetComponent<CircleCollider2D>().radius = 1.3f;

        animator.SetBool("blowup", false);
        transform.GetComponent<CircleCollider2D>().radius = 0.0001f;
        transform.GetComponent<PolygonCollider2D>().enabled = true;
        transform.GetComponent<SpriteRenderer>().sprite = thing;
        transform.gameObject.SetActive(false);
    }

}
