using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum ItemType
{
    Food,
    Equipment,
    Default
}
public abstract class ItemObject : ScriptableObject
{
    public int Id;
    public Sprite uiDisplay;
    //public GameObject prefabs;
    public ItemType type;
    //public Image itemImage;
    [TextArea(15, 20)]
    public string description;
}
[System.Serializable]
public class Item
{
    public string Name;
    public int Id;
    public Item(ItemObject itemobject)
    {
        Name = itemobject.name;
        Id = itemobject.Id;
    }
}
