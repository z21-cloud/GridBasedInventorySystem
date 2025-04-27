using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using UnityEngine;

namespace Inventory
{
    public abstract class Item : ScriptableObject
    {
        [Header("Main Item Components")]
        public string itemName;
        [Multiline] public string description;
        public Vector2Int slotSize;
        public Sprite image;
        public Color background;
        public GameObject prefab;

        [Header("Type's Item Components")]
        [SerializeField] private ItemType _type;
        [SerializeField] private SlotType _slotType;
        [SerializeField] private Frequency _frequency;

        [Header("GUID")]
        [SerializeField] private string _guid = System.Guid.NewGuid().ToString();

        public ItemType Type => _type;
        public SlotType SlotType => _slotType;
        public Frequency SpawnFrequency => _frequency;
        public string GUID => _guid;

        public virtual void Use()
        {
            Debug.Log($"Используем предмет: {itemName}");
        }
    }
}

