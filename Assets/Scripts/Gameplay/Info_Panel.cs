using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Info_Panel : MonoBehaviour
{
    public GameObject infoPanel;
    private Animator animator;

    private int counter = 0;

    void Start() 
    {
        //Define components
        animator = infoPanel.transform.GetComponent<Animator>();

        //Info panel enters the screen
        StartCoroutine(triggerPanelEntering());
    }

    void Update()
    {
       //Trigger the info panel's exit animation when the player touches the screen
        if (Input.touchCount > 0 && counter == 1) {
            counter = 2;
            StartCoroutine(triggerPanelExit());
        }
    }

    private IEnumerator triggerPanelEntering()
    {
        //activate the info panel and play its entering animation
        animator.SetBool("Enter", false);
        infoPanel.SetActive(true);    

        //allow the player to tap to get rid of the panel after one second
        yield return new WaitForSeconds(0.4f);
        counter = 1; 
    }

    private IEnumerator triggerPanelExit() 
    {        
        //play the exiting animation
        animator.SetBool("Enter", true);

        //start the spawner script once the animation is over
        yield return new WaitForSeconds(1f);
        transform.GetComponent<Spawner>().startSpawningEnemies();
        infoPanel.SetActive(false);     
    }
}
