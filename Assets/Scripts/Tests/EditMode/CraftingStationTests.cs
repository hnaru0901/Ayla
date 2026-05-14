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
        public void Interact_WithInputItem_DepositsItemAndClearsCarry()
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
        public void Interact_WhenComplete_GivesOutputAndResetsStation()
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

        private GameObject CreateObject(string objectName)
        {
            GameObject gameObject = new GameObject(objectName);
            createdObjects.Add(gameObject);
            return gameObject;
        }
    }
}
