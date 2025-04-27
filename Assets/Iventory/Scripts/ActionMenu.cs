using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using TMPro;
using Inventory;
using Player.Status;

namespace UI
{
    public class ActionMenu : MonoBehaviour
    {
        public ActionType actionType;
        private ItemData selectedItemData;

        public enum ActionType
        {
            Default,
            Food
        }

        public void SetActionMenu(ItemData itemData)
        {
            selectedItemData = itemData;
            switch(itemData.item.Type)
            {
                case ItemType.Food:
                    {
                        SetActionFood(itemData);
                        break;
                    }
                default: 
                    {
                        SetActionDefault(itemData);
                        break;
                    }
            }
        }

        private void SetActionFood(ItemData itemData)
        {
            transform.transform.position = Mouse.current.position.ReadValue();
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = itemData.item.itemName;
            transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(()=> 
            {
                itemData.item.Use();
                InventoryManager.Instance.RemoveItem(itemData);
                UIController.Instance.HideAllActionMenu();
                UIController.Instance.isLockedUI = false;
            });


            transform.GetChild(2).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(2).GetComponent<Button>().onClick.AddListener(() => { Drop(itemData); });
            transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(Close);
        }

        private void SetActionDefault(ItemData itemData)
        {
            transform.transform.position = Mouse.current.position.ReadValue();
            transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = itemData.item.itemName;
            transform.GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(1).GetComponent<Button>().onClick.AddListener(() => { Drop(itemData); });
            transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.RemoveAllListeners();
            transform.GetChild(0).GetChild(1).GetComponent<Button>().onClick.AddListener(Close);
        }

        private void Drop(ItemData itemData)
        {
            GameManager.Instance.playerContorller.DropItem(itemData);
            InventoryManager.Instance.RemoveItem(itemData);
            UIController.Instance.HideAllActionMenu();
            UIController.Instance.isLockedUI = false;

            Debug.Log(itemData.item.itemName + " dropped.");
        }

        private void Close()
        {
            Debug.Log("ActionMenu is closed");
            gameObject.SetActive(false);
            UIController.Instance.isLockedUI = false;
        }
    }
}

