using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Reaper_3 : MonoBehaviour
{
    Animator animator;
    Rigidbody2D rig;
    private float speed;
    private bool switchDir = false;

    private float height_Add;
    private float speed_y = 0.04f;
    private float height;

    private float x = 0;

    private float bound = 7.74f;
    private bool counter = false;
    private float ampMultiplier = 1;

    // Start is called before the first frame update
    void Start()
    {
        speed = UnityEngine.Random.Range(2.0f, 3.0f);
        height_Add = UnityEngine.Random.Range(-0.2f, 0.1f);
        if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
        {
            speed *= -1;
            bound = -7.74f;
            transform.rotation = Quaternion.Euler(0, 180, 0);
        }
        animator = transform.GetComponent<Animator>();
        rig = transform.GetComponent<Rigidbody2D>();
        rig.velocity = new Vector2(speed, 0);
        StartCoroutine(varyHeight());
    }

    private IEnumerator varyHeight()
    {
        if (counter == false) {
            x += 0.02f;
            if (x > 1)
            {
                counter = true;
                speed = UnityEngine.Random.Range(1.4f, 3.0f);
                ampMultiplier = UnityEngine.Random.Range(0.6f, 1.15f);
            }
        }
        if (counter == true)
        {
            x -= 0.02f;
            if (x < 0)
            {
                counter = false;
                speed = UnityEngine.Random.Range(1.4f, 3.0f);
                ampMultiplier = UnityEngine.Random.Range(0.6f, 1.15f);
            }
        }        
        yield return new WaitForSeconds(speed_y);
        height = ampMultiplier * (float) Math.Sin(Math.PI * x) + 1.83f;
        height += height_Add;
        transform.position = new Vector2(transform.position.x, height);
        StartCoroutine(varyHeight());
    }

    // Update is called once per frame
    void Update()
    {     
        if (bound == 7.74f && transform.position.x > 7.74f)
        {
            bound = -7.74f;
            rig.velocity = new Vector2(-speed, 0);
        }

        if (bound == -7.74f && transform.position.x < -7.74f)
        {
            bound = 7.74f;
            rig.velocity = new Vector2(speed, 0);
        }

        if (transform.position.x > GameObject.FindGameObjectWithTag("Player").transform.position.x)
            transform.rotation = Quaternion.Euler(0, 180, 0);

        if (transform.position.x < GameObject.FindGameObjectWithTag("Player").transform.position.x)
            transform.rotation = Quaternion.Euler(0, 0, 0);
    }
}
