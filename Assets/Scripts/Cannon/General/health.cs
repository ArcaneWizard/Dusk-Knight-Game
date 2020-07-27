using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class health : MonoBehaviour
{
    private float maxHp;
    public static float hp;
    private bool dead;

    public float shakeVariance;
    public float shakeRate;
    public float shakeDuration;

    public float fallDownSpeed;
    public float reloadSceneDelay;

    private bool shakingIsOver;

    public bool killTower;

    public Image healthBar;

    void Awake() {
        //get the starting hp of the tower
        maxHp = 100;

        //set the tower's hp to that
        hp = maxHp;
    }

    void Update() {
        //set the healthBar fill to represent the tower's % hp left
        healthBar.fillAmount = hp / maxHp;

        //collapse the tower and restart when dead
        if (hp <= 0 && !dead) { 
            dead = true;
            StartCoroutine(towerCollapse());
        }
        
        if (killTower) {
            killTower = false;
            hp = 0;
        }

        if (dead && !shakingIsOver) 
            transform.localPosition = new Vector2(-10.49f, -3.57f + Mathf.Sin(Time.time * shakeRate)  * shakeVariance);
            
        if (shakingIsOver) 
            transform.Translate(new Vector2(0, -fallDownSpeed * Time.deltaTime));
    }

    private IEnumerator towerCollapse() {
        yield return new WaitForSeconds(shakeDuration);
        shakingIsOver = true;
        yield return new WaitForSeconds(reloadSceneDelay);
        SceneManager.LoadScene(0);
    }
}
