using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class grenade : MonoBehaviour
{
    private float speed = 6f;
    Animator animator;
    private bool stop = false;
    public Sprite thing;

    void Start()
    {
        animator = transform.GetComponent<Animator>();
        animator.SetBool("blowup", false);
    }

    
    void Update()
    {
        //Could you reduce the outer bounds on this? They seem way larger than they need to be. Isn't really important lag wise but any optimization helps.
        if((Mathf.Abs(transform.position.x) < 10f && Mathf.Abs(transform.position.y) < 9f) && stop == false)
            transform.GetComponent<Rigidbody2D>().velocity = transform.up * speed;

        else if (stop == false)
            transform.gameObject.SetActive(false);

        if (stop == true)
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);

    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        stop = true;
        StartCoroutine(boom());

            //Same dmg is done to all Enemies for now but this could change if magical enemies are resistant to explosions later on
            //That's why I added specific enemy layers
            if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
            {
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= 100;
            }
    }

    private IEnumerator boom()
    {
        //Collider needs to be larger (whenever the grenade hits the ground, enemies close to the land spot are not always reached by the explosion)
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
