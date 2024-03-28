using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(fileName = "normalitemskin", menuName = "ScriptableObject/normalitemskin")]
public class NormalItemSkin : ScriptableObject
{
    public enum eItemSkinName
    {
        Default,
        Fish
    }

    [System.Serializable]
    public class ItemSkin
    {
        public eItemSkinName skinName;
        public Sprite[] sprites;
    }

    [SerializeField] private ItemSkin[] skins;

    private string[] itemPrefabNames = new string[]{
        Constants.PREFAB_NORMAL_TYPE_ONE,
        Constants.PREFAB_NORMAL_TYPE_TWO,
        Constants.PREFAB_NORMAL_TYPE_THREE,
        Constants.PREFAB_NORMAL_TYPE_FOUR,
        Constants.PREFAB_NORMAL_TYPE_FIVE,
        Constants.PREFAB_NORMAL_TYPE_SIX,
        Constants.PREFAB_NORMAL_TYPE_SEVEN,
    };

    public void SetSkin(eItemSkinName skinName)
    {
        ItemSkin skin = skins.Where(x => x.skinName == skinName).FirstOrDefault();
        if (skin != null)
        {
            for (int i = 0; i < itemPrefabNames.Length && i < skin.sprites.Length; ++i)
            {
                GameObject go = Resources.Load<GameObject>(itemPrefabNames[i]);
                go.GetComponent<SpriteRenderer>().sprite = skin.sprites[i];
            }
        }
    }

}
