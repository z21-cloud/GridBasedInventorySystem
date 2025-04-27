using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class EmptyPanel : MonoBehaviour, IDropHandler, IPointerEnterHandler
    {
        public void OnDrop(PointerEventData eventData)
        {
            if(!InventoryManager.Instance.isLockedUI)
            {
                eventData.pointerDrag.GetComponent<ItemUI>().RestartPosition();
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InventoryManager.Instance.isDraggingItem && eventData != null)
            {
                if(!eventData.pointerDrag.gameObject.GetComponent<ItemDataMB>().itemData.isRotated) //if no rotated
                {
                    eventData.pointerDrag.gameObject.transform.GetChild(0).GetComponent<Image>().color = InventoryManager.Instance.disagree;
                }
                else //if rotated
                {
                    eventData.pointerDrag.gameObject.transform.GetChild(1).GetComponent<Image>().color = InventoryManager.Instance.disagree;
                }
            }
        }
    }
}

