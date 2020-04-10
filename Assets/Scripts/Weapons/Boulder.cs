using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    Vector2 dir;
    [HideInInspector]
    public bool switchBoulders = false;

    // Start is called before the first frame update
    void Update()
    {
        if (switchBoulders == true)
        {
            dir = new Vector2(150, 400);
            transform.GetComponent<Rigidbody2D>().AddForce(dir);
            switchBoulders = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 10)
        {
           if (gameObject.tag == "Boulder")
               //do dmg to player

           gameObject.SetActive(false);
        }

        if (col.gameObject.layer == 22)
        {
            Debug.Log("hit ground");
            gameObject.SetActive(false);
        }
    }
}
