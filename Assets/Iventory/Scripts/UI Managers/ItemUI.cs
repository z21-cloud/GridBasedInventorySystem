using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UI;

namespace Inventory
{
    public class ItemUI : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerDownHandler, IBeginDragHandler, IEndDragHandler, IDropHandler, IDragHandler
    {
        private ItemData itemData;
        //backup
        private Vector3 startPosition;
        private bool startRotate;
        private Transform startParent;
        private Vector2Int startItemMatrixPosition;
        //backup*
        public void Initialize()
        {
            itemData = GetComponent<ItemDataMB>().itemData;
            transform.GetChild(0).GetComponent<Image>().color = itemData.item.background;
            transform.GetChild(0).GetChild(0).transform.GetComponent<Image>().sprite = itemData.item.image;
            transform.GetComponent<RectTransform>().position = itemData.slotPoisiton;

            if (transform.childCount > 1)
            {
                transform.GetChild(1).GetComponent<Image>().color = itemData.item.background;
                transform.GetChild(1).GetChild(0).transform.GetComponent<Image>().sprite = itemData.item.image;

                if (itemData.isRotated)
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(true);
                }
                else
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(true);
                }
            }
        }

        public void RestartPosition()
        {
            itemData = GetComponent<ItemDataMB>().itemData;
            transform.position = startPosition;
            if(itemData.isRotated != startRotate)
            {
                if(!startRotate) // false: no-rotated
                {
                    transform.GetChild(1).gameObject.SetActive(false);
                    transform.GetChild(0).gameObject.SetActive(true);
                }
                else //rotated
                {
                    transform.GetChild(0).gameObject.SetActive(false);
                    transform.GetChild(1).gameObject.SetActive(true);
                }
            }
            itemData.isRotated = startRotate;
            transform.SetParent(startParent);
            itemData.matrixPosition = startItemMatrixPosition;
            InventoryManager.Instance.SetMatrixThanPanel(itemData, true);
        }

        public void OnBeginDrag(PointerEventData eventData)
        {
            if (!InventoryManager.Instance.isLockedUI) //if not lockedUI, if it is free 
            {
                itemData = GetComponent<ItemDataMB>().itemData;
                transform.GetComponent<CanvasGroup>().blocksRaycasts = false;
                startParent = transform.parent;
                startPosition = transform.position;
                startRotate = itemData.isRotated;
                startItemMatrixPosition = itemData.matrixPosition;
                transform.SetParent(InventoryManager.Instance.draggingItemParent); //for the item to appear first
                InventoryManager.Instance.SetItemBlockRaycast(false); //for detect the slots under the items
                InventoryManager.Instance.SetMatrixThanPanel(itemData, false);
                InventoryManager.Instance.draggingItem = gameObject;
                InventoryManager.Instance.isDraggingItem = true;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            if (!InventoryManager.Instance.isLockedUI) //if not lockedUI, if it is free 
            {
                itemData = GetComponent<ItemDataMB>().itemData;
                transform.GetComponent<CanvasGroup>().blocksRaycasts = true;
                InventoryManager.Instance.isDraggingItem = false;
                InventoryManager.Instance.SetItemBlockRaycast(true);
                transform.GetChild(0).GetComponent<Image>().color = itemData.item.background;
                if(transform.childCount > 1)
                {
                    transform.GetChild(1).GetComponent<Image>().color = itemData.item.background;
                }
            }
        }

        public void OnDrop(PointerEventData eventData)
        {
            if (!InventoryManager.Instance.isLockedUI) //if not lockedUI, if it is free 
            {
                eventData.pointerDrag.GetComponent<ItemUI>().RestartPosition();
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            if (eventData.button == PointerEventData.InputButton.Right && !InventoryManager.Instance.isLockedUI) //if clicked right button
            {
                UIController.Instance.SetActionMenu(itemData, true); // open action panel
            }
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            UIController.Instance.Tooltip(itemData, true); //open tooltip
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            UIController.Instance.Tooltip(null, false); //close tooltip
        }

        public void OnDrag(PointerEventData eventData)
        {
            //
        }
    }
}

