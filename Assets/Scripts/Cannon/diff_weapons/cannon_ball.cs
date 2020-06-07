using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class cannon_ball : MonoBehaviour
{
    public float speed = 3500f;
    public bool oneLaunch = false;

    Vector3 bottomLeft, topRight, p;
    float minX, minY, maxX, maxY;
    bool inBounds;

    public Camera camera;
    
    void Start() {
       //Get bounds of the screen for any screen size
        bottomLeft = camera.ViewportToWorldPoint(new Vector2(0,0));
        topRight = camera.ViewportToWorldPoint(new Vector2(1,1));

        minX = bottomLeft.x - 1;
        minY = bottomLeft.y - 1;
        maxX = topRight.x + 1;
        maxY = topRight.y + 1;
    }

    void Update()
    {
        p = transform.position;
        inBounds = p.x > minX && p.y > minY && p.x < maxX && p.y < maxY;
        
        //Launch the bullet in the direction it was aimed 
        if (oneLaunch == false)
        {
            transform.GetComponent<Rigidbody2D>().velocity = new Vector2(0, 0);
            transform.GetComponent<Rigidbody2D>().AddForce(transform.up * speed * shooting.touchPercent);
            oneLaunch = true;
        }
        
        //check when/if the bullet exits the screen bounds
        if (!inBounds)
            transform.gameObject.SetActive(false);
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //collided with an enemy
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
             col.gameObject.transform.GetComponent<Enemy_Health>().hp -= Health.CB;
             transform.gameObject.SetActive(false);
        }

        //collided with the ground
        if (col.gameObject.layer == 22)
            transform.gameObject.SetActive(false);
    }
}
