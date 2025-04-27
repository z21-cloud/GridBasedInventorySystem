using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;
using Inventory;
using TMPro;

namespace UI
{
    public class UIController : MonoBehaviour
    {
        #region SINGLETON
        private static UIController instance = null;
        public static UIController Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<UIController>();
                }
                return instance;
            }
        }

        private void OnEnable()
        {
            instance = this;
        }
        #endregion

        public CanvasGroup canvasGroupInventory;
        public CanvasGroup canvasGroupMenu;
        [SerializeField] GameObject lootPanel;
        public bool isLockedUI = false;
        //
        [SerializeField] Transform tooltip;
        //
        public List<ActionMenu> actionMenuList = new List<ActionMenu>();
        [SerializeField] Transform actionMenuParent;

        void Start()
        {
            canvasGroupInventory.alpha = 0;
            canvasGroupInventory.interactable = false;
            canvasGroupInventory.blocksRaycasts = false;

            canvasGroupMenu.alpha = 0;
            canvasGroupMenu.interactable = false;
            canvasGroupMenu.blocksRaycasts = false;

            foreach (Transform menu in actionMenuParent)
            {
                actionMenuList.Add(menu.GetComponent<ActionMenu>());
            }
        }

        // Update is called once per frame
        void Update()
        {
            if (InventoryManager.Instance.isDraggingItem && !InventoryManager.Instance.isLockedUI) //item drag
            {
                InventoryManager.Instance.draggingItem.GetComponent<RectTransform>().position = Vector3.Lerp(
                    InventoryManager.Instance.draggingItem.GetComponent<RectTransform>().position,
                    new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0),
                    Time.deltaTime * InventoryManager.Instance.draggingItemSmoothFactor
                    );
            }
            if(!isLockedUI && !InventoryManager.Instance.isDraggingItem) //tooltip
            {
                tooltip.GetComponent<RectTransform>().position = Vector3.Lerp(tooltip.GetComponent<RectTransform>().position, new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue(), 0) + new Vector3(50f, -50f), Time.deltaTime * InventoryManager.Instance.draggingItemSmoothFactor);
            }
        }

        public void SetCanvasIntentory(bool value)
        {
            if (value) //if value = true
            {
                canvasGroupInventory.alpha = 1;
                canvasGroupInventory.interactable = true;
                canvasGroupInventory.blocksRaycasts = true;
            }
            else //if = false
            {
                canvasGroupInventory.alpha = 0;
                canvasGroupInventory.interactable = false;
                canvasGroupInventory.blocksRaycasts = false;
            }
        }

        public void SetCanvasMenu(bool value)
        {
            if (value) //if value = true
            {
                canvasGroupMenu.alpha = 1;
                canvasGroupMenu.interactable = true;
                canvasGroupMenu.blocksRaycasts = true;
            }
            else //if = false
            {
                canvasGroupMenu.alpha = 0;
                canvasGroupMenu.interactable = false;
                canvasGroupMenu.blocksRaycasts = false;
            }
        }

        public void TurnCanvasMenu()
        {
            if (GetStatusOffCanvas(canvasGroupMenu))
            {
                canvasGroupMenu.alpha = 0;
                canvasGroupMenu.interactable = false;
                canvasGroupMenu.blocksRaycasts = false;
            }
            else
            {
                canvasGroupMenu.alpha = 1;
                canvasGroupMenu.interactable = true;
                canvasGroupMenu.blocksRaycasts = true;
            }
        }

        public void TurnCanvasInventory()
        {
            if(GetStatusOffCanvas(canvasGroupInventory))
            {
                canvasGroupInventory.alpha = 0;
                canvasGroupInventory.interactable = false;
                canvasGroupInventory.blocksRaycasts = false;
            }
            else
            {
                canvasGroupInventory.alpha = 1;
                canvasGroupInventory.interactable = true;
                canvasGroupInventory.blocksRaycasts = true;
            }
        }

        public bool GetStatusOffCanvas(CanvasGroup canvasGroup)
        {
            if(canvasGroup.alpha == 1)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public void SetActiveLootPanel(bool value)
        {
            lootPanel.SetActive(value);
        }

        public void SetCursos(bool value)
        {
            if(value)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.Confined;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
        }

        public void Tooltip(ItemData itemData, bool value) //true: on, false: off
        {
            if (value && !isLockedUI)
            {
                tooltip.transform.GetChild(0).GetChild(0).GetComponent<TextMeshProUGUI>().text = itemData.item.itemName;
                tooltip.transform.GetChild(1).GetChild(0).GetComponent<TextMeshProUGUI>().text = itemData.item.description;
                tooltip.gameObject.SetActive(true);
                LayoutRebuilder.ForceRebuildLayoutImmediate(tooltip as RectTransform); //reflesh layout
            }
            else if (!value)
            {
                tooltip.gameObject.SetActive(false);
            }
        }

        public void SetActionMenu(ItemData itemData, bool value)
        {
            isLockedUI = true;
            Tooltip(null, false);

            HideAllActionMenu();
            switch(itemData.item.Type)
            {
                case ItemType.Food: 
                    { 
                        GetActionMenu(ActionMenu.ActionType.Food).gameObject.SetActive(value); 
                        GetActionMenu(ActionMenu.ActionType.Food).SetActionMenu(itemData); 
                        break; 
                    }
                default: 
                    {
                        GetActionMenu(ActionMenu.ActionType.Default).gameObject.SetActive(value);
                        GetActionMenu(ActionMenu.ActionType.Default).SetActionMenu(itemData);
                        break;
                    }
            }
        }

        public void HideAllActionMenu()
        {
            foreach(ActionMenu actionMenu in actionMenuList)
            {
                actionMenu.transform.gameObject.SetActive(false);
            }
        }

        private ActionMenu GetActionMenu(ActionMenu.ActionType actionType)
        {
            foreach(ActionMenu actionMenu in actionMenuList)
            {
                if(actionType == actionMenu.actionType)
                {
                    return actionMenu;
                }
            }
            return null;
        }
    }
}

