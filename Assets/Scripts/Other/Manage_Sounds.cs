using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Manage_Sounds : MonoBehaviour
{
    public AudioClip enemyHit; //happy
    public AudioClip cannonShot; //happy
    public AudioClip flamesound; 
    public AudioClip potionshot; //happy
    public AudioClip potionhit; //happy
    public AudioClip explode; //happy
    public AudioClip arrowshot; //happy
    public AudioClip arrowhit; //happy
    public AudioClip gatling; //happy

    public AudioClip orcAttack; //happy
    public AudioClip goblinAttack; //happy
    public AudioClip R1Attack; //happy
    public AudioClip R2Attack; //happy
    public AudioClip R3Attack; //happy

    public AudioClip orbConnect;  //happy
    public AudioClip boulderConnect; //happy

    public AudioClip buttonClick; //happy
    public AudioClip errorPurchase; //happy
    public AudioClip purchase; //happy

    public static Manage_Sounds Instance { get; private set; }
    public static float soundMultiplier = 1f;

    void Awake()
    {
        if (Instance == null) { Instance = this; }
    }
    
    void Update()
    {
        soundMultiplier = PlayerPrefs.GetFloat("Sound");
    }

    void example()
    {
        // I added the second volume parameter to all weapons in the shooting script but add it to ALL other weapon sounds you implemented
        transform.GetComponent<AudioSource>().PlayOneShot(Manage_Sounds.Instance.arrowhit, 0.7f * Manage_Sounds.soundMultiplier);
    }

    //For boulder and orb collisions with the tower
    public void playHitSound(AudioClip clip, float volume)
    {
        transform.GetComponent<AudioSource>().PlayOneShot(clip, volume * Manage_Sounds.soundMultiplier);
    }

}
