using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class Drag : MonoBehaviour, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDragHandler
{
    [SerializeField] private Canvas canvas;
    private Vector3 origin;
    private CanvasGroup c_group;
    private bool draggable;
    public GameObject Error;

    private void Awake()
    {
        origin = transform.localPosition;
        c_group = transform.GetComponent<CanvasGroup>();
        draggable = Shop.cannonlevels[transform.GetSiblingIndex()] > 0;
    }

    public void OnBeginDrag(PointerEventData data)
    {
        if (!draggable)
        {
            return;
        }
        c_group.blocksRaycasts = false;
        transform.GetComponent<RectTransform>().localScale = Vector3.Scale(transform.GetComponent<RectTransform>().localScale,new Vector3(0.5f,0.5f,0.5f));
    }

    public void OnDrag(PointerEventData data)
    {
        if (!draggable)
        {
            return;
        }
        transform.GetComponent<RectTransform>().anchoredPosition += data.delta/canvas.scaleFactor;
    }

    public void OnEndDrag(PointerEventData data)
    {
        if (!draggable)
        {
            return;
        }
        transform.localPosition = origin;
        c_group.blocksRaycasts = true;
        transform.GetComponent<RectTransform>().localScale = Vector3.Scale(transform.GetComponent<RectTransform>().localScale, new Vector3(02f, 2f, 2f));
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!draggable)
        {
            Error.SetActive(true);
            Error.transform.GetChild(0).GetComponent<Text>().text = "You do not own this weapon yet.";
        }
    }
}
