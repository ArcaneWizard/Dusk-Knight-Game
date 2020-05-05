using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FlameThrower : MonoBehaviour
{

    public List<GameObject> flame;
    public static float touchPercent;
    private bool fwishing = false;
    private bool playsound = true;

    void Start()
    {
        gameObject.AddComponent<AudioSource>();
        for (int i = 0; i < transform.childCount; i++)
            flame.Add(transform.GetChild(i).gameObject);
    }

    // Update is called once per frame
    void Update()
    {

         Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();

        if (Input.touchCount > 0)
            StartCoroutine(checkForWeaponChange(m));

        else
        {  
            transform.GetComponent<AudioSource>().Stop();
            
            fwishing = false;
            StopAllCoroutines();
            foreach (GameObject i in flame)
            {
                i.SetActive(false);
            }
        }

    }

    private IEnumerator checkForWeaponChange(Manage_Sounds m)
    {
        yield return new WaitForSeconds(0.01f);
        if (GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Select_Weapon>().weaponChange == false)
        {
            Touch touch = Input.GetTouch(0);
            Vector3 touchPosition = Camera.main.ScreenToWorldPoint(touch.position) - transform.position;
            float rot = Mathf.Atan2(touchPosition.y, touchPosition.x) * Mathf.Rad2Deg;
            StartCoroutine(addSmallDelayCheck(rot));
            transform.GetComponent<AudioSource>().PlayOneShot(m.flamesound, 0.8f * Manage_Sounds.soundMultiplier);
        }
    }

    //DESCRIPTION -------------------------------------------
    //check if the tap was done to switch weapons or not
    //-------------------------------------------------------
    private IEnumerator addSmallDelayCheck(float rot)
    {
        yield return new WaitForSeconds(0.01f);

        if (GameObject.FindGameObjectWithTag("Player").transform.GetComponent<Select_Weapon>().weaponChange == false)
        {
            transform.rotation = Quaternion.Euler(0f, 0f, rot - 90);

            if (!fwishing)
            {
                StartCoroutine(Fwish());
                fwishing = true;
            }
        }
    }

    private IEnumerator Fwish()
    {
        foreach(GameObject i in flame)
        {
            i.SetActive(true);
            yield return new WaitForSeconds(0.05f);
            i.SetActive(false);
        }
        flame[7].SetActive(true);
    }

    /*private IEnumerator Fwoosound()
    {
        Debug.Log("sound playing");
        Manage_Sounds m = GameObject.Find("Sound Manager").transform.GetComponent<Manage_Sounds>();
        transform.GetComponent<AudioSource>().PlayOneShot(m.flamesound, 0.8f * Manage_Sounds.soundMultiplier);
        yield return new WaitForSeconds(1f);
        playsound = true;
        print(playsound);
    }*/

}