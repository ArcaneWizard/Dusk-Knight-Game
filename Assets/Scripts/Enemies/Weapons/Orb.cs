using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Orb : MonoBehaviour
{
    private Vector2 dir;

    private AudioSource audioSource;
    private Rigidbody2D rig;
    public GameObject tower;

    void Start()
    {
        //define components
        audioSource = transform.GetComponent<AudioSource>();
        rig = transform.GetComponent<Rigidbody2D>();

        //set direction and velocity of orb
        dir = tower.transform.position - transform.position;
        rig.velocity = dir.normalized * 5f + new Vector2(0, UnityEngine.Random.Range(-1.5f, 1.5f));

        //play cast orb sound effect
        audioSource.PlayOneShot(Manage_Sounds.Instance.R1Attack, 0.12f * Manage_Sounds.soundMultiplier);
    }

    void OnCollisionEnter2D(Collision2D col)
    {
        //disable orbs that hit the ground
        if (col.gameObject.layer == 22)
            gameObject.SetActive(false);

        //damage the player if they connect (+ audio)
        if (col.gameObject.layer == 10)
        {
            Health.playerHP -= Mathf.RoundToInt(Health.R3Dmg);
            Manage_Sounds.Instance.playHitSound(Manage_Sounds.Instance.orbConnect, 0.4f);
            gameObject.SetActive(false);
        }
    }
}
