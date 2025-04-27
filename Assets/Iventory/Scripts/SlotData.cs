using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

namespace Inventory
{
    public class SlotData : MonoBehaviour, IDropHandler, IPointerEnterHandler
    {
        public SlotType slotType;
        public Vector2Int matrixPosition;
        public Panel.Type panelType;
        public GameObject myLootContainer;
        public bool isFull = false; //for the character panel

        public void OnDrop(PointerEventData eventData)
        {
            if(eventData.pointerDrag != null && !InventoryManager.Instance.isLockedUI)
            {
                if(slotType == SlotType.General || slotType == eventData.pointerDrag.transform.GetComponent<ItemDataMB>().itemData.item.SlotType)
                {
                    if (panelType == Panel.Type.Character)
                    {
                        InventoryManager.Instance.CheckItemToPlace(eventData.pointerDrag.gameObject, transform);
                    }
                    else
                    {
                        InventoryManager.Instance.CheckItemMatrixToPlace(eventData.pointerDrag.gameObject, transform);
                    }
                }
                else
                {
                    eventData.pointerDrag.GetComponent<ItemUI>().RestartPosition();
                }
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            if(InventoryManager.Instance.isDraggingItem)
            {
                StartCoroutine(ColorHelper(eventData.pointerDrag.gameObject));
            }
        }

        IEnumerator ColorHelper(GameObject item)
        {
            yield return new WaitForEndOfFrame();
            InventoryManager.Instance.ColorHelper(item, transform);
        }
    }
}