using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class health : MonoBehaviour
{
    public static float maxHp;
    public static float hp;
    private bool dead;
    private float lastHp;

    public float shakeVariance;
    public float shakeRate;
    public float shakeDuration;

    public float fallDownSpeed;
    public float reloadSceneDelay;

    private bool shakingIsOver;

    public bool killTower;

    public Image healthBar;
    public cameraShake shake;

    void Awake() {
        maxHp = 110;
        hp = maxHp;
        lastHp = maxHp;
    }

    void Update() {
        //set the healthBar fill to represent the tower's % hp left
        healthBar.fillAmount = hp / maxHp;

        //collapse the tower and restart when dead
        if (hp <= 0 && !dead) { 
            dead = true;
            StartCoroutine(towerCollapse());
        }

        //cannot go over 100% of your health
        if (hp > maxHp)
            hp = maxHp;
        
        //add camera shake when hurt
        if (lastHp > hp) {
            lastHp = hp;
            StartCoroutine(shake.Shake(0.1f, 0.2f));
        }
        
        //this bool can be checkmarked in the editor to test killing the tower
        if (killTower) {
            killTower = false;
            hp = 0;
        }

        //shaking effect when the player loses all their hp
        if (dead && !shakingIsOver) 
            transform.localPosition = new Vector2(-10.49f, -3f + Mathf.Sin(Time.time * shakeRate)  * shakeVariance);
        
        //falling effect after the tower has been shook
        if (shakingIsOver) 
            transform.Translate(new Vector2(0, -fallDownSpeed * Time.deltaTime));
    }

    //times the shaking, falling down and reloading of the level
    private IEnumerator towerCollapse() {
        yield return new WaitForSeconds(shakeDuration);
        shakingIsOver = true;
        yield return new WaitForSeconds(reloadSceneDelay);
        SceneManager.LoadScene(0);
    }
}
