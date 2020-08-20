using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class state : MonoBehaviour
{
    
    [Space(10)]
    [Header("Knight effects")]

    public float effectDuration = 0.8f;
    public float missedShotDmg = 10;
    public float darkPain = 3;
    public float lightRegen = 2;
    public float darkDmgMultiplier = 2;
    public float lightDmgMultiplier = 1;
    public float ragePerSecond = 2;

    public static string knightState;
    public static bool missedShot;

    [Space(10)]
    [Header("Sprites")]  

    public Sprite darkToken;
    public Sprite lightToken;

    public Sprite darkKnightArmor;
    public Sprite darkKnightHands;
    public Sprite lightKnightArmor;
    public Sprite lightKnightHands;

    [Space(10)]
    [Header("In-game components")]  

    public SpriteRenderer knightArmor;
    public SpriteRenderer knightHands;
    public Image stateToken;
    public Image rage_token;

    [Space(10)]
    [Header("Particle effects")]  

    public ParticleSystem darkEffect;
    public ParticleSystem lightEffect;

    void Awake() 
    {
        //start on Light Knight state
        knightState = "Light";
        updateStateToken();
        updatePhysicalAppearance();

        //disable particle effects 
        darkEffect.Stop();
    }

    //Dark knight state effects
    void Update() 
    {
        //check rage progression and change button accordingly
        if (shooting.rage_count >= shooting.max_count / 3 && shooting.rage_count < 2 * shooting.max_count / 3)
        {
            rage_token.enabled = true;
        }
        else if (shooting.rage_count >= 2*shooting.max_count / 3 && shooting.rage_count < shooting.max_count)  
        {
            rage_token.enabled = true;
        }
        else if (shooting.rage_count >= shooting.max_count)
        {
            rage_token.enabled = true;
        }
        else if (shooting.rage_count < shooting.max_count / 3)
        {
            rage_token.enabled = false;
        }

        //Dark knight state: lose 2 hp/sec 
        if (knightState == "Dark") {
            if (health.hp > 0)
                health.hp -= Time.deltaTime * darkPain;

            player_bullet.dmgMultiplier = darkDmgMultiplier;
            
            if (shooting.rage_count < shooting.max_count) 
                shooting.rage_count += Time.deltaTime * ragePerSecond;
        }

        else {
            if (health.hp < health.maxHp)
                health.hp += Time.deltaTime * lightRegen;
                
            player_bullet.dmgMultiplier = lightDmgMultiplier;
        } 

        //Dark knight state: loss hp when you miss a shot
        if (missedShot) {
            missedShot = false;
            
            if (knightState == "Dark")
                health.hp -= missedShotDmg;
        }

    }

    //change the "state" of the knight when the knight's token is pressed
    public void swapStates() 
    {
        knightState = (knightState == "Light") ? "Dark" : "Light";
        updateStateToken();
        updatePhysicalAppearance();
        StartCoroutine(spawnEffect());
    }

    //update the UI knight token
    private void updateStateToken() 
    {
        stateToken.sprite = (knightState == "Light") ? lightToken : darkToken;
    }

    //update the armor and hands of the knight
    private void updatePhysicalAppearance()
    {
        knightArmor.sprite = (knightState == "Light") ? lightKnightArmor : darkKnightArmor;
        knightHands.sprite = (knightState == "Light") ? lightKnightHands : darkKnightHands;
    }

    //play particle effect for the knight swap
    private IEnumerator spawnEffect() {
        if (knightState == "Light") {
            darkEffect.Stop();
        }
        else {
            darkEffect.Play();
        }
        
        yield return new WaitForSeconds(effectDuration);
        darkEffect.Stop();
    }
}