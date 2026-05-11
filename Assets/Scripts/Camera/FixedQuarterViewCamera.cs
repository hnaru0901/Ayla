using UnityEngine;

namespace Ayla
{
    public class FixedQuarterViewCamera : MonoBehaviour
    {
        [SerializeField] private Transform target;
        [SerializeField] private Vector3 offset = new Vector3(0f, 8f, -8f);
        [SerializeField] private Vector3 lookOffset = new Vector3(0f, 1f, 0f);

        private void LateUpdate()
        {
            if (target == null)
            {
                return;
            }

            transform.position = target.position + offset;
            transform.LookAt(target.position + lookOffset);
        }
    }
}
