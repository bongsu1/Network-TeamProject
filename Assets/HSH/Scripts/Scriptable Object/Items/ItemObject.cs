using UnityEngine;

public enum ItemType
{
    Food,
    Helmet,
    Weapon,
    Shield,
    Boots,
    Chest,
    Tool,
    Default
}
public enum Attributes
{
    Agility,
    Intellect,
    Stamina,
    strength
}
[CreateAssetMenu(fileName = "New Item", menuName = "Inventory System/Items/item")]
public class ItemObject : ScriptableObject
{
    //public int Id;
    public Sprite uiDisplay;
    public bool stackable; // 합치기 가능한지 아닌지 여부 체크하는 부분
    //public GameObject dropItem;
    public ItemType type;
    [TextArea(15, 20)]
    public string description;
    //public ItemBuff[] buffs;
    public Item data = new Item();

    public Item CreateItem()
    {
        Debug.Log("CreateItem");
        Item newItem = new Item(this);
        return newItem;
    }
}
[System.Serializable]
public class Item
{
    public string Name;
    public int Id = -1;
    public ItemBuff[] buffs;
    public WeaponType weaponType;
    public Item()
    {
        Name = "";
        Id = -1;
    }
    public Item(ItemObject itemobject)
    {
        Name = itemobject.name;
        Id = itemobject.data.Id;
        buffs = new ItemBuff[itemobject.data.buffs.Length];
        weaponType = itemobject.data.weaponType;
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(itemobject.data.buffs[i].min, itemobject.data.buffs[i].max)
            {
                attribute = itemobject.data.buffs[i].attribute
            };
        }
    }
}

[System.Serializable]
public class ItemBuff
{
    public Attributes attribute;
    public int value;
    public int min;
    public int max;
    public ItemBuff(int _min, int _max)
    {
        min = _min;
        max = _max;
        GenerateValue();
    }
    public void GenerateValue()
    {
        value = UnityEngine.Random.Range(min, max);
    }
}
