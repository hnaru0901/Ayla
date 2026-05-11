using UnityEngine;

namespace Ayla
{
    public class CarryMarker : MonoBehaviour
    {
        [SerializeField] private PlayerCarryState carryState;
        [SerializeField] private GameObject marker;

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

            marker.SetActive(item != CarryItemType.None);
        }
    }
}
