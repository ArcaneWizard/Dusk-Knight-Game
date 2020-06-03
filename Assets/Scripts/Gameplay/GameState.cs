using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SocialPlatforms.Impl;

public class GameState : MonoBehaviour
{
    public Text HowToPlay1;
    public Text HowToPlay2;
    public Text scoreText;

    private string key;
    private bool counter = false;
    private bool counter2 = false;
    public static int time;
    public static int min;
    public static int sec;
    public static string s;

    public Health healthScript;
    public Shop shop;

    public GameObject Revived;

    private IEnumerator scoreIncreasesInTime()
    {
        yield return new WaitForSeconds(1);
        time++;
        if (Health.playerHP > 0)
        StartCoroutine(scoreIncreasesInTime());
    }

    public void Revive() {
        Time.timeScale = 0;
        Advertisement.Show("rewardedVideo", new ShowOptions() { resultCallback = HandleAdResult });
    } 
    
    private void HandleAdResult(ShowResult result)
    {
        switch (result)
        {
            case ShowResult.Finished:
                Debug.Log("Player watched ad");
                PlayerPrefs.SetInt("continue", time);
                PlayerPrefs.SetInt("revived", 1);
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
                break;
            case ShowResult.Skipped:
                Debug.Log("Player skipped ad");
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
                break;
            case ShowResult.Failed:
                Debug.Log("No internet");
                Time.timeScale = 1;
                SceneManager.LoadScene(0);
                break;
        }
    }

    // Start is called before the first frame update
    void Awake()
    {
        key = "GameState24";

        if (PlayerPrefs.GetInt(key) == 0)
        {
            int state = PlayerPrefs.GetInt(key);
            state++;
            PlayerPrefs.SetInt(key, state);
            PlayerPrefs.SetFloat("Music", 0.9f);
            PlayerPrefs.SetFloat("Sound", 0.9f);
        }

        if (PlayerPrefs.GetInt(key) == 1)
        {
            shop.ResetUpgrades();
        }

        time = PlayerPrefs.GetInt("continue");  
        PlayerPrefs.SetInt("continue", 0);
    }

    void Update()
    {       
        if (healthScript.firstTime == false && counter == false) {         
            StartCoroutine(scoreIncreasesInTime());
            counter = true;
        }

        if (Input.touchCount > 0 && PlayerPrefs.GetInt(key) == 1 && counter2 == false) {            
            StartCoroutine(displayInstructions1());
            PlayerPrefs.SetInt(key, 2);
            counter2 = true;
        }
        if (Input.touchCount > 0 && PlayerPrefs.GetInt(key) == 2 && counter2 == false) {            
            StartCoroutine(displayInstructions1());
            PlayerPrefs.SetInt(key, 3);
            counter2 = true;
            print("wh");
        }

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
            scoreText.gameObject.SetActive(false);
            
            if (PlayerPrefs.GetInt("revived") == 1)
                Revived.gameObject.SetActive(false);
            else    
                Revived.gameObject.SetActive(true);    

            if (time > PlayerPrefs.GetInt("Time"))
                PlayerPrefs.SetInt("Time", time);
        }
    }

    private IEnumerator displayInstructions1()
    {
        yield return new WaitForSeconds(0.4f);
        HowToPlay2.gameObject.SetActive(true);
        yield return new WaitForSeconds(3);

        HowToPlay2.transform.GetComponent<Text>().text = "The further you tap from the cannon, the further you shoot.";
        yield return new WaitForSeconds(4f);
        HowToPlay2.gameObject.SetActive(false);

        HowToPlay1.gameObject.SetActive(true);
        yield return new WaitForSeconds(5f);
        HowToPlay1.transform.GetComponent<Text>().text = "Your weapons are accessible below while the shop is above.";
        yield return new WaitForSeconds(3f);
        HowToPlay1.gameObject.SetActive(false);
    }
}
