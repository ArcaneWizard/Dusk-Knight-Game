﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Boulder : MonoBehaviour
{
    Vector2 dir;
    [HideInInspector]
    public bool switchBoulders = false;
    public string side = "right";

    // Start is called before the first frame update
    void Update()
    {
        if (switchBoulders == true)
        {
            if (side == "left")
                dir = new Vector2(200, 300);
            else
                dir = new Vector2(-200, 300);
            transform.GetComponent<Rigidbody2D>().AddForce(dir);
            switchBoulders = false;
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        if (col.gameObject.layer == 10 || col.gameObject.layer == 22)
           gameObject.SetActive(false);
    }
}
