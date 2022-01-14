using Message;
using System;

namespace I
{
    public static class ItemHelper
    {
        public static ItemType GetItemType(int itemId)
        {
            return (ItemType)(itemId / 100000);
        }
        public static CurrencyType GetCurrencyType(int itemId)
        {
            H.Ensure(GetItemType(itemId) == ItemType.Currency, Code.ItemNotCurrency);
            return (CurrencyType)(itemId);
        }
    }
}