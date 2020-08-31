using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class lvl_selection : MonoBehaviour
{
    public List<Sprite> previews; 
    private List<string> descriptions = new List<string>(new string[11]);
    public Transform Levels;

    public Image preview;
    public Text description;

    private Color32 selected = new Color32(124, 119, 191, 255);
    private Color32 unselected = new Color32(204, 204, 204, 255);
    private Color32 locked = new Color32(106, 106, 106, 255);

    private Color32 unselectedText = new Color32 (41, 41, 41, 255);
    private Color32 selectedText = new Color32(29, 29, 29, 255);

    //From 1 to 22 (levels 1 to 22)
    private int lvlSelected;

    //From 1 to 22 (this variable changes and must be saved)
    private int lvlUnlocked = 7;

    void Awake() 
    {
        //highest lvl unlocked is the lvl selected by default
        lvlSelected = lvlUnlocked;
        
        descriptions[0] = "Prove yourself a warrior by taking back the outpost from northern invaders.";
        descriptions[1] = "Great job, but it looks like they’ve sent backup reinforcements. Wipe them out too.";
        descriptions[2] = "We’ve intercepted a message that they’re sending Orcs on a nearby route. Set up camp in the hills and ambush them.";
        descriptions[3] = "Travel to Warthrox. If you spot any monsters, set up camp and take them out.";
        descriptions[4] = "Looks like we were too late. The flying reapers have joined the fight.";
        descriptions[5] = "There a lot more monsters approaching. Hang in tight, we are crafting a powerful weapon to send to you";
        descriptions[6] = "The Reapers have a record of creepy rituals and explosive sorcery. Take caution as you header deeper into their homeland.";
        descriptions[7] = "We airdropped you a better weapon. It’ll let you harness your rage even more effectively.";
        descriptions[8] = "Watch out. Ogres usually live below ground, but they’ve somehow been spotted leading the charge.";
        descriptions[9] = "It was all a trap. Destroy the research and escape.";
        descriptions[10] = "You’ve stumbled to the very heart of Warthrox. Be wary of our region’s Overlord, the Reaper King.";

        //Setup the unlocked and locked lvls at the beginning + the selected lvl
        for (int level = 1; level <= Levels.childCount; level++) 
        {
            if (level < lvlUnlocked) 
                Levels.transform.GetChild(level-1).transform.GetComponent<Image>().color = unselected;

            if (level == lvlUnlocked) {
                Levels.transform.GetChild(level-1).transform.GetComponent<Image>().color = selected;
                Levels.transform.GetChild(level-1).transform.GetChild(0).transform.GetComponent<Text>().color = selectedText;            
            }

            if (level > lvlUnlocked) {
                Levels.transform.GetChild(level-1).transform.GetComponent<Image>().color = locked;
                Levels.transform.GetChild(level-1).transform.GetChild(0).gameObject.SetActive(false);
                Levels.transform.GetChild(level-1).transform.GetChild(1).gameObject.SetActive(true);
            }
        }
        
        preview.sprite = previews[lvlSelected-1];
        description.text = descriptions[lvlSelected-1];
    }

    public void selectLevel() 
    {
        //get the button pressed and what lvl it corresponds to
        GameObject button = UnityEngine.EventSystems.EventSystem.current.currentSelectedGameObject;
        lvlSelected = button.transform.GetSiblingIndex() + 1;

        //if the lvl has been unlocked
        if (lvlSelected <= lvlUnlocked) 
        {
            //set the preview image and description to correspond to that level
            preview.sprite = previews[lvlSelected-1];
            description.text = descriptions[lvlSelected-1];

            //reset all levels' buttons and text colors
            for (int level = 1; level <= lvlUnlocked; level++) {
                Levels.transform.GetChild(level-1).transform.GetComponent<Image>().color = unselected;
                Levels.transform.GetChild(level-1).transform.GetChild(0).transform.GetComponent<Text>().color = unselectedText;            
            }
 
            //Update the selected button's color and text color
            button.transform.GetComponent<Image>().color = selected;
            button.transform.GetChild(0).transform.GetComponent<Text>().color = selectedText;            
        }

        else {
            //play error buzzing sound
        }
    }

    public void startLevel() 
    {
        //Load the scene corresponding to the most recent lvl selected
        SceneManager.LoadScene(lvlSelected + 2);
    }

    public void shopping()
    {
        SceneManager.LoadScene(1);
    }
}

