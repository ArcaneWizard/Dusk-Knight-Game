using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class cannon_ball : MonoBehaviour
{
    private float speed = 2.7f;
    private bool stop = false;
    public bool oneHit = false;
    public bool oneLaunch = false;

    void Update()
    {
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
        
        if (transform.GetComponent<Rigidbody2D>().velocity != new Vector2(0, 0))
        {
            gameObject.transform.GetComponent<SpriteRenderer>().enabled = true;
            for (int i = 0; i < gameObject.transform.childCount; i++)
                gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
        }
    }


    private void OnTriggerEnter2D(Collider2D col)
    {
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
        transform.gameObject.SetActive(false);
    }
}
