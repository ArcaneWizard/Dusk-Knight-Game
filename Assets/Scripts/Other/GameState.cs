using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GameState : MonoBehaviour
{
    public Text HowToPlay1;
    public Text HowToPlay2;
    public Text scoreText;

    private string key;
    public static int time;
    public static int min;
    public static int sec;
    public static string s;

    private IEnumerator scoreIncreasesInTime()
    {
        yield return new WaitForSeconds(1);
        time++;
        if (Health.playerHP > 0)
        StartCoroutine(scoreIncreasesInTime());
    }


    // Start is called before the first frame update
    void Start()
    {
        key = "GameState6";

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

        time = 0;
        StartCoroutine(scoreIncreasesInTime());
    }

    void Update()
    {
        min = time / 60;
        sec = time % 60;
        s = sec.ToString();

        if (sec <= 9)
            s = "0" + sec;

        if (time == 60 || time == 300 || time == 600 || time == 1800 || (time % 3600 == 0 && time != 0))
            scoreText.fontSize = 80;
        else
            scoreText.fontSize = 75;

        scoreText.text = min.ToString() + ": " + s;

        if (Health.playerHP <= 0)
        {
            if (time > PlayerPrefs.GetInt("Time"))
                PlayerPrefs.SetInt("Time", time);
        }

    }

    private IEnumerator displayInstructions1()
    {
        yield return new WaitForSeconds(0.4f);
        HowToPlay2.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);

        HowToPlay2.transform.GetComponent<Text>().text = "Tap farther away from the cannon to shoot longer distances.";
        yield return new WaitForSeconds(4f);
        HowToPlay2.gameObject.SetActive(false);

        HowToPlay1.gameObject.SetActive(true);
        yield return new WaitForSeconds(4f);
        HowToPlay1.gameObject.SetActive(false);
    }
}
