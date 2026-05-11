using UnityEngine;

namespace Ayla
{
    [RequireComponent(typeof(PlayerCarryState))]
    public class PlayerInteraction : MonoBehaviour
    {
        [SerializeField] private float interactionRadius = 1.5f;
        [SerializeField] private LayerMask interactionLayers = ~0;

        private readonly Collider[] results = new Collider[8];
        private PlayerCarryState carryState;

        private void Awake()
        {
            carryState = GetComponent<PlayerCarryState>();
        }

        public void TryInteract()
        {
            InteractableTarget target = FindNearestTarget();

            if (target == null)
            {
                return;
            }

            target.Interact(carryState);
        }

        private InteractableTarget FindNearestTarget()
        {
            int hitCount = Physics.OverlapSphereNonAlloc(
                transform.position,
                interactionRadius,
                results,
                interactionLayers,
                QueryTriggerInteraction.Collide);

            InteractableTarget nearest = null;
            float nearestDistance = float.MaxValue;

            for (int i = 0; i < hitCount; i++)
            {
                if (!results[i].TryGetComponent(out InteractableTarget target))
                {
                    continue;
                }

                float distance = Vector3.SqrMagnitude(transform.position - target.transform.position);

                if (distance >= nearestDistance)
                {
                    continue;
                }

                nearest = target;
                nearestDistance = distance;
            }

            return nearest;
        }
    }
}
