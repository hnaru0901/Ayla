using UnityEngine;
using UnityEngine.UI;

namespace Ayla
{
    public class CarryHud : MonoBehaviour
    {
        [System.Serializable]
        private struct CarryItemIcon
        {
            public CarryItemType item;
            public Sprite sprite;
        }

        [SerializeField] private PlayerCarryState carryState;
        [SerializeField] private Text heldText;
        [SerializeField] private Image heldImage;
        [SerializeField] private CarryItemSpriteLibrary spriteLibrary;
        [SerializeField] private CarryItemIcon[] itemIcons;

        private void OnEnable()
        {
            if (carryState != null)
            {
                carryState.ItemChanged += UpdateView;
            }

            UpdateView(carryState != null ? carryState.CurrentItem : CarryItemType.None);
        }

        private void OnDisable()
        {
            if (carryState != null)
            {
                carryState.ItemChanged -= UpdateView;
            }
        }

        private void UpdateView(CarryItemType item)
        {
            if (heldText != null)
            {
                heldText.text = $"Held: {item}";
            }

            if (heldImage == null)
            {
                return;
            }

            Sprite sprite = FindSprite(item);
            heldImage.sprite = sprite;
            heldImage.enabled = sprite != null;
        }

        private Sprite FindSprite(CarryItemType item)
        {
            if (spriteLibrary != null)
            {
                return spriteLibrary.FindSprite(item);
            }

            if (item == CarryItemType.None || itemIcons == null)
            {
                return null;
            }

            for (int i = 0; i < itemIcons.Length; i++)
            {
                if (itemIcons[i].item == item)
                {
                    return itemIcons[i].sprite;
                }
            }

            return null;
        }
    }
}
