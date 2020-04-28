using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Advertisements;

public class GameState : MonoBehaviour
{
    public Text HowToPlay1;
    public Text HowToPlay2;

    private string key;

    // Start is called before the first frame update
    void Start()
    {
        key = "GameState4";

        if (PlayerPrefs.GetInt(key) < 2)
        {
            StartCoroutine(displayInstructions1());

            int state = PlayerPrefs.GetInt(key);
            state++;
            PlayerPrefs.SetInt(key, state);
        }

        if (PlayerPrefs.GetInt(key) == 1)
        {
            PlayerPrefs.SetFloat("Music", 0.9f);
            PlayerPrefs.SetFloat("Sound", 0.9f);
        }


        Advertisement.Initialize("3577863", true);
    }

    private IEnumerator displayInstructions1()
    {
        yield return new WaitForSeconds(0.4f);
        HowToPlay2.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);

        HowToPlay2.transform.GetComponent<Text>().text = "Tap farther away from the cannon to shoot longer distances.";
        yield return new WaitForSeconds(5f);

        HowToPlay1.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        HowToPlay1.gameObject.SetActive(false);
        HowToPlay2.gameObject.SetActive(false);
    }
}
