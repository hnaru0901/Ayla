using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Ayla.Tests
{
    public class CraftingStationTests
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
        public void Interact_WithSingleRecipeInput_DepositsItemAndClearsCarry()
        {
            PlayerCarryState carryState = CreateCarryState();
            CraftingStation station = CreateStation(CarryItemType.RawIngredient, CarryItemType.PreparedIngredient, 3f);

            Assert.That(carryState.TrySetItem(CarryItemType.RawIngredient), Is.True);

            bool interacted = station.Interact(carryState);

            Assert.That(interacted, Is.True);
            Assert.That(carryState.IsEmpty, Is.True);
            Assert.That(station.HasInput, Is.True);
            Assert.That(station.Progress, Is.EqualTo(0f));
        }

        [Test]
        public void CancelWork_KeepsProgressForLaterResume()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(CarryItemType.RawIngredient, CarryItemType.PreparedIngredient, 3f);

            carryState.TrySetItem(CarryItemType.RawIngredient);
            station.Interact(carryState);
            station.Interact(carryState, workState);

            station.TickWork(1.25f);
            workState.CancelWork();

            Assert.That(workState.IsWorking, Is.False);
            Assert.That(station.HasInput, Is.True);
            Assert.That(station.IsWorking, Is.False);
            Assert.That(station.Progress, Is.EqualTo(1.25f));
        }

        [Test]
        public void Interact_WhenSingleRecipeComplete_GivesOutputAndResetsStation()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(CarryItemType.RawIngredient, CarryItemType.PreparedIngredient, 3f);

            carryState.TrySetItem(CarryItemType.RawIngredient);
            station.Interact(carryState);
            station.Interact(carryState, workState);
            station.TickWork(3f);

            bool interacted = station.Interact(carryState, workState);

            Assert.That(interacted, Is.True);
            Assert.That(carryState.CurrentItem, Is.EqualTo(CarryItemType.PreparedIngredient));
            Assert.That(station.IsEmpty, Is.True);
            Assert.That(station.Progress, Is.EqualTo(0f));
        }

        [Test]
        public void DisplayItem_ShowsInputUntilWorkCompletesThenOutput()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(CarryItemType.WhiteFlower, CarryItemType.PreparedWhiteFlower, 3f);

            carryState.TrySetItem(CarryItemType.WhiteFlower);
            station.Interact(carryState);

            Assert.That(station.DisplayItem, Is.EqualTo(CarryItemType.WhiteFlower));

            station.Interact(carryState, workState);
            station.TickWork(3f);

            Assert.That(station.DisplayItem, Is.EqualTo(CarryItemType.PreparedWhiteFlower));

            station.Interact(carryState, workState);

            Assert.That(station.DisplayItem, Is.EqualTo(CarryItemType.None));
        }

        [Test]
        public void Interact_WithMultipleFlowerRecipes_PreservesFlowerTypeInOutput()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(
                3f,
                new CraftingStationRecipe(CarryItemType.WhiteFlower, CarryItemType.PreparedWhiteFlower),
                new CraftingStationRecipe(CarryItemType.RedFlower, CarryItemType.PreparedRedFlower));

            carryState.TrySetItem(CarryItemType.RedFlower);
            station.Interact(carryState);
            station.Interact(carryState, workState);
            station.TickWork(3f);
            station.Interact(carryState, workState);

            Assert.That(carryState.CurrentItem, Is.EqualTo(CarryItemType.PreparedRedFlower));
        }

        [Test]
        public void TickWork_WithRecipeDuration_UsesSelectedRecipeDuration()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(
                3f,
                new CraftingStationRecipe(CarryItemType.WhiteFlower, CarryItemType.PreparedWhiteFlower, 3f),
                new CraftingStationRecipe(CarryItemType.RedFlower, CarryItemType.PreparedRedFlower, 5f));

            carryState.TrySetItem(CarryItemType.RedFlower);
            station.Interact(carryState);
            station.Interact(carryState, workState);

            station.TickWork(3f);

            Assert.That(station.IsWorking, Is.True);
            Assert.That(station.NormalizedProgress, Is.EqualTo(0.6f).Within(0.001f));

            station.TickWork(2f);

            Assert.That(station.IsComplete, Is.True);
        }

        [Test]
        public void Interact_WithAutoStartStation_StartsWorkAfterDeposit()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(CarryItemType.PreparedWhiteFlower, CarryItemType.WhitePotionBase, 3f);

            station.SetAutoStartWorkForTests(true);
            carryState.TrySetItem(CarryItemType.PreparedWhiteFlower);
            bool interacted = station.Interact(carryState, workState);

            Assert.That(interacted, Is.True);
            Assert.That(carryState.IsEmpty, Is.True);
            Assert.That(station.IsWorking, Is.True);
            Assert.That(workState.IsWorking, Is.False);
        }

        [Test]
        public void Interact_WithAutoStartStation_CompletesWithoutPlayerWorkState()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(CarryItemType.PreparedWhiteFlower, CarryItemType.WhitePotionBase, 3f);

            station.SetAutoStartWorkForTests(true);
            carryState.TrySetItem(CarryItemType.PreparedWhiteFlower);
            station.Interact(carryState, workState);
            station.TickWork(3f);

            Assert.That(station.IsComplete, Is.True);
            Assert.That(workState.IsWorking, Is.False);
        }

        [Test]
        public void NormalizedProgress_ReportsProgressBetweenEmptyAndComplete()
        {
            PlayerCarryState carryState = CreateCarryState();
            PlayerWorkState workState = CreateWorkState();
            CraftingStation station = CreateStation(CarryItemType.RawIngredient, CarryItemType.PreparedIngredient, 4f);

            carryState.TrySetItem(CarryItemType.RawIngredient);
            station.Interact(carryState);
            station.Interact(carryState, workState);
            station.TickWork(1f);

            Assert.That(station.NormalizedProgress, Is.EqualTo(0.25f));
        }

        [Test]
        public void FormatProgressLabel_ShowsPercentUntilComplete()
        {
            Assert.That(CraftingStationIndicator.FormatProgressLabel(0f, false), Is.EqualTo("0%"));
            Assert.That(CraftingStationIndicator.FormatProgressLabel(0.375f, false), Is.EqualTo("38%"));
            Assert.That(CraftingStationIndicator.FormatProgressLabel(1f, true), Is.EqualTo("Done"));
        }

        private PlayerCarryState CreateCarryState()
        {
            return CreateObject("PlayerCarryState").AddComponent<PlayerCarryState>();
        }

        private PlayerWorkState CreateWorkState()
        {
            return CreateObject("PlayerWorkState").AddComponent<PlayerWorkState>();
        }

        private CraftingStation CreateStation(CarryItemType inputItem, CarryItemType outputItem, float workDuration)
        {
            CraftingStation station = CreateObject("CraftingStation").AddComponent<CraftingStation>();
            station.Configure(inputItem, outputItem, workDuration);
            return station;
        }

        private CraftingStation CreateStation(float workDuration, params CraftingStationRecipe[] recipes)
        {
            CraftingStation station = CreateObject("CraftingStation").AddComponent<CraftingStation>();
            station.Configure(workDuration, recipes);
            return station;
        }

        private GameObject CreateObject(string objectName)
        {
            GameObject gameObject = new GameObject(objectName);
            createdObjects.Add(gameObject);
            return gameObject;
        }
    }
}
