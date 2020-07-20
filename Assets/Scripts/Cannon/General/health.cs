using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class health : MonoBehaviour
{
    private float maxHp;
    public static float hp;

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
    }
}
