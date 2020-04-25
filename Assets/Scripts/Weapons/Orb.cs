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
            if (gameObject.name == "witch orbs")
                transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 5f + new Vector2(0, UnityEngine.Random.Range(-1.5f, 1.5f));
            else if (gameObject.name == "blue orbs")
                transform.GetComponent<Rigidbody2D>().velocity = dir.normalized * 5f + new Vector2(0, UnityEngine.Random.Range(0, 4));
            else
                Debug.Log("orbs' name was changed. Error in the Orb script");

            switchOrbs = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 10)
        {
            gameObject.SetActive(false);
        }
    }
}
