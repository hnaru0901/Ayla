using UnityEngine;

namespace Ayla
{
    public class CarryMarker : MonoBehaviour
    {
        [System.Serializable]
        private struct CarryItemSprite
        {
            public CarryItemType item;
            public Sprite sprite;
        }

        [SerializeField] private PlayerCarryState carryState;
        [SerializeField] private GameObject marker;
        [SerializeField] private SpriteRenderer markerRenderer;
        [SerializeField] private CarryItemSprite[] itemSprites;

        private void OnEnable()
        {
            if (carryState != null)
            {
                carryState.ItemChanged += UpdateMarker;
            }

            UpdateMarker(carryState != null ? carryState.CurrentItem : CarryItemType.None);
        }

        private void OnDisable()
        {
            if (carryState != null)
            {
                carryState.ItemChanged -= UpdateMarker;
            }
        }

        private void UpdateMarker(CarryItemType item)
        {
            if (marker == null)
            {
                return;
            }

            Sprite sprite = FindSprite(item);
            marker.SetActive(sprite != null);

            if (markerRenderer != null)
            {
                markerRenderer.sprite = sprite;
            }
        }

        private Sprite FindSprite(CarryItemType item)
        {
            if (item == CarryItemType.None || itemSprites == null)
            {
                return null;
            }

            for (int i = 0; i < itemSprites.Length; i++)
            {
                if (itemSprites[i].item == item)
                {
                    return itemSprites[i].sprite;
                }
            }

            return null;
        }
    }
}
