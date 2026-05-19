using UnityEngine;

namespace Ayla
{
    [System.Serializable]
    public struct CarryItemSprite
    {
        [SerializeField] private CarryItemType item;
        [SerializeField] private Sprite sprite;

        public CarryItemSprite(CarryItemType item, Sprite sprite)
        {
            this.item = item;
            this.sprite = sprite;
        }

        public CarryItemType Item => item;
        public Sprite Sprite => sprite;
    }

    public class CarryItemSpriteLibrary : MonoBehaviour
    {
        [SerializeField] private CarryItemSprite[] itemSprites;

        public Sprite FindSprite(CarryItemType item)
        {
            if (item == CarryItemType.None || itemSprites == null)
            {
                return null;
            }

            for (int i = 0; i < itemSprites.Length; i++)
            {
                if (itemSprites[i].Item == item)
                {
                    return itemSprites[i].Sprite;
                }
            }

            return null;
        }

        // 테스트에서 Inspector 설정 없이 아이템 스프라이트 목록을 초기화할 때 사용합니다.
        public void ConfigureForTests(params CarryItemSprite[] itemSprites)
        {
            this.itemSprites = itemSprites;
        }
    }
}
