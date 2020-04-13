using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if((Mathf.Abs(transform.position.x)<9.0f || Mathf.Abs(transform.position.y) < 5.7f) && stop == false)
        {
            transform.Translate(new Vector3(0, 1, 0) * speed * Time.deltaTime);
        }
        else if(stop==false)
        {
            transform.gameObject.SetActive(false);
        }
        
    }


    private void OnTriggerEnter2D(Collider2D collision)
    {
        //transform.gameObject.GetComponent<>
        stop = true;
        //transform.GetComponent<PolygonCollider2D>().enabled = false;
        StartCoroutine(boom());
        if (stop == true)
        {
            collision.gameObject.transform.GetComponent<Enemy_Health>().hp -= 100;
        }

            //.gameObject.transform.GetComponent<Enemy_Health>().hp -= 100;
            //Instantiate(minisplat, transform.position, Quaternion.identity);
            //transform.gameObject.SetActive(false);
    }

    private IEnumerator boom()
    {

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
