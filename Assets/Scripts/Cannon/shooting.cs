using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class shooting : MonoBehaviour
{

    public GameObject grenade;
    public Transform muzzle;
    private float cooldown;
    private float startcooldown=3;
    private int counter = 0;
    GameObject[] grenadearr = new GameObject[12];

    void Start()
    {
        Vector3 offscreen = new Vector3(100, 100, 100);
        for (int i = 0; i< 12; i++ ){
            GameObject ammo = Instantiate(grenade, offscreen, Quaternion.Euler(0f, 0f, 270f)) as GameObject;
            grenadearr[i] = ammo;
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
        grenadearr[counter].transform.position = muzzle.position;
        grenadearr[counter].transform.rotation = Quaternion.Euler(0f, 0f, rot + 270);
        if (counter < 11)
        {
            counter += 1;
        }
        else
        {
            counter = 0;
        }
    }
}
