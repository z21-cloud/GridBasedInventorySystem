using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DragTitleBar : MonoBehaviour, IDragHandler
{
    public void OnDrag(PointerEventData eventData)
    {
        transform.parent.transform.GetComponent<RectTransform>().anchoredPosition += eventData.delta;
    }
}
