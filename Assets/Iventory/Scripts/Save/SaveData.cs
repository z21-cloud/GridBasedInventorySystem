using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class SaveData
{
    // These 3 array list will save
    public S_Panel[] panelList;
    public S_LootData[] lootDataList;
    public S_ItemData[] itemDataListOnGround;
    public int[] playerStatus = new int[3]; //[0] = health, [1] = energy, [2] = water;
    //
    public SaveData(int panelCount, int lootDataCount, int itemGroundCount) // Constructor
    {
        panelList = new S_Panel[panelCount];
        lootDataList = new S_LootData[lootDataCount];
        itemDataListOnGround = new S_ItemData[itemGroundCount];
    }

    [Serializable]
    public class S_Panel // = Panel class
    {
        public int typeId;
        public Vector2Int size;
        public List<S_ItemData> itemList;

        public S_Panel(int typeId, Vector2Int size, List<S_ItemData> itemList) // Constructor
        {
            this.itemList = new List<S_ItemData>(itemList.Count);
            this.typeId = typeId;
            this.size = size;
            this.itemList = itemList;
        }
    }

    [Serializable]
    public class S_LootData // = LootData class
    {
        public string id;
        public Vector2Int inventorySize;
        public int[] frequencyCount;
        public int[] maxFrequencyCount;
        public List<S_ItemData> itemList;
        public bool[,] inventoryMatrix;
        public bool isFull;
        public S_LootData(string id, Vector2Int inventorySize, int[] frequencyCount, int[] maxFrequencyCount, List<S_ItemData> itemList, bool[,] inventoryMatrix, bool isFull) // Constructor
        {
            this.itemList = new List<S_ItemData>(itemList.Count);
            this.id = id;
            this.inventorySize = inventorySize;
            this.frequencyCount = frequencyCount;
            this.maxFrequencyCount = maxFrequencyCount;
            this.itemList = itemList;
            this.inventoryMatrix = inventoryMatrix;
            this.isFull = isFull;
        }
    }

    [Serializable]
    public class S_ItemData // = ItemData class
    {
        public string itemId;
        public bool isRotated;
        public Vector2Int matrixPosition;
        public Vector3 slotPosition;
        public int slotPanelTypeId;
        public string myLootContainerId;
        public Vector3 position;
        public Vector3 rotation;

        public S_ItemData(string itemId, bool isRotated, Vector2Int matrixPosition, Vector3 slotPosition, int slotPanelTypeId, string myLootContainerId, Vector3 position, Vector3 rotation ) // Constructor
        {
            this.itemId = itemId;
            this.isRotated = isRotated;
            this.matrixPosition = matrixPosition;
            this.slotPosition = slotPosition;
            this.slotPanelTypeId = slotPanelTypeId;
            this.myLootContainerId = myLootContainerId;
            this.position = position;
            this.rotation = rotation;
        }
    }
}
