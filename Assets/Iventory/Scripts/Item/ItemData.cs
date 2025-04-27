using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [Serializable] //save system
    public class ItemData
    {
        public Item item;
        public bool isRotated = false;
        public Vector2Int matrixPosition;
        public Vector3 slotPoisiton = Vector3.zero;
        public Panel.Type slotPanelType;
        public GameObject myLootContainer;
        public string myLootContainerId;
    }
}

