using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;

namespace Ayla.Tests
{
    public class CarryItemSpriteLibraryTests
    {
        private readonly List<Object> createdObjects = new List<Object>();

        [TearDown]
        public void TearDown()
        {
            foreach (Object createdObject in createdObjects)
            {
                Object.DestroyImmediate(createdObject);
            }

            createdObjects.Clear();
        }

        [Test]
        public void FindSprite_WithRegisteredItem_ReturnsSprite()
        {
            Sprite sprite = CreateSprite();
            CarryItemSpriteLibrary library = CreateLibrary(
                new CarryItemSprite(CarryItemType.WhiteFlower, sprite));

            Assert.That(library.FindSprite(CarryItemType.WhiteFlower), Is.SameAs(sprite));
        }

        [Test]
        public void FindSprite_WithUnregisteredItem_ReturnsNull()
        {
            CarryItemSpriteLibrary library = CreateLibrary(
                new CarryItemSprite(CarryItemType.WhiteFlower, CreateSprite()));

            Assert.That(library.FindSprite(CarryItemType.RedFlower), Is.Null);
        }

        private CarryItemSpriteLibrary CreateLibrary(params CarryItemSprite[] sprites)
        {
            CarryItemSpriteLibrary library = CreateObject("CarryItemSpriteLibrary").AddComponent<CarryItemSpriteLibrary>();
            library.ConfigureForTests(sprites);
            return library;
        }

        private Sprite CreateSprite()
        {
            Texture2D texture = new Texture2D(1, 1);
            createdObjects.Add(texture);

            Sprite sprite = Sprite.Create(texture, new Rect(0f, 0f, 1f, 1f), Vector2.zero);
            createdObjects.Add(sprite);
            return sprite;
        }

        private GameObject CreateObject(string objectName)
        {
            GameObject gameObject = new GameObject(objectName);
            createdObjects.Add(gameObject);
            return gameObject;
        }
    }
}
