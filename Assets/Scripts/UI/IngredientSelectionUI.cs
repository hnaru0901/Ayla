using UnityEngine;
using UnityEngine.InputSystem;

namespace Ayla
{
    public class IngredientSelectionUI : MonoBehaviour
    {
        [SerializeField] private GameObject panel;

        private PlayerCarryState currentCarryState;

        public bool IsOpen => currentCarryState != null;

        public void Open(PlayerCarryState carryState)
        {
            if (carryState == null || !carryState.IsEmpty)
            {
                return;
            }

            currentCarryState = carryState;
            SetPanelActive(true);
        }

        public void Cancel()
        {
            currentCarryState = null;
            SetPanelActive(false);
        }

        public bool SelectWhiteFlower()
        {
            return TrySelect(CarryItemType.WhiteFlower);
        }

        public bool SelectRedFlower()
        {
            return TrySelect(CarryItemType.RedFlower);
        }

        private void Awake()
        {
            SetPanelActive(false);
        }

        private void Update()
        {
            if (!IsOpen)
            {
                return;
            }

            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return;
            }

            if (keyboard.qKey.wasPressedThisFrame)
            {
                SelectWhiteFlower();
                return;
            }

            if (keyboard.eKey.wasPressedThisFrame)
            {
                SelectRedFlower();
                return;
            }

            if (keyboard.escapeKey.wasPressedThisFrame)
            {
                Cancel();
            }
        }

        private bool TrySelect(CarryItemType item)
        {
            if (currentCarryState == null || !currentCarryState.TrySetItem(item))
            {
                return false;
            }

            Cancel();
            return true;
        }

        private void SetPanelActive(bool isActive)
        {
            if (panel != null)
            {
                panel.SetActive(isActive);
            }
        }
    }
}
