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
    private SpriteRenderer renderer;
    private Collider2D collider;
    
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
        renderer = transform.GetComponent<SpriteRenderer>();

        if (transform.GetComponent<PolygonCollider2D>())
            collider = transform.GetComponent<PolygonCollider2D>();
        else 
            collider = transform.GetComponent<CircleCollider2D>();
    }

    void Update()
    {
        p = transform.position;
        inBounds = p.x > minX && p.y > minY && p.x < maxX;
        
        //Launch the bullet in the direction it was aimed 
        if (oneLaunch == false)
        {
            rig.velocity = new Vector2(0, 0);
            rig.gravityScale = 2;
            rig.AddForce(-transform.up * speed * shooting.touchPercent);

            syncRotation = false;
            Invoke("enableRotation", 0.03f);
            
            oneEnemyHit = false;
            oneLaunch = true;

            renderer.color = new Color32(255, 255, 255, 255);
            collider.enabled = true;
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

                //show the dmg recieved in a text popup
                Enemy_Health e = col.gameObject.transform.GetComponent<Enemy_Health>();
                e.floatText("40".ToString(), Color.white);

                //show any headshots in a text popup 
                if (transform.eulerAngles.z > 180f && transform.eulerAngles.z < 220f)
                    e.floatText("Headshot", new Color32(28, 75, 123, 255));

                //dmg that enemy and then turn off the bullet
                col.gameObject.transform.GetComponent<Enemy_Health>().hp -= 40;
                transform.gameObject.SetActive(false);
            }
        }

        //collided with the ground
        if (col.gameObject.layer == 14) {
            syncRotation = false;
            collider.enabled = false;

            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);

            StartCoroutine(fade());
        }
    }

    private IEnumerator fade() {
        byte alpha = 255;
        while (alpha > 5) {
            alpha -= 5;
            renderer.color = new Color32(255, 255, 255, alpha);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);        
    }

    private void enableRotation() {        
        syncRotation = true;
    }
} 
