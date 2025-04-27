using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "WeaponName", menuName = "Item/New Weapon")]
    public class ItemWeapon : Item
    {
        [Header("Weapon Components")]
        public WeaponType weaponTYpe;
        public ItemMagazine magazine;
        public int effectiveRange;

        public enum WeaponType
        {
            Assault,
            Sniper,
            Pistol,
            Knife
        }
    }
}

