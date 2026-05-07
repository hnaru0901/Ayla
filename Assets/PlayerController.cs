using UnityEngine;
using UnityEngine.InputSystem;

namespace Ayla
{
    [RequireComponent(typeof(CharacterController))]
    public class PlayerController : MonoBehaviour
    {
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float turnSpeed = 720f;

        private CharacterController characterController;

        private void Awake()
        {
            characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            Vector2 input = ReadKeyboardInput();
            Vector3 movement = PlayerMovementInput.ToWorldDirection(input);

            if (movement.sqrMagnitude <= 0f)
            {
                return;
            }

            characterController.Move(movement * moveSpeed * Time.deltaTime);

            Quaternion targetRotation = Quaternion.LookRotation(movement, Vector3.up);
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                targetRotation,
                turnSpeed * Time.deltaTime);
        }

        private static Vector2 ReadKeyboardInput()
        {
            Keyboard keyboard = Keyboard.current;

            if (keyboard == null)
            {
                return Vector2.zero;
            }

            float horizontal = 0f;
            float vertical = 0f;

            if (keyboard.aKey.isPressed || keyboard.leftArrowKey.isPressed)
            {
                horizontal -= 1f;
            }

            if (keyboard.dKey.isPressed || keyboard.rightArrowKey.isPressed)
            {
                horizontal += 1f;
            }

            if (keyboard.sKey.isPressed || keyboard.downArrowKey.isPressed)
            {
                vertical -= 1f;
            }

            if (keyboard.wKey.isPressed || keyboard.upArrowKey.isPressed)
            {
                vertical += 1f;
            }

            return new Vector2(horizontal, vertical);
        }
    }
}
