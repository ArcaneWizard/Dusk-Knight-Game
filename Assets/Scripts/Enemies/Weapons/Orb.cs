using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private Vector2 dir;
    private float randomX;
    private float randomY;

    [HideInInspector]
    public bool setupOnce;

    private AudioSource audioSource;
    private Rigidbody2D rig;
    private SpriteRenderer renderer;
    private Collider2D collider;
    public float volume;

    public GameObject tower;
    public float speed = 5f;
    public float variance = 2f;

    void Update()
    {
        //When the orb is shot out
        if (setupOnce)
        {   
            //perform this setup only once
            setupOnce = false;

            //define components
            audioSource = transform.GetComponent<AudioSource>();
            rig = transform.GetComponent<Rigidbody2D>();
            renderer = transform.GetComponent<SpriteRenderer>();
            collider = transform.GetComponent<CircleCollider2D>();

            //reset color, collider and gravity
            renderer.color = new Color32(255, 26, 26, 255);            
            collider.enabled = true;
            rig.gravityScale = 3;

            //set direction and velocity of orb
            dir = tower.transform.position - transform.position;
            dir = new Vector2(dir.x / 1.4f, 0);

            //add randomness to the direction, with a bias towards undershooting
            randomX = UnityEngine.Random.Range(-2.5f * variance, 2 * variance) / 3f;
            if (randomX >= -1.7f && randomX <= -0.83f)
            {
                randomX = UnityEngine.Random.Range(-2.5f * variance, 0) / 3f;
            }
            randomY = UnityEngine.Random.Range(-variance, variance) / 3f;

            rig.velocity = dir * speed + new Vector2(dir.x * randomX, dir.y * randomY);

            //play cast orb sound effect
            audioSource.PlayOneShot(Manage_Sounds.Instance.R1Attack, volume * Manage_Sounds.soundMultiplier);
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //damage the player if they connect (+ audio)
        if (col.gameObject.layer == 10)
        {
            Manage_Sounds.Instance.playHitSound(Manage_Sounds.Instance.orbConnect, 0.4f);
            gameObject.SetActive(false);
        }
    }

    void OnTriggerEnter2D(Collider2D col) {

        //collides with ground
        if (col.gameObject.layer == 15) {
            collider.enabled = false;
            rig.gravityScale = 0;
            rig.velocity = new Vector2(0, 0);

            gameObject.SetActive(false);
        }
    }

    private IEnumerator fade() {
        byte alpha = 255;
        while (alpha > 12) {
            alpha -= 12;
            renderer.color = new Color32(255, 26, 26, alpha);
            yield return new WaitForSeconds(0.1f);
        }
        gameObject.SetActive(false);        
    }
}
