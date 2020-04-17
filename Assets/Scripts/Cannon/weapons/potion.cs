using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class potion : MonoBehaviour
{
    private float speed = 3f;
    private bool stop = false;
    public bool oneHit = false;
    public bool oneLaunch = false;

    private Rigidbody2D rig;

    void Start()
    {
        rig = transform.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        //Set bounds + reset settings when its being chosen from the array
        if ((Mathf.Abs(transform.position.x) < 10f || Mathf.Abs(transform.position.y) < 9f) && stop == false)
        {
            if (oneLaunch == false)
            {
               rig.velocity = new Vector2(0, 0);
               rig.AddForce(transform.up * speed * 400 * shooting.touchPercent);
               oneLaunch = true;
            }
        }

        else if (stop == false)
            transform.gameObject.SetActive(false);

        float rot = Mathf.Atan2(rig.velocity.y, rig.velocity.x) * Mathf.Rad2Deg;
         transform.rotation = Quaternion.Euler(0f, 0f, 0);

        //Stop moving
        if (stop == true)
            rig.velocity = new Vector2(0, 0);

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

        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
            if (oneHit == false)
            {
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= 20;
                col.gameObject.transform.GetComponent<Enemy_Health>().poison = true;
                col.gameObject.transform.GetComponent<Enemy_Health>().isPoisoned = false;
                oneHit = true;
            }
        }

        stop = false;
        transform.gameObject.SetActive(false);
    }
}
