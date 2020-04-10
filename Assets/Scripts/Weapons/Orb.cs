using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    Vector2 dir;
    [HideInInspector]
    public bool switchOrbs = false;

    // Start is called before the first frame update
    void Update()
    {
        if (switchOrbs == true)
        {
            dir = GameObject.FindGameObjectWithTag("Player").transform.position - transform.position;
            transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 5f;
            switchOrbs = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 10)
        {
            if (gameObject.tag == "Reaper orb")
            //do dmg to player
            if (gameObject.tag == "Witch orb")
            //do dmg to player

            gameObject.SetActive(false);
        }
    }
}
