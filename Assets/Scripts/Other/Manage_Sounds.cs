using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Sounds : MonoBehaviour
{
    public AudioClip enemyHit;
    public AudioClip cannonShot;
    public AudioClip flamesound;
    public AudioClip potionshot;
    public AudioClip potionhit;
    public AudioClip explode;
    public AudioClip arrowshot;
    public AudioClip arrowhit;
    public AudioClip gatling;

    public AudioClip ogreAttack; //boulder "throw" sound needed
    public AudioClip orcAttack;
    public AudioClip goblinAttack;
    public AudioClip R1Attack; //woosh throw sound needed
    public AudioClip R2Attack;
    public AudioClip R3Attack; //woosh throw sound needed

    public AudioClip orbConnect;
    public AudioClip boulderConnect;

    public AudioClip buttonClick;
    public AudioClip errorPurchase;
    public AudioClip purchase;

    public static Manage_Sounds Instance { get; private set; }
    public static float soundMultiplier = 1f;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
    }

    void example()
    {
        // I added the second volume parameter to all weapons in the shooting script but add it to all other weapon sounds you implemented
        transform.GetComponent<AudioSource>().PlayOneShot(Manage_Sounds.Instance.arrowhit, 0.7f * Manage_Sounds.soundMultiplier);
    }

    //For boulder and orb collisions with the tower
    public void playHitSound(AudioClip clip, float volume)
    {
        transform.GetComponent<AudioSource>().PlayOneShot(clip, volume * Manage_Sounds.soundMultiplier);
    }

}
