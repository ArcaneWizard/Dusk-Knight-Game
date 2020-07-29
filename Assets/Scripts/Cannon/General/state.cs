using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class state : MonoBehaviour
{
    public static string knightState;

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

    [Space(10)]
    [Header("Particle effects")]  

    public ParticleSystem darkEffect;
    public ParticleSystem lightEffect;

    public float effectDuration;

    void Awake() 
    {
        //start on Light mode always
        knightState = "Light";
        updateStateToken();
        updatePhysicalAppearance();

        //disable particle effects at the beginning
        lightEffect.Stop();
        darkEffect.Stop();
    }

    //dictate what happens if you're in a certain knight mode
    void Update() 
    {
        //lose 2 hp/sec while in Dark knight mode
        if (knightState == "Dark") {
            if (health.hp > 0)
                health.hp -= Time.deltaTime * 3f;

            player_bullet.dmgMultiplier = 2f;
        }

        else {
            if (health.hp < health.maxHp)
                health.hp += Time.deltaTime * 2f;
                
            player_bullet.dmgMultiplier = 1f;
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
            lightEffect.Play();
        }
        else {
            lightEffect.Stop();
            darkEffect.Play();
        }
        
        yield return new WaitForSeconds(effectDuration);
        lightEffect.Stop();
        darkEffect.Stop();
    }
}