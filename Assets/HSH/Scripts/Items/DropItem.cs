using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DropItem : MonoBehaviour
{
    public ItemObject itemObject;
    public SpriteRenderer image;

    public void SetItem(ItemObject _itemObject)
    {
        itemObject.itemName = _itemObject.itemName;
        itemObject.itemImage = _itemObject.itemImage;
        itemObject.type = _itemObject.type;
        //itemobject.efts = _itemObject.efts;

        //image.sprite = item.itemImage;
    }
    public ItemObject GetItem()
    {
        return itemObject;
    }
    public void DestroyItem()
    {
        Destroy(gameObject);
    }
}
