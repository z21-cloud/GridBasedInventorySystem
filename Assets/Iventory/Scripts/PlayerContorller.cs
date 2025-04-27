using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Inventory;
using UI;

namespace Player.Input
{
    public class PlayerContorller : MonoBehaviour
    {
        [SerializeField] Transform droppedItemsParent;
        [SerializeField] Transform dropPoint;
        public void OnInventoryItemRotate(InputValue inputValue) //r
        {
            Vector2Int size = InventoryManager.Instance.draggingItem.GetComponent<ItemDataMB>().itemData.item.slotSize;

            if(InventoryManager.Instance.isDraggingItem && size.x != size.y)
            {
                bool previousState = InventoryManager.Instance.draggingItem.transform.GetComponent<ItemDataMB>().itemData.isRotated;
                bool newStatus = !previousState;
                InventoryManager.Instance.draggingItem.transform.GetChild(0).gameObject.SetActive(previousState);
                InventoryManager.Instance.draggingItem.transform.GetChild(1).gameObject.SetActive(newStatus);
                InventoryManager.Instance.draggingItem.transform.GetComponent<ItemDataMB>().itemData.isRotated = newStatus;
                InventoryManager.Instance.RefreshItem();
            }
        }

        public void OnInventory(InputValue inputValue) //tab
        {
            UIController.Instance.TurnCanvasInventory();
            UIController.Instance.Tooltip(null, false);
            UIController.Instance.HideAllActionMenu();
            UIController.Instance.SetActiveLootPanel(false);
            UIController.Instance.SetCursos(UIController.Instance.GetStatusOffCanvas(
                UIController.Instance.canvasGroupInventory
                ));
        }

        public void OnLoot(InputValue inputValue) //e
        {
            RaycastHit hit;
            if(Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, 2f))//can change distance and can add a layer
            {
                if(hit.transform.CompareTag("Item"))
                {
                    //
                }
                else if(hit.transform.CompareTag("Container"))
                {
                    StartCoroutine(InventoryManager.Instance.CreatePanel(
                        InventoryManager.Instance.GetPanel(
                            Panel.Type.Loot), hit.transform.parent.GetComponent<LootData>()));
                    UIController.Instance.SetActiveLootPanel(true);
                    UIController.Instance.SetCanvasIntentory(true);
                    UIController.Instance.SetCursos(true);
                }
            }
        }

        public void OnMenu(InputValue inputValue) //escape
        {
            UIController.Instance.TurnCanvasMenu();
            UIController.Instance.SetCanvasIntentory(false);
            UIController.Instance.SetCursos(UIController.Instance.GetStatusOffCanvas(
                UIController.Instance.canvasGroupMenu
                ));
        }

        public void DropItem(ItemData itemData)
        {
            GameObject droppedItem = Instantiate(itemData.item.prefab, dropPoint.position, Quaternion.identity, droppedItemsParent);
            droppedItem.AddComponent<ItemDataMB>();
            droppedItem.GetComponent<ItemDataMB>().itemData = itemData;
            Debug.Log(itemData.myLootContainerId + "dropped item if con id");
            droppedItem.AddComponent<Rigidbody>();
            droppedItem.AddComponent<RemoveRigidbody>();
            droppedItem.GetComponent<Rigidbody>().AddForce(dropPoint.forward * 20f);
        }
    }
}

