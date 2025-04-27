using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "AmmoName", menuName = "Item/New Ammo")]
    public class ItemAmmo : Item
    {
        [Header("Ammo Components")]
        public AmmoType ammoType;
        public int damage;

        public enum AmmoType
        {
            ammo_556,
            ammo_545,
            ammo_919,
            ammo_762
        }
    }
}
