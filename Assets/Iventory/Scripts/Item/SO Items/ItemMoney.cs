using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Inventory
{
    [CreateAssetMenu(fileName = "MoneyName", menuName = "Item/New Money")]
    public class ItemMoney : Item
    {
        [Header("Money Components")]
        public MoneyType moneyType;
        public string moneySymbol;
        public int moneyAmount;
        
        public enum MoneyType
        {
            Dollar,
            Euro,
            Rub
        }
    }
}

