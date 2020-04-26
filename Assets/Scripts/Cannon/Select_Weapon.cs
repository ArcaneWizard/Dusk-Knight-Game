using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Select_Weapon : MonoBehaviour
{
    // Start is called before the first frame update
    public bool weaponChange = false;

    void Start()
    {
        GameObject.Find("Head").transform.GetChild(2).gameObject.SetActive(true);
    }

    //add new select[Weapon] methods here:
    public void selectGrenade()
    {
        unselectEverything();
        GameObject.Find("Head").transform.GetChild(0).gameObject.SetActive(true);
    }

    public void selectBullet()
    {
        unselectEverything();
        GameObject.Find("Head").transform.GetChild(1).gameObject.SetActive(true);
    }
    public void selectCannonBall()
    {
        unselectEverything();
        GameObject.Find("Head").transform.GetChild(2).gameObject.SetActive(true);
    }
    public void selectPotion()
    {
        unselectEverything();
        GameObject.Find("Head").transform.GetChild(3).gameObject.SetActive(true);
    }
    public void selectArrow()
    {
        unselectEverything();
        GameObject.Find("Head").transform.GetChild(4).gameObject.SetActive(true);
        GameObject.Find("Head").transform.GetChild(4).transform.GetComponent<shooting>().loaded = false;
    }

    public void selectFlame()
    {
        unselectEverything();
        GameObject.Find("Head").transform.GetChild(5).gameObject.SetActive(true);
    }

    //make sure to turn the new weapon off in this method:
    void unselectEverything()
    {
        GameObject.Find("Head").transform.GetChild(0).gameObject.SetActive(false);
        GameObject.Find("Head").transform.GetChild(1).gameObject.SetActive(false);
        GameObject.Find("Head").transform.GetChild(2).gameObject.SetActive(false);
        GameObject.Find("Head").transform.GetChild(3).gameObject.SetActive(false);
        GameObject.Find("Head").transform.GetChild(4).gameObject.SetActive(false);
        GameObject.Find("Head").transform.GetChild(5).gameObject.SetActive(false);
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
        for (int i = 0; i < GameObject.FindGameObjectWithTag("Arrow").transform.childCount; i++)
            GameObject.FindGameObjectWithTag("Arrow").transform.GetChild(i).gameObject.SetActive(false);
    }
}
