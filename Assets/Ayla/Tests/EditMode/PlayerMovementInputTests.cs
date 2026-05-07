using NUnit.Framework;
using UnityEngine;

namespace Ayla.Tests
{
    public class PlayerMovementInputTests
    {
        [Test]
        public void NormalizeInput_ClampsDiagonalMovement()
        {
            Vector3 movement = PlayerMovementInput.ToWorldDirection(new Vector2(1f, 1f));

            Assert.That(movement.magnitude, Is.EqualTo(1f).Within(0.0001f));
        }

        [Test]
        public void NormalizeInput_UsesHorizontalInputOnWorldX()
        {
            Vector3 movement = PlayerMovementInput.ToWorldDirection(new Vector2(1f, 0f));

            Assert.That(movement, Is.EqualTo(Vector3.right));
        }

        [Test]
        public void NormalizeInput_UsesVerticalInputOnWorldZ()
        {
            Vector3 movement = PlayerMovementInput.ToWorldDirection(new Vector2(0f, 1f));

            Assert.That(movement, Is.EqualTo(Vector3.forward));
        }
    }
}
