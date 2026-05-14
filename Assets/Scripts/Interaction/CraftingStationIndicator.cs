using UnityEngine;

namespace Ayla
{
    public class CraftingStationIndicator : MonoBehaviour
    {
        [SerializeField] private CraftingStation station;
        [SerializeField] private Renderer targetRenderer;
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
            if (targetRenderer == null || station == null)
            {
                return;
            }

            bool shouldShow = !station.IsEmpty;
            targetRenderer.enabled = shouldShow;

            if (!shouldShow)
            {
                SetColor(emptyColor);
                return;
            }

            SetColor(Color.Lerp(startColor, completeColor, station.NormalizedProgress));
        }

        private void SetColor(Color color)
        {
            targetRenderer.material.color = color;
        }
    }
}
