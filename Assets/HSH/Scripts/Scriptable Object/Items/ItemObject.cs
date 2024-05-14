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
public abstract class ItemObject : ScriptableObject
{
    //public int Id;
    public Sprite uiDisplay;
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
    public Item()
    {
        Name = "";
        Id = -1;
    }
    public Item(ItemObject itemobject)
    {
        Name = itemobject.name;
        Id = itemobject.Id;
        buffs = new ItemBuff[itemobject.buffs.Length];
        for (int i = 0; i < buffs.Length; i++)
        {
            buffs[i] = new ItemBuff(itemobject.buffs[i].min, itemobject.buffs[i].max)
            {
                attribute = itemobject.buffs[i].attribute
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
