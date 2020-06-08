using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;
using UnityEngine.Rendering;

public class player_bullet : MonoBehaviour
{
    //Bullet characteristics or modifiers
    public float speed = 3500f;
    public bool oneLaunch = false;
    private bool oneEnemyHit, syncRotation;

    //Bounds detection 
    Vector3 bottomLeft, topRight, p;
    float minX, minY, maxX, maxY;
    private bool inBounds;

    //Objects used
    public Camera camera;
    private Rigidbody2D rig;
    
    void Start() {
       //Get bounds of the screen for any screen size
        bottomLeft = camera.ViewportToWorldPoint(new Vector2(0,0));
        topRight = camera.ViewportToWorldPoint(new Vector2(1,1));

        minX = bottomLeft.x - 1;
        minY = bottomLeft.y - 1;
        maxX = topRight.x + 1;
        maxY = topRight.y + 1;

        //Define components
        rig = transform.GetComponent<Rigidbody2D>();
    }

    void Update()
    {
        p = transform.position;
        inBounds = p.x > minX && p.y > minY && p.x < maxX;
        
        //Launch the bullet in the direction it was aimed 
        if (oneLaunch == false)
        {
            rig.velocity = new Vector2(0, 0);
            rig.AddForce(-transform.up * speed * shooting.touchPercent);

            syncRotation = false;
            Invoke("enableRotation", 0.03f);
            
            oneEnemyHit = false;
            oneLaunch = true;
        }
        
        //check when/if the bullet exits the screen bounds
        if (!inBounds)
            transform.gameObject.SetActive(false);

        //sync bullet rotation with how it travels through the air
        if (syncRotation == true)
           transform.up = rig.velocity.normalized;
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        //collided with an enemy
        if (col.gameObject.layer == 8 || col.gameObject.layer == 9 || col.gameObject.layer == 11 || col.gameObject.layer == 19 || col.gameObject.layer == 20 || col.gameObject.layer == 21)
        {
            //If multiple enemies are hit, only damage one
            if (oneEnemyHit == false)
            {
                oneEnemyHit = true;

                //turn on floating text popup for enemy hit
                col.gameObject.transform.GetComponent<Enemy_Health>().floatText(Health.CB);

                //dmg that enemy and then turn off the bullet
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= Health.CB;
                transform.gameObject.SetActive(false);
            }
        }

        //collided with the ground
        if (col.gameObject.layer == 22)
            transform.gameObject.SetActive(false);
    }

    private void enableRotation() {        
        syncRotation = true;
    }
}
