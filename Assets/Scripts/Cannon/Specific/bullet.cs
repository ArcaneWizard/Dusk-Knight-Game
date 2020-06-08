using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class bullet : MonoBehaviour
{
    private float speed = 10f;
    public bool oneHit = false;

    void Update()
    {
        if (Mathf.Abs(transform.localPosition.x) < 14.4f && Mathf.Abs(transform.localPosition.y) < 8f)
        {
            transform.GetComponent<Rigidbody2D>().velocity = transform.up * speed;
            gameObject.transform.GetComponent<SpriteRenderer>().enabled = true;
        }
        else
            transform.gameObject.SetActive(false);
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
        //In shooting script, bool oneHit is set to false when a bullet is spawned. 
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
            if (oneHit == false)
            {
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= Health.bullet;
                oneHit = true;
            }
        };

        gameObject.SetActive(false);
    }

}
