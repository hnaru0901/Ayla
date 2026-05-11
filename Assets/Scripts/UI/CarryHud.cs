using UnityEngine;
using UnityEngine.UI;

namespace Ayla
{
    public class CarryHud : MonoBehaviour
    {
        [SerializeField] private PlayerCarryState carryState;
        [SerializeField] private Text heldText;

        private void OnEnable()
        {
            if (carryState != null)
            {
                carryState.ItemChanged += UpdateText;
            }

            UpdateText(carryState != null ? carryState.CurrentItem : CarryItemType.None);
        }

        private void OnDisable()
        {
            if (carryState != null)
            {
                carryState.ItemChanged -= UpdateText;
            }
        }

        private void UpdateText(CarryItemType item)
        {
            if (heldText == null)
            {
                return;
            }

            heldText.text = $"Held: {item}";
        }
    }
}
