using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class bullet : MonoBehaviour
{
    private float speed = 10f;
    private bool stop = false;
    public bool oneHit = false;

    void Update()
    {
        if (Mathf.Abs(transform.position.x) < 10f && Mathf.Abs(transform.position.y) < 9f && stop == false)
        {
            transform.GetComponent<Rigidbody2D>().velocity = transform.up * speed;
        }
        else if (stop == false)
        {
            transform.gameObject.SetActive(false);
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        //see shooting script (bool oneHit is set to false when a bullet is spawned). 
        stop = true;
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
            if (oneHit == false)
            {
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= 40;
                oneHit = true;
            }
        }

        stop = false;
        //Setting oneHit back to false here at the end of this method would not work. It needs to be done once by a seperate script like above to avoid splash dmg
        transform.gameObject.SetActive(false);
    }

}
