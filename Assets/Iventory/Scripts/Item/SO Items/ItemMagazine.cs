using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "MagazineName", menuName = "Item/New Magazine")]
    public class ItemMagazine : Item
    {
        [Header("Magazine Components")]
        public ItemAmmo.AmmoType ammoTYpe;
        public int ammoCapacity;
    }
}

