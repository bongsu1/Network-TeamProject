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
    public string itemName;
    public GameObject prefabs;
    public ItemType type;
    public Sprite itemImage;
    [TextArea(15, 20)]
    public string description;
}
