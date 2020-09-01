using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Loadout : MonoBehaviour, IDropHandler
{
    private int[] equiped = { 1, 2, 3 };
    public GameObject error;

    public void OnDrop(PointerEventData data)
    {
        int weapon = data.pointerDrag.transform.GetSiblingIndex();
        if (!equiped.Contains(weapon))
        {
            transform.GetChild(equiped[transform.GetSiblingIndex() - 2] - 1).gameObject.SetActive(false);
            equiped[transform.GetSiblingIndex() - 2] = weapon;
            transform.GetChild(weapon - 1).gameObject.SetActive(true);
        }
        else
        {
            error.SetActive(true);
            error.transform.GetChild(0).GetComponent<Text>().text = "This weapon is already equipped.";
        }
    }
    
    
}