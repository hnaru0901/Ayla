using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Ayla.Tests
{
    public class IngredientSelectionTests
    {
        private readonly List<GameObject> createdObjects = new List<GameObject>();

        [TearDown]
        public void TearDown()
        {
            foreach (GameObject createdObject in createdObjects)
            {
                Object.DestroyImmediate(createdObject);
            }

            createdObjects.Clear();
        }

        [Test]
        public void Interact_WithEmptyCarry_OpensIngredientSelection()
        {
            PlayerCarryState carryState = CreateCarryState();
            IngredientSelectionUI selectionUI = CreateSelectionUI();
            IngredientShelf shelf = CreateShelf(selectionUI);

            bool interacted = shelf.Interact(carryState);

            Assert.That(interacted, Is.True);
            Assert.That(selectionUI.IsOpen, Is.True);
            Assert.That(carryState.IsEmpty, Is.True);
        }

        [Test]
        public void SelectRedFlower_WhenOpen_SetsCarryAndCloses()
        {
            PlayerCarryState carryState = CreateCarryState();
            IngredientSelectionUI selectionUI = CreateSelectionUI();

            selectionUI.Open(carryState);
            bool selected = selectionUI.SelectRedFlower();

            Assert.That(selected, Is.True);
            Assert.That(carryState.CurrentItem, Is.EqualTo(CarryItemType.RedFlower));
            Assert.That(selectionUI.IsOpen, Is.False);
        }

        [Test]
        public void Interact_WhenSelectionIsOpen_CancelsIngredientSelection()
        {
            PlayerCarryState carryState = CreateCarryState();
            IngredientSelectionUI selectionUI = CreateSelectionUI();
            IngredientShelf shelf = CreateShelf(selectionUI);

            shelf.Interact(carryState);
            bool interacted = shelf.Interact(carryState);

            Assert.That(interacted, Is.True);
            Assert.That(selectionUI.IsOpen, Is.False);
            Assert.That(carryState.IsEmpty, Is.True);
        }

        [Test]
        public void Interact_WithHeldItem_DoesNotOpenIngredientSelection()
        {
            PlayerCarryState carryState = CreateCarryState();
            IngredientSelectionUI selectionUI = CreateSelectionUI();
            IngredientShelf shelf = CreateShelf(selectionUI);

            carryState.TrySetItem(CarryItemType.WhiteFlower);
            bool interacted = shelf.Interact(carryState);

            Assert.That(interacted, Is.False);
            Assert.That(selectionUI.IsOpen, Is.False);
        }

        private PlayerCarryState CreateCarryState()
        {
            return CreateObject("PlayerCarryState").AddComponent<PlayerCarryState>();
        }

        private IngredientSelectionUI CreateSelectionUI()
        {
            return CreateObject("IngredientSelectionUI").AddComponent<IngredientSelectionUI>();
        }

        private IngredientShelf CreateShelf(IngredientSelectionUI selectionUI)
        {
            IngredientShelf shelf = CreateObject("IngredientShelf").AddComponent<IngredientShelf>();
            shelf.Configure(selectionUI);
            return shelf;
        }

        private GameObject CreateObject(string objectName)
        {
            GameObject gameObject = new GameObject(objectName);
            createdObjects.Add(gameObject);
            return gameObject;
        }
    }
}
