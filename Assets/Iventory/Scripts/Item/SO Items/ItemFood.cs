using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Player.Status;

namespace Inventory
{
    [CreateAssetMenu(fileName = "FoodName", menuName = "Item/New Food")]
    public class ItemFood : Item
    {
        [Header("Food Components")]
        public FoodType foodType;

        [Header("Restoration Settings")]
        public int energyRestoration;
        public int waterRestoration;
        //

        public enum FoodType
        {
            Food,
            Drink
        }

        public override void Use()
        {
            base.Use();
            PlayerStatus.Instance.SetEnergy(energyRestoration);
            Debug.Log($"{itemName} ��� ������. ������������� �������: {energyRestoration}");
            PlayerStatus.Instance.SetWater(waterRestoration);
            Debug.Log($"{itemName} ��� �����. ������������� ����: {waterRestoration}");
        }
    }
}

