namespace Ayla
{
    public class RawIngredientSource : InteractableTarget
    {
        public override bool Interact(PlayerCarryState carryState)
        {
            return carryState != null && carryState.TrySetItem(CarryItemType.RawIngredient);
        }
    }
}
