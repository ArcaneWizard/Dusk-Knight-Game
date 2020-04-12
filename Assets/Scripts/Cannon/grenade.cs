using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class grenade : MonoBehaviour
{
    private float speed = 6f;
    Animator animator;
    private bool stop = false;

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
        transform.GetComponent<PolygonCollider2D>().enabled = false;
        StartCoroutine(boom());

            //.gameObject.transform.GetComponent<Enemy_Health>().hp -= 100;
            //Instantiate(minisplat, transform.position, Quaternion.identity);
            //transform.gameObject.SetActive(false);
    }

    private IEnumerator boom()
    {
        animator.SetBool("blowup", true);
        yield return new WaitForSeconds(0.5f);
        animator.SetBool("blowup", false);
        animator.SetBool("revert", true);
        yield return new WaitForSeconds(0.1f);
        animator.SetBool("revert", false);
        transform.gameObject.SetActive(false);
        stop = false;
    }
}
