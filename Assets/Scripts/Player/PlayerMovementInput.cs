using UnityEngine;

namespace Ayla
{
    public static class PlayerMovementInput
    {
        public static Vector3 ToWorldDirection(Vector2 input)
        {
            Vector2 clampedInput = Vector2.ClampMagnitude(input, 1f);
            return new Vector3(clampedInput.x, 0f, clampedInput.y);
        }
    }
}
