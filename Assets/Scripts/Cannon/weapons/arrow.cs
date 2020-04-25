﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class arrow : MonoBehaviour
{
    private float speed = 3f;
    private bool stop = false;
    public bool oneHit = false;
    public bool oneLaunch = true;
    public bool launched = false;
    private float flip=0;

    private Rigidbody2D rig;
    private Transform arrowHead;
    

    void Start()
    {
        transform.GetChild(0).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 50);
        transform.GetChild(1).GetComponent<SpriteRenderer>().color = new Color32(255, 255, 255, 50);

        oneLaunch = true;
        rig = transform.GetComponent<Rigidbody2D>();
        arrowHead = GameObject.Find("Head").transform.GetChild(4).transform;
    }

    void Update()
    {
        if (arrowHead.rotation.y != 0)
            flip = -1;
        else
            flip = 1;

        //Set bounds + reset settings when its being chosen from the array
        if ((Mathf.Abs(transform.position.x) < 10f || Mathf.Abs(transform.position.y) < 9f) && !stop)
        {
            if (!oneLaunch)
            {
                rig.velocity = new Vector2(0, 0);
                rig.AddForce(transform.up * speed * 400 * shooting.touchPercent);
                oneLaunch = true;
                launched = true;
            }
        }

        //This bit of code makes the grenade (or arrow) rotate as it falls
        if (launched)
        {
            float rot = Mathf.Atan2(rig.velocity.y, rig.velocity.x) * Mathf.Rad2Deg;
            if (rig.velocity != new Vector2(0, 0))
                transform.rotation = Quaternion.Euler(0f, 0f, rot - 90f);
        }
        else 
        {
            transform.rotation = Quaternion.Euler(0f, (flip-1)*90, arrowHead.rotation.eulerAngles.z - 90);
            transform.position = arrowHead.GetChild(0).position;
        }

        //Stop moving + re-enable sprite (only after proper rotation)
        if (stop)
            rig.velocity = new Vector2(0, 0);

        if (launched == true)
        {
          //  for (int i = 0; i < gameObject.transform.childCount; i++)
          //      gameObject.transform.GetChild(i).GetComponent<SpriteRenderer>().enabled = true;
        }
        else
            gameObject.transform.GetChild(0).transform.GetComponent<SpriteRenderer>().enabled = false;

    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        stop = true;

        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
            if (oneHit == false)
            {
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= Health.arrow;
                col.gameObject.transform.GetComponent<Enemy_Health>().iced = true;
                oneHit = true;
            }
        }

        stop = false;
        transform.gameObject.SetActive(false);
        launched = false;
    }
}
