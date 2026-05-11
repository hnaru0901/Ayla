using UnityEngine;

namespace Ayla
{
    public abstract class InteractableTarget : MonoBehaviour
    {
        [SerializeField] private string interactionName = "Interactable";

        public string InteractionName => interactionName;

        public abstract bool Interact(PlayerCarryState carryState);
    }
}
