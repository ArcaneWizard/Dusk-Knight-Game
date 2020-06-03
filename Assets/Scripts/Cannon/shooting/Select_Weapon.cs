using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select_Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public bool weaponChange = false;
    public GameObject cannon;
    public GameObject grenadier;
    public GameObject gatlingGun;
    public GameObject potionCrafter;
    public GameObject ballista;
    public GameObject flamethrower;

    public GameObject Arrows;

    void Start()
    {
       cannon.gameObject.SetActive(true);
    }

    //add new select[Weapon] methods here:
    public void selectGrenade()
    {
        unselectEverything();
        grenadier.gameObject.SetActive(true);
    }

    public void selectBullet()
    {
        unselectEverything();
        gatlingGun.gameObject.SetActive(true);
    }
    public void selectCannonBall()
    {
        unselectEverything();
        cannon.gameObject.SetActive(true);
    }
    public void selectPotion()
    {
        unselectEverything();
        potionCrafter.gameObject.SetActive(true);
    }
    public void selectArrow()
    {
        unselectEverything();
        ballista.gameObject.SetActive(true);
        ballista.transform.GetComponent<shooting>().loaded = false;
    }

    public void selectFlame()
    {
        unselectEverything();
        flamethrower.gameObject.SetActive(true);
    }

    //make sure to turn the new weapon off in this method:
    void unselectEverything()
    {
        cannon.gameObject.SetActive(false);
        ballista.gameObject.SetActive(false);
        flamethrower.gameObject.SetActive(false);
        potionCrafter.gameObject.SetActive(false);
        grenadier.gameObject.SetActive(false);
        gatlingGun.gameObject.SetActive(false);
    }

    private IEnumerator weaponChanged()
    {
        weaponChange = true;
        yield return new WaitForSeconds(0.1f);
        weaponChange = false;
    }

    public void dbug()
    {
        StartCoroutine(weaponChanged());
        for (int i = 0; i < Arrows.transform.childCount; i++)
            Arrows.transform.GetChild(i).gameObject.SetActive(false);
    }
}
