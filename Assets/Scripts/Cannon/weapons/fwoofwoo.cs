using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class fwoofwoo : MonoBehaviour
{
    private float cooldown;
    private float startcool = 0.6f;

    // Start is called before the first frame update
    void Start()
    {
        cooldown = 0;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        cooldown -= Time.fixedDeltaTime;
        if (cooldown < 0)
        {
            Debug.Log("bla");
            //cooldown = startcool;
        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
            if (cooldown <= 0)
            {
                Debug.Log("hit");
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= 20;
            }
        }
    }
}
