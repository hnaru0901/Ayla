using System;
using UnityEngine;

namespace Ayla
{
    public class PlayerCarryState : MonoBehaviour
    {
        [SerializeField] private CarryItemType currentItem = CarryItemType.None;

        public event Action<CarryItemType> ItemChanged;

        public CarryItemType CurrentItem => currentItem;
        public bool IsEmpty => currentItem == CarryItemType.None;

        public bool TrySetItem(CarryItemType item)
        {
            if (item == CarryItemType.None || !IsEmpty)
            {
                return false;
            }

            currentItem = item;
            ItemChanged?.Invoke(currentItem);
            return true;
        }

        public void Clear()
        {
            if (IsEmpty)
            {
                return;
            }

            currentItem = CarryItemType.None;
            ItemChanged?.Invoke(currentItem);
        }
    }
}
