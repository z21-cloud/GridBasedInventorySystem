using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "ClothName", menuName = "Item/New Cloth")]
    public class ItemCloth : Item
    {
        [Header("Cloth Components")]
        public ClothType clothType;
        public int health;
        public int armor;

        public enum ClothType
        {
            Headset,
            Headwear,
            FaceCover,
            TacticalRig,
            BodyArmor,
            Backpack
        }
    }
}

