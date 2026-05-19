using TMPro;
using UnityEngine;

namespace Ayla
{
    public class CraftingStationIndicator : MonoBehaviour
    {
        [System.Serializable]
        private struct LocalCarryItemSprite
        {
            public CarryItemType item;
            public Sprite sprite;
        }

        [SerializeField] private CraftingStation station;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private SpriteRenderer itemRenderer;
        [SerializeField] private TMP_Text progressText;
        [SerializeField] private CarryItemSpriteLibrary spriteLibrary;
        [SerializeField] private LocalCarryItemSprite[] itemSprites;
        [SerializeField] private Color emptyColor = Color.clear;
        [SerializeField] private Color startColor = Color.red;
        [SerializeField] private Color completeColor = Color.blue;

        private void Awake()
        {
            if (station == null)
            {
                station = GetComponentInParent<CraftingStation>();
            }

            if (targetRenderer == null)
            {
                targetRenderer = GetComponent<Renderer>();
            }
        }

        private void Update()
        {
            UpdateIndicator();
        }

        private void UpdateIndicator()
        {
            if (station == null)
            {
                return;
            }

            bool shouldShow = !station.IsEmpty;
            if (targetRenderer != null)
            {
                targetRenderer.enabled = shouldShow;
            }

            UpdateProgressText(shouldShow);
            UpdateItemRenderer(shouldShow);

            if (!shouldShow)
            {
                SetColor(emptyColor);
                return;
            }

            SetColor(Color.Lerp(startColor, completeColor, station.NormalizedProgress));
        }

        public static string FormatProgressLabel(float normalizedProgress, bool isComplete)
        {
            if (isComplete)
            {
                return "PickUp!!";
            }

            int percent = Mathf.RoundToInt(Mathf.Clamp01(normalizedProgress) * 100f);
            return $"{percent}%";
        }

        private void UpdateProgressText(bool shouldShow)
        {
            if (progressText == null)
            {
                return;
            }

            progressText.gameObject.SetActive(shouldShow);

            if (!shouldShow)
            {
                return;
            }

            progressText.text = FormatProgressLabel(station.NormalizedProgress, station.IsComplete);
        }

        private void UpdateItemRenderer(bool shouldShow)
        {
            if (itemRenderer == null)
            {
                return;
            }

            Sprite sprite = shouldShow ? FindSprite(station.DisplayItem) : null;
            itemRenderer.sprite = sprite;
            itemRenderer.enabled = sprite != null;
        }

        private Sprite FindSprite(CarryItemType item)
        {
            if (spriteLibrary != null)
            {
                return spriteLibrary.FindSprite(item);
            }

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

        private void SetColor(Color color)
        {
            if (targetRenderer != null)
            {
                targetRenderer.material.color = color;
            }
        }
    }
}
