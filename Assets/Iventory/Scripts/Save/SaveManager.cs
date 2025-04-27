using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Spawn;
using Inventory;
using System.IO;
using UI;
using Player.Status;

namespace SaveManager
{
    public class SaveManager : MonoBehaviour
    {
        private SaveData saveData;
        private SaveData loadedData;
        [SerializeField] SpawnManager spawnManager;
        [SerializeField] Transform itemOnGroundParent;

        private void SaveToJson()
        {
            var json = JsonUtility.ToJson(saveData);
            File.WriteAllText(Application.dataPath + "/item.json", json);
            Debug.Log("Saved to: " + Application.dataPath + "/item.json");
        }

        private void LoadFromJson()
        {
            var json = File.ReadAllText(Application.dataPath + "/item.json");
            loadedData = JsonUtility.FromJson<SaveData>(json);
        }

        public void Save() //call from button
        {
            int panelCount = InventoryManager.Instance.panelList.Count;
            int lootDataCount = spawnManager.lootDataList.Count;
            int itemOnGroundCount = itemOnGroundParent.childCount;
            saveData = new SaveData(panelCount, lootDataCount, itemOnGroundCount);

            #region Save Panel
            for (int i = 0; i < panelCount; i++) //moving on panels
            {
                int itemCount = InventoryManager.Instance.panelList[i].itemDataList.Count;
                List<SaveData.S_ItemData> itemList = new List<SaveData.S_ItemData>();
                for (int j = 0; j < itemCount; j++) //moving on items of panel
                {
                    ItemData item = InventoryManager.Instance.panelList[i].itemDataList[j];
                    SaveData.S_ItemData s_itemData = new SaveData.S_ItemData
                        (
                        item.item.GUID,
                        item.isRotated,
                        item.matrixPosition,
                        item.slotPoisiton,
                        (int)item.slotPanelType,
                        item.myLootContainerId,
                        Vector3.zero,
                        Vector3.zero
                        );
                    itemList.Add(s_itemData);
                }
                saveData.panelList[i] = new SaveData.S_Panel((int)InventoryManager.Instance.panelList[i].type, InventoryManager.Instance.panelList[i].size, itemList);
            }
            #endregion Save Panel

            #region Save LootData(Containers)

            int lootDataListCount = spawnManager.lootDataList.Count;

            for (int i = 0; i < lootDataListCount; i++) //moving on containers
            {
                int itemCount = spawnManager.lootDataList[i].itemList.Count;
                List<SaveData.S_ItemData> itemDataList = new List<SaveData.S_ItemData>();

                for (int j = 0; j < itemCount; j++) //moving on items of containers
                {
                    ItemData item = spawnManager.lootDataList[i].itemList[j];
                    SaveData.S_ItemData s_itemData = new SaveData.S_ItemData
                        (
                            item.item.GUID,
                            item.isRotated,
                            item.matrixPosition,
                            item.slotPoisiton,
                            (int)item.slotPanelType,
                            item.myLootContainerId,
                            Vector3.zero,
                            Vector3.zero
                        );
                    itemDataList.Add(s_itemData);
                }
                SaveData.S_LootData s_loot = new SaveData.S_LootData(
                    spawnManager.lootDataList[i].id,
                    spawnManager.lootDataList[i].size,
                    spawnManager.lootDataList[i].frequencyCount,
                    spawnManager.lootDataList[i].maxFrequencyCount,
                    itemDataList,
                    spawnManager.lootDataList[i].matrix,
                    spawnManager.lootDataList[i].isFull
                    );
                saveData.lootDataList[i] = s_loot;
            }

            #endregion Save LootData(Containers)

            #region Save Items On Ground

            for (int i = 0; i < itemOnGroundParent.childCount; i++)
            {
                ItemData item = itemOnGroundParent.GetChild(i).GetComponent<ItemDataMB>().itemData;
                Vector3 position = itemOnGroundParent.GetChild(i).transform.position;
                Vector3 rotation = itemOnGroundParent.GetChild(i).transform.localEulerAngles;
                SaveData.S_ItemData s_item = new SaveData.S_ItemData
                    (
                        item.item.GUID,
                        item.isRotated,
                        item.matrixPosition,
                        item.slotPoisiton,
                        (int)item.slotPanelType,
                        item.myLootContainerId,
                        position,
                        rotation
                    );
                saveData.itemDataListOnGround[i] = s_item;
            }
            #endregion Save Items On Ground

            #region Save Player Status

            saveData.playerStatus[0] = PlayerStatus.Instance.health;
            saveData.playerStatus[1] = PlayerStatus.Instance.energy;
            saveData.playerStatus[2] = PlayerStatus.Instance.water;

            #endregion Saved Player Status

            SaveToJson();
        }

        public void Load()  //call from button
        {
            LoadFromJson();

            int panelCount = loadedData.panelList.Length;

            #region Load Panels

            for (int i = 0; i < panelCount; i++)
            {
                foreach (Panel panel in InventoryManager.Instance.panelList)
                {
                    if ((int)panel.type == loadedData.panelList[i].typeId)
                    {
                        panel.size = loadedData.panelList[i].size;
                        List<ItemData> itemDataList = new List<ItemData>();

                        foreach (SaveData.S_ItemData s_itemData in loadedData.panelList[i].itemList)
                        {
                            ItemData newItemData = new ItemData();
                            newItemData.item = FindItemFromId(s_itemData.itemId);
                            newItemData.isRotated = s_itemData.isRotated;
                            newItemData.slotPoisiton = s_itemData.slotPosition;
                            newItemData.slotPanelType = GetPanelTypeFromId(s_itemData.slotPanelTypeId);
                            newItemData.matrixPosition = s_itemData.matrixPosition;
                            newItemData.myLootContainerId = s_itemData.myLootContainerId;
                            itemDataList.Add(newItemData);
                        }
                        panel.itemDataList = itemDataList; //Updated itemDataList of panel  
                    }
                }
            }

            UIController.Instance.SetCanvasIntentory(false);
            StartCoroutine(InventoryManager.Instance.CreatePanel(InventoryManager.Instance.GetPanel(Panel.Type.Backpack), null)); //creaete items of backpack panel
            InventoryManager.Instance.ClearPanel(Panel.Type.Character);
            InventoryManager.Instance.FillItem(InventoryManager.Instance.GetPanel(Panel.Type.Character), null); //create items of character panel


            #endregion Loaded Panels

            #region Load Containers

            int looCount = loadedData.lootDataList.Length;
            CleatLootData();

            for (int i = 0; i < looCount; i++)
            {
                LootData loot = FindContainerFromId(loadedData.lootDataList[i].id);

                if(loot.id == loadedData.lootDataList[i].id)
                {
                    List<ItemData> itemDataList = new List<ItemData>();

                    foreach (SaveData.S_ItemData s_itemData in loadedData.lootDataList[i].itemList) //moving on items of LootData
                    {
                        ItemData newItemData = new ItemData();
                        newItemData.item = FindItemFromId(s_itemData.itemId);
                        newItemData.isRotated = s_itemData.isRotated;
                        newItemData.slotPoisiton = s_itemData.slotPosition;
                        newItemData.slotPanelType = GetPanelTypeFromId(s_itemData.slotPanelTypeId);
                        newItemData.matrixPosition = s_itemData.matrixPosition;
                        newItemData.myLootContainerId = s_itemData.myLootContainerId;
                        newItemData.myLootContainer = FindContainerGameobjectFromId(s_itemData.myLootContainerId);
                        itemDataList.Add(newItemData);
                    }

                    loot.size = loadedData.lootDataList[i].inventorySize;
                    loot.frequencyCount = loadedData.lootDataList[i].frequencyCount;
                    loot.maxFrequencyCount = loadedData.lootDataList[i].maxFrequencyCount;
                    loot.itemList = itemDataList;
                    loot.matrix = loadedData.lootDataList[i].inventoryMatrix;
                    loot.isFull = loadedData.lootDataList[i].isFull;
                }
            }

            #endregion Loaded Containers

            #region Load Items On Ground

            ClearItemsOnGround();
            
            foreach(SaveData.S_ItemData s_itemData in loadedData.itemDataListOnGround)
            {
                ItemData newItemData = new ItemData();
                newItemData.item = FindItemFromId(s_itemData.itemId);
                newItemData.isRotated = s_itemData.isRotated;
                newItemData.slotPoisiton = s_itemData.slotPosition;
                newItemData.slotPanelType = GetPanelTypeFromId(s_itemData.slotPanelTypeId);
                newItemData.matrixPosition = s_itemData.matrixPosition;
                GameObject newItem = Instantiate(newItemData.item.prefab, s_itemData.position, Quaternion.Euler(s_itemData.rotation), itemOnGroundParent);
                newItem.AddComponent<ItemDataMB>().itemData = newItemData;
            }
            #endregion Loaded Items On Ground

            #region Load Player Status

            PlayerStatus.Instance.SetHealth(loadedData.playerStatus[0]);
            PlayerStatus.Instance.SetEnergy(loadedData.playerStatus[1]);
            PlayerStatus.Instance.SetWater(loadedData.playerStatus[2]);

            #endregion Loaded Player Status
        }

        private Item FindItemFromId(string guid)
        {
            foreach(Item item in spawnManager.itemList)
            {
                if(guid == item.GUID)
                {
                    return item;
                }
            }
            return null;
        }

        private Panel.Type GetPanelTypeFromId(int id)
        {
            foreach (Panel.Type panelType in System.Enum.GetValues(typeof(Panel.Type)))
            {
                if(id == (int)panelType)
                {
                    return panelType;
                }
            }
            return Panel.Type.Character; //will be fix it
        }

        private void CleatLootData()
        {
            foreach (LootData loot in spawnManager.lootDataList)
            {
                loot.size = new Vector2Int(0, 0);
                loot.frequencyCount = null;
                loot.maxFrequencyCount = null;
                loot.itemList = null;
                loot.matrix = null;
                loot.isFull = false;
            }
        }

        private LootData FindContainerFromId(string id)
        {
            foreach (LootData loot in spawnManager.lootDataList)
            {
                if(id == loot.id)
                {
                    return loot;
                }
            }
            return null;
        }

        private GameObject FindContainerGameobjectFromId(string id)
        {
            foreach (LootData loot in spawnManager.lootDataList)
            {
                if(id == loot.id)
                {
                    return loot.gameObject;
                }
            }
            return null;
        }

        private void ClearItemsOnGround()
        {
            foreach (Transform item in itemOnGroundParent)
            {
                Destroy(item.gameObject);
            }
        }
    }
}

