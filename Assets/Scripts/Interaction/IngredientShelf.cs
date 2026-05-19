using UnityEngine;

namespace Ayla
{
    public class IngredientShelf : InteractableTarget
    {
        [SerializeField] private IngredientSelectionUI selectionUI;

        public override bool Interact(PlayerCarryState carryState)
        {
            if (carryState == null || selectionUI == null)
            {
                return false;
            }

            if (selectionUI.IsOpen)
            {
                selectionUI.Cancel();
                return true;
            }

            if (!carryState.IsEmpty)
            {
                return false;
            }

            selectionUI.Open(carryState);
            return true;
        }

        // 테스트에서 Inspector 설정 없이 선택 UI 참조를 초기화할 때 사용합니다.
        public void Configure(IngredientSelectionUI selectionUI)
        {
            this.selectionUI = selectionUI;
        }
    }
}
