using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UI;

namespace Inventory
{
    public class InventoryManager : MonoBehaviour
    {
        #region SINGLETON
        private static InventoryManager instance = null;
        public static InventoryManager Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new GameObject("GameManager").AddComponent<InventoryManager>();
                }
                return instance;
            }
        }

        private void OnEnable()
        {
            instance = this;
        }
        #endregion

        //Slot
        [SerializeField] GameObject slotPrefab; //for instantiate
        [SerializeField] Transform slotParentBackpack;
        [SerializeField] Transform slotParentLoot;

        public Transform itemParentCharacter;
        public Transform itemParentBackpack;
        public Transform itemParentLoot;

        //item UI prefabs
        [SerializeField] GameObject item1x1;
        [SerializeField] GameObject item1x2;
        [SerializeField] GameObject item1x3;
        [SerializeField] GameObject item1x4;
        [SerializeField] GameObject item1x5;
        [SerializeField] GameObject item2x2;

        //Dragging
        public bool isDraggingItem;
        public GameObject draggingItem;
        public float draggingItemSmoothFactor = 10f;
        public Transform draggingItemParent;
        //Color
        public Color agree;
        public Color disagree;

        //Panel
        public List<Panel> panelList = new List<Panel>();
        //
        public bool isLockedUI;
        private Transform lastSlot;

        private void Start()
        {
            panelList = GetComponents<Panel>().ToList();
            isLockedUI = UIController.Instance.isLockedUI;

            StartCoroutine(CreatePanel(GetPanel(Panel.Type.Backpack), null)); //Creating Backpack Panel
        }

        public IEnumerator CreatePanel(Panel panel, LootData lootData)
        {
            ClearPanel(panel.type);

            if (lootData != null) //for lootdata and loot panel
            {
                panel.size = lootData.size;
            }

            yield return new WaitForEndOfFrame();

            for (int i = 0; i < panel.size.x; i++) //creating slots
            {
                for (int j = 0; j < panel.size.y; j++)
                {
                    GameObject slot = Instantiate(slotPrefab, panel.slotParent);
                    slot.transform.GetComponent<SlotData>().matrixPosition = new Vector2Int(i, j);
                    slot.transform.GetComponent<SlotData>().panelType = panel.type;
                    if (lootData != null)
                    {
                        slot.transform.GetComponent<SlotData>().myLootContainer = lootData.gameObject;
                    }
                }
            }

            yield return new WaitForEndOfFrame();

            panel.matrix = new bool[panel.size.x, panel.size.y]; //panel bool matrix

            //Add items
            if (lootData != null)
            {
                FindSlotPositionForItem(panel.slotParent, lootData.itemList);
                panel.itemDataList = lootData.itemList; //list synced;
            }
            else
            { 
                FindSlotPositionForItem(panel.slotParent, panel.itemDataList);
            }
            FillItem(panel, lootData);
        }

        public void ClearPanel(Panel.Type panelType)
        {
            Panel panel = GetPanel(panelType);
            if(panelType != Panel.Type.Character)
            {
                foreach (Transform slot in panel.slotParent) //removing slots
                {
                    Destroy(slot.gameObject);
                }
            }

            foreach(Transform item in panel.itemParent) //removing items
            {
                Destroy(item.gameObject);
            }
        }

        public void FindSlotPositionForItem(Transform slotParent, List<ItemData> itemDataList)
        {
            foreach (ItemData itemData in itemDataList)
            {
                foreach (Transform slot in slotParent)
                {
                    if (slot.GetComponent<SlotData>().matrixPosition == itemData.matrixPosition)
                    {
                        itemData.slotPoisiton = slot.GetComponent<RectTransform>().position;
                        break;
                    }
                }
            }
        }
        public void FillItem(Panel panel, LootData lootData)
        {
            if (lootData != null) //for loot panel
            {
                foreach (ItemData itemData in lootData.itemList)
                {
                    GameObject newItem = Instantiate(GetMyPrefab(itemData.item.slotSize), panel.itemParent);
                    newItem.transform.GetComponent<ItemDataMB>().itemData = itemData;
                    newItem.transform.GetComponent<ItemDataMB>().itemData.slotPanelType = panel.type;
                    newItem.transform.GetComponent<ItemUI>().Initialize();
                    SetMatrixThanPanel(itemData, true);
                }
            }
            else //for other panels
            {
                if(panel.type == Panel.Type.Backpack)
                {
                    foreach (ItemData itemData in panel.itemDataList)
                    {
                        GameObject newItem = Instantiate(GetMyPrefab(itemData.item.slotSize), panel.itemParent);
                        newItem.transform.GetComponent<ItemDataMB>().itemData = itemData;
                        newItem.transform.GetComponent<ItemDataMB>().itemData.slotPanelType = panel.type;
                        newItem.transform.GetComponent<ItemUI>().Initialize();
                        SetMatrixThanPanel(itemData, true);
                    }
                }
                else if(panel.type == Panel.Type.Character)
                {
                    FindSloPositionForItemCharacterPanel(panel.itemDataList); 
                    foreach (ItemData itemData in panel.itemDataList)
                    {
                        GameObject newItem = Instantiate(GetMyPrefab(itemData.item.slotSize), panel.itemParent);
                        newItem.transform.GetComponent<ItemDataMB>().itemData = itemData;
                        newItem.transform.GetComponent<ItemDataMB>().itemData.slotPanelType = panel.type;
                        newItem.transform.GetComponent<ItemUI>().Initialize();
                        SetMatrixThanPanel(itemData, true);
                    }
                }
            }
        }

        private GameObject GetMyPrefab(Vector2Int size)
        {
            if (size == new Vector2Int(1, 1))
            {
                return item1x1;
            }
            else if (size == new Vector2Int(1, 2))
            {
                return item1x2;
            }
            else if (size == new Vector2Int(1, 3))
            {
                return item1x3;
            }
            else if (size == new Vector2Int(1, 4))
            {
                return item1x4;
            }
            else if (size == new Vector2Int(1, 5))
            {
                return item1x5;
            }
            else if (size == new Vector2Int(2, 2))
            {
                return item2x2;
            }
            return null;
        }
        public void SetMatrixThanPanel(ItemData itemData, bool value)
        {
            Panel panel = GetPanel(itemData.slotPanelType);
            Vector2Int itemSize = itemData.item.slotSize;

            //character panel
            if (itemData.slotPanelType == Panel.Type.Character) 
            {
                foreach(Transform slot in panel.slotParent)
                {
                    if (slot.GetComponent<SlotData>().matrixPosition.y == itemData.matrixPosition.y)
                    {
                        slot.GetComponent<SlotData>().isFull = value;
                        return;
                    }
                }
            }
            //character end

            //for other panels
            if(!itemData.isRotated) //if no rotated
            {
                for(int i = 0; i < itemSize.x; i++)
                {
                    for (int j = 0; j < itemSize.y; j++)
                    {
                        panel.matrix[itemData.matrixPosition.x + i, itemData.matrixPosition.y + j] = value;
                    }
                }
            }
            else //if rotated
            {
                for (int i = 0; i < itemSize.y; i++)
                {
                    for (int j = 0; j < itemSize.x; j++)
                    {
                        panel.matrix[itemData.matrixPosition.x + i, itemData.matrixPosition.y + j] = value;
                    }
                }
            }
        }

        private void FindSloPositionForItemCharacterPanel(List<ItemData> itemDataList)
        {
            Panel panel = GetPanel(Panel.Type.Character);
            foreach(ItemData itemData in itemDataList)
            {
                foreach(Transform slot in panel.slotParent)
                {
                    if(slot.GetComponent<SlotData>().matrixPosition.y == itemData.matrixPosition.y)
                    {
                        itemData.slotPoisiton = slot.GetComponent<RectTransform>().position;
                        break;
                    }
                }
            }
        }

        public Panel GetPanel(Panel.Type panelType)
        {
            foreach(Panel panel in panelList)
            {
                if(panel.type == panelType)
                {
                    return panel;
                }
            }
            return null;
        }

        public void SetItemBlockRaycast(bool value) //false: off ; true: on
        {
            foreach (Panel panel in panelList) //close all raycast of items
            {
                foreach (Transform item in panel.itemParent)
                {
                    item.GetComponent<CanvasGroup>().blocksRaycasts = value;
                }
            }
        }

        public void CheckItemToPlace(GameObject item, Transform slot) //for character panel
        {
            if (!slot.GetComponent<SlotData>().isFull) //false: avaible
            {
                slot.GetComponent<SlotData>().isFull = true;
                item.GetComponent<ItemDataMB>().itemData.matrixPosition.y = slot.GetComponent<SlotData>().matrixPosition.y;
                Panel previousPanel = GetPanel(item.GetComponent<ItemDataMB>().itemData.slotPanelType);
                SetItemParent(item, GetPanel(Panel.Type.Character).itemParent);
                //check the rotate
                if (slot.GetComponent<SlotData>().slotType == SlotType.Weapon || slot.GetComponent<SlotData>().slotType == SlotType.Pistol || slot.GetComponent<SlotData>().slotType == SlotType.Knife)
                {
                    if (item.GetComponent<ItemDataMB>().itemData.isRotated) //if rotated, set no rotated
                    {
                        item.transform.GetChild(0).gameObject.SetActive(true);
                        item.transform.GetChild(1).gameObject.SetActive(false);
                        item.transform.GetComponent<ItemDataMB>().itemData.isRotated = false;
                    }
                    float itemWidth = item.GetComponent<RectTransform>().sizeDelta.x;
                    float slotWidth = slot.GetComponent<RectTransform>().sizeDelta.x;
                    float deltaWidth = (slotWidth - itemWidth) / 2f;
                    float yOffset = slot.GetComponent<RectTransform>().sizeDelta.y / 4f;
                    item.GetComponent<RectTransform>().position
                        = slot.transform.GetComponent<RectTransform>().position + new Vector3(deltaWidth, -yOffset);

                }
                else
                {
                    item.GetComponent<RectTransform>().position = slot.transform.GetComponent<RectTransform>().position;
                }
                //
                UpdateItemDataListThanPanel(item, GetPanel(Panel.Type.Character), previousPanel);
                item.GetComponent<ItemDataMB>().itemData.slotPanelType = Panel.Type.Character;
            }
            else
            {
                item.GetComponent<ItemUI>().RestartPosition();
            }
        }

        public void CheckItemMatrixToPlace(GameObject item, Transform slot)
        {
            Vector2Int itemSize = item.GetComponent<ItemDataMB>().itemData.item.slotSize;
            Vector2Int slotMatrixPosition = slot.GetComponent<SlotData>().matrixPosition;
            Panel panel = GetPanel(slot.transform.GetComponent<SlotData>().panelType);
            Panel previousPanel = GetPanel(item.GetComponent<ItemDataMB>().itemData.slotPanelType);

            if(CheckMatrix(item, slot)) //false: full, true: avaible 
            {
                if(!item.GetComponent<ItemDataMB>().itemData.isRotated) // if no rotated
                {
                    for (int i = 0; i < itemSize.x; i++)
                    {
                        for (int j = 0; j < itemSize.y; j++)
                        {
                            panel.matrix[slotMatrixPosition.x + i, slotMatrixPosition.y + j] = true;
                        }
                    }
                }
                else //if rotated
                {
                    for (int i = 0; i < itemSize.y; i++)
                    {
                        for (int j = 0; j < itemSize.x; j++)
                        {
                            panel.matrix[slotMatrixPosition.x + i, slotMatrixPosition.y + j] = true;
                        }
                    }
                }

                item.GetComponent<RectTransform>().position = slot.transform.GetComponent<RectTransform>().position;
                SetItemParent(item, panel.itemParent);
                item.GetComponent<ItemDataMB>().itemData.matrixPosition = slotMatrixPosition;
                UpdateItemDataListThanPanel(item, panel, previousPanel);
                item.GetComponent<ItemDataMB>().itemData.slotPanelType = panel.type;
            }
            else
            {
                item.GetComponent<ItemUI>().RestartPosition();
            }
        }

        public bool CheckMatrix(GameObject item, Transform slot)
        {
            Vector2Int itemSize = item.GetComponent<ItemDataMB>().itemData.item.slotSize;
            Vector2Int slotMatrixPosition = slot.GetComponent<SlotData>().matrixPosition;
            Panel panel = GetPanel(slot.transform.GetComponent<SlotData>().panelType);
            int row = panel.matrix.GetLength(0);
            int column = panel.matrix.GetLength(1);

            if(!item.GetComponent<ItemDataMB>().itemData.isRotated)
            {
                if(slotMatrixPosition.y + itemSize.y <= column && slotMatrixPosition.x + itemSize.x <= row) //if there enough space. NO ROTATE
                {
                    for (int i = 0; i < itemSize.x; i++)
                    {
                        for (int j = 0; j < itemSize.y; j++)
                        {
                            if (panel.matrix[slotMatrixPosition.x + i, slotMatrixPosition.y + j] == true)
                            {
                                return false;
                            }
                        }
                    }
                    return true; //avaible
                }
                else
                {
                    return false;
                }
            }
            else
            {
                if (slotMatrixPosition.y + itemSize.x <= column && slotMatrixPosition.x + itemSize.y <= row) //if there enough space. NO ROTATE
                {
                    for (int i = 0; i < itemSize.y; i++)
                    {
                        for (int j = 0; j < itemSize.x; j++)
                        {
                            if (panel.matrix[slotMatrixPosition.x + i, slotMatrixPosition.y + j] == true)
                            {
                                return false; 
                            }
                        }
                    }
                    return true; //avaible
                }
                else
                {
                    return false;
                }
            }
        }

        public void SetItemParent(GameObject item, Transform parent)
        {
            item.transform.SetParent(parent);
        }

        public void UpdateItemDataListThanPanel(GameObject item, Panel newPanel, Panel previousPanel)
        {
            if(previousPanel.type != newPanel.type)
            {
                previousPanel.itemDataList.Remove(item.GetComponent<ItemDataMB>().itemData);
                newPanel.itemDataList.Add(item.GetComponent<ItemDataMB>().itemData);
            }
        }

        public void ColorHelper(GameObject item, Transform slot)
        {
            lastSlot = slot;
            Panel panel = GetPanel(slot.transform.GetComponent<SlotData>().panelType);
            ItemData itemData = item.GetComponent<ItemDataMB>().itemData;

            if(panel.type == Panel.Type.Character) //for character panel
            {
                if(!slot.transform.GetComponent<SlotData>().isFull && 
                    slot.transform.GetComponent<SlotData>().slotType == item.GetComponent<ItemDataMB>().itemData.item.SlotType)
                {
                    if(!itemData.isRotated)
                    {
                        item.transform.GetChild(0).transform.GetComponent<Image>().color = agree;
                    }
                    else //if rotated
                    {
                        item.transform.GetChild(1).transform.GetComponent<Image>().color = agree;
                    }
                }
                else
                {
                    if (!itemData.isRotated)
                    {
                        item.transform.GetChild(0).transform.GetComponent<Image>().color = disagree;
                    }
                    else //if rotated
                    {
                        item.transform.GetChild(1).transform.GetComponent<Image>().color = disagree;
                    }
                } 
                return;
            }

            //for other panels
            if (CheckMatrix(item, slot)) //true: avaible, false: full
            {
                if (!itemData.isRotated)
                {
                    if (!itemData.isRotated)
                    {
                        item.transform.GetChild(0).transform.GetComponent<Image>().color = agree;
                    }
                    else //if rotated
                    {
                        item.transform.GetChild(1).transform.GetComponent<Image>().color = agree;
                    }
                }
            }
            else
                {
                    if (!itemData.isRotated)
                    {
                        item.transform.GetChild(0).transform.GetComponent<Image>().color = disagree;
                    }
                    else //if rotated
                    {
                        item.transform.GetChild(1).transform.GetComponent<Image>().color = disagree;
                    }
                }
        }
        public void RefreshItem()
        {
            ColorHelper(draggingItem, lastSlot);
        }

        public void RemoveItem(ItemData itemData)
        {
            Panel panel = GetPanel(itemData.slotPanelType);

            foreach(Transform item in panel.itemParent)
            {
                if(item.GetComponent<ItemDataMB>().itemData == itemData)
                {
                    SetMatrixThanPanel(item.GetComponent<ItemDataMB>().itemData, false);
                    Destroy(item.gameObject);
                    break;
                }
            }
            panel.itemDataList.Remove(itemData);
        }
    }
}

