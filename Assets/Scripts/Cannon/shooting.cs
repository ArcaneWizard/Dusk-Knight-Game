﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{

    public List<GameObject> ammo;
    public Transform muzzle;
    private float cooldown;
    private float startcooldown=2.5f;
    private int counter = 0;

    void Start()
    {
        Vector3 offscreen = new Vector3(100, 100, 100);
        for (int i = 0; i< 8; i++ ){
            ammo.Add(transform.parent.transform.GetChild(1).transform.GetChild(i).gameObject);
        }
        cooldown = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.touchCount > 0)
        {
            Touch touch = Input.GetTouch(0);
            Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position) - transform.position;
            float rot = Mathf.Atan2(touchPosition.y, touchPosition.x) * Mathf.Rad2Deg;
            transform.rotation = Quaternion.Euler(0f, 0f, rot);
            if (cooldown <= 0)
            {
                cooldown = startcooldown;
                Fire(rot);
            }
            else
            {
                cooldown -= Time.deltaTime;
            }
            
        }
        cooldown -= Time.deltaTime;
    }

    void Fire(float rot)
    {
        ammo[counter].transform.position = muzzle.position;
        ammo[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
        ammo[counter].SetActive(true);
        if (counter < 7)
        {
            counter += 1;
        }
        else
        {
            counter = 0;
        }
    }
}