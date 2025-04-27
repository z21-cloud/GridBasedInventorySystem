using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Inventory;
using System.Linq.Expressions;

namespace Spawn
{
    public class SpawnManager : MonoBehaviour
    {
        private int[] ratio = new int[System.Enum.GetValues(typeof(Frequency)).Length];
        public List<LootData> lootDataList = new List<LootData>();
        public List<Item> itemList = new List<Item>();
        List<Item> itemListFrequency1 = new List<Item>();
        List<Item> itemListFrequency5 = new List<Item>();
        List<Item> itemListFrequency10 = new List<Item>();
        List<Item> itemListFrequency25 = new List<Item>();
        List<Item> itemListFrequency50 = new List<Item>();
        List<List<Item>> itemListFrequency = new List<List<Item>>(System.Enum.GetValues(typeof(Frequency)).Length);

        void Start()
        {
            GetRatio();
            DivideList();
            Spawn();
        }

        private void GetRatio()
        {
            int i = 0;
            foreach (Frequency frequencyType in System.Enum.GetValues(typeof(Frequency)))
            {
                ratio[i] = (int)frequencyType;
                i++;
            }
        }

        private void DivideList()
        {
            itemListFrequency.Add(itemListFrequency1);
            itemListFrequency.Add(itemListFrequency5);
            itemListFrequency.Add(itemListFrequency10);
            itemListFrequency.Add(itemListFrequency25);
            itemListFrequency.Add(itemListFrequency50);

            foreach (Item item in itemList)
            {
                switch (item.SpawnFrequency)
                {
                    case Frequency.one:
                        {
                            itemListFrequency1.Add(item);
                            break;
                        }
                    case Frequency.five:
                        {
                            itemListFrequency5.Add(item);
                            break;
                        }
                    case Frequency.ten:
                        {
                            itemListFrequency10.Add(item);
                            break;
                        }
                    case Frequency.twenty_five:
                        {
                            itemListFrequency25.Add(item);
                            break;
                        }
                    case Frequency.fifty:
                        {
                            itemListFrequency50.Add(item);
                            break;
                        }
                }
            }
        }
        public void Spawn()
        {
            foreach (LootData lootData in lootDataList) //moving in containers
            {
                int frequencyCount = lootData.frequencyCount.Length;

                for(int i = 0; i < frequencyCount; i++) //[0] = %1, [1] = %5, [2] = %10 ... //moving in probability
                {
                    for (int j = 0; j < lootData.maxFrequencyCount[i]; j++) //moving in probability's count
                    {
                        if(Random.Range(1, 100) <= ratio[i] && !lootData.isFull && lootData.frequencyCount[i] < lootData.maxFrequencyCount[i])
                        {
                            int random = Random.Range(0, itemListFrequency[i].Count);
                            ItemData newItemData = new ItemData();
                            newItemData.item = itemListFrequency[i][random];
                            newItemData.slotPanelType = Panel.Type.Loot;
                            SearchEmptyPlaceInMatrix(lootData, newItemData);
                        }
                    }
                }
            }
        }

        private void SearchEmptyPlaceInMatrix(LootData lootData, ItemData itemData)
        {
            itemData.myLootContainer = lootData.transform.gameObject;
            
            int row = lootData.matrix.GetLength(0);
            int column = lootData.matrix.GetLength(1);
            Vector2Int itemSize = itemData.item.slotSize;

            for (int i = 0; i < row; i++) //moving in rows
            {
                for (int j = 0; j < column; j++) //moving in columns
                {
                    if(i + itemSize.x <= row && j + itemSize.y <= column) //is there enough area. NO ROTATE
                    {
                        bool status = false;

                        for (int x = 0; x < itemSize.x; x++)
                        {
                            for (int y = 0; y < itemSize.y; y++)
                            {
                                if (lootData.matrix[i + x, j + y] == true) //slot is full
                                {
                                    status = true; 
                                }
                            }
                        }

                        if(status == false) //avaible
                        {
                            AddItem(lootData, new Vector2Int(i, j), itemData);
                            return;
                        }
                    }

                    else if(i + itemSize.y <= row && j + itemSize.x <= column) //is there enough area. ROTATED
                    {
                        bool status = false;

                        for (int x = 0; x < itemSize.y; x++)
                        {
                            for (int y = 0; y < itemSize.x; y++)
                            {
                                if (lootData.matrix[i + x, j + y] == true) //slot is full
                                {
                                    status = true;
                                }
                            }
                        }

                        if (status == false) //avaible
                        {
                            itemData.isRotated = true;
                            AddItem(lootData, new Vector2Int(i, j), itemData);
                            return;
                        }
                    }
                }
            }
        }

        private void AddItem(LootData lootData, Vector2Int matrixPosition, ItemData itemData)
        {
            Debug.Log($"Item {itemData.item.itemName} spawned at {matrixPosition} " +
          $"(Size: {itemData.item.slotSize}, Rotated: {itemData.isRotated})");
            itemData.matrixPosition = matrixPosition;
            lootData.itemList.Add(itemData); //spawned into container;
            Vector2Int itemSize = itemData.item.slotSize;

            if(!itemData.isRotated) //if no rotated
            {
                for (int i = 0; i < itemSize.x; i++)
                {
                    for (int j = 0; j < itemSize.y; j++)
                    {
                        lootData.matrix[matrixPosition.x + i, matrixPosition.y + j] = true; //item's slots are made true
                    }
                }
            }
            else //if rotated
            {
                for (int i = 0; i < itemSize.y; i++)
                {
                    for (int j = 0; j < itemSize.x; j++)
                    {
                        lootData.matrix[matrixPosition.x + i, matrixPosition.y + j] = true; //item's slots are made true
                    }
                }
            }

            int f = 0;
            foreach(Frequency frequencyType in System.Enum.GetValues(typeof(Frequency)))
            {
                if(frequencyType == itemData.item.SpawnFrequency)
                {
                    lootData.frequencyCount[f] += 1;
                    break;
                }
                f++;
            }
        }
    }
}

