using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameState : MonoBehaviour
{
    public Text HowToPlay1;
    public Text HowToPlay2;

    // Start is called before the first frame update
    void Start()
    {
        if (PlayerPrefs.GetInt("GameState") < 2)
        {
            StartCoroutine(displayInstructions1());

            int state = PlayerPrefs.GetInt("GameState");
            state++;
            PlayerPrefs.SetInt("GameState", state);
        }
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
