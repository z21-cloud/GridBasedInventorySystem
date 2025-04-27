using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    public class LootData : MonoBehaviour
    {
        [Header("For Save")] public string id = System.Guid.NewGuid().ToString();
        [Header("X: Row, Y: Column")] public Vector2Int size;

        public int[] frequencyCount = new int[System.Enum.GetValues(typeof(Frequency)).Length];
        public int[] maxFrequencyCount = new int[System.Enum.GetValues(typeof(Frequency)).Length];

        public List<ItemData> itemList = new List<ItemData>();
        public bool[,] matrix;
        public bool isFull = false;

        private void Awake()
        {
            matrix = new bool[size.x, size.y];
        }
    }
}

