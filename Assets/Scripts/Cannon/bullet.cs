using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class bullet : MonoBehaviour
{
    private float speed = 8f;
    Animator animator;
    private bool stop = false;
    public Sprite thing;

    void Start()
    {

    }


    void Update()
    {
        //Could you reduce the outer bounds on this? They seem way larger than they need to be. Isn't really important lag wise but any optimization helps.
        if ((Mathf.Abs(transform.position.x) < 8.7f || Mathf.Abs(transform.position.y) < 5.4f) && stop == false)
            transform.GetComponent<Rigidbody2D>().velocity = transform.position*1f;

        else if (stop == false)
            transform.gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        stop = true;


        //Same dmg is done to all Enemies for now but this could change if magical enemies are resistant to explosions later on
        //That's why I added specific enemy layers
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
            col.gameObject.transform.GetComponent<Enemy_Health>().hp -= 30;
        }
        
        transform.gameObject.SetActive(false);
        stop = false;

    }

}
