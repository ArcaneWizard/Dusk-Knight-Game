using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class cutscene : MonoBehaviour
{
    private List<List<Sprite>> cutscenes = new List<List<Sprite>>();
    public int endScene;

    public static List<int> cutscenesToShow = new List<int>() {0, 1, 2};
    private int cutscenesShown = 0;
    private int lastCutSceneShown = 0;
    private float animationSpeed = 1;
    private IEnumerator nextCutSceneInst;

    [Space(10)]
    [Header("Cutscenes")]

    public List<Sprite> kingdomAttacked;
    public List<Sprite> useRageEffect;
    public List<Sprite> awakenDuskKnight;

    [Space(10)]
    [Header("Components")]

    public Image image;
    public Text caption;
    private List<string> captions;

    void Start() {

        //add all cutscenes to a list
        cutscenes.Add(kingdomAttacked);
        cutscenes.Add(useRageEffect);
        cutscenes.Add(awakenDuskKnight);

        captions = new List<string>(new string[cutscenes.Count]);

        //add all captions to a list
        captions[0] = "5 years ago, the Overlord of the Hills tried to siege our kingdom and failed. ";
        captions[1] = "But in that battle, we were forced to let an artifact of great darkness take over ourselves. The artifact filled our soldiers were rage, and you were by far the most affected by it.";
        captions[2] = "Now, the war has begun once again with newly formed alliances. It’s time for you to step out of the shadows, harness your rage and join the fight.";

        //display the first animated cutscene and caption
        animationSpeed = 0.7f;
        StartCoroutine(showAnimatedCutscene());
        caption.text = captions[cutscenesToShow[cutscenesShown]];
    }

    //Animate a given cutscene
    private IEnumerator showAnimatedCutscene() 
    {
        //loop through all the cutscene's sprites with a slight delay btwn each
        foreach (Sprite scene in cutscenes[cutscenesToShow[cutscenesShown]]) {
            if (lastCutSceneShown == cutscenesShown)  {
                yield return new WaitForSeconds(0.1f / animationSpeed);
                image.sprite = scene;
            }       
            else
                 yield return new WaitForSeconds(0.001f);
        }

        lastCutSceneShown = cutscenesShown;
        StartCoroutine(showAnimatedCutscene());
    }

    //Cycle through diff cutscenes with the NEXT button
    public void nextCutScene() 
    {
        //cycle to the next specified cutscene
        if (cutscenesShown < cutscenesToShow.Count - 1) {
            animationSpeed = 1;
            cutscenesShown++;
            caption.text = captions[cutscenesToShow[cutscenesShown]];
        }

        //afterwards load the right lvl or lvl selection
        else         
            SceneManager.LoadScene(endScene);
    }
}
