using TMPro;
using UnityEngine;

namespace Ayla
{
    public class CraftingStationIndicator : MonoBehaviour
    {
        [SerializeField] private CraftingStation station;
        [SerializeField] private Renderer targetRenderer;
        [SerializeField] private TMP_Text progressText;
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
            UpdateProgressText(shouldShow);

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

        private void SetColor(Color color)
        {
            targetRenderer.material.color = color;
        }
    }
}
