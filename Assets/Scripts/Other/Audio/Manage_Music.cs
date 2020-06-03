using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;

public class Manage_Music : MonoBehaviour
{
    public UnityEngine.UI.Slider sound;
    public UnityEngine.UI.Slider music;
    
    // Start is called before the first frame update
    void Start()
    {
        music.value = PlayerPrefs.GetFloat("Music");
        sound.value = PlayerPrefs.GetFloat("Sound");
    }

    void Update() { 
        GameObject.Find("Music Manager").transform.GetComponent<AudioSource>().volume = music.value;
        if (GameObject.Find("Settings Panel") != null)
        GameObject.Find("Settings Panel").transform.GetComponent<AudioSource>().volume = music.value;

        PlayerPrefs.SetFloat("Music", music.value);
        PlayerPrefs.SetFloat("Sound", sound.value);
    }
}
