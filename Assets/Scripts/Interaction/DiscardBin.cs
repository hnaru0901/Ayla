namespace Ayla
{
    public class DiscardBin : InteractableTarget
    {
        public override bool Interact(PlayerCarryState carryState)
        {
            if (carryState == null || carryState.IsEmpty)
            {
                return false;
            }

            carryState.Clear();
            return true;
        }
    }
}
