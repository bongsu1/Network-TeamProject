using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Item/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;
    //public Dictionary<ItemObject, int> GetID = new Dictionary<ItemObject, int>();
    public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();

    public void OnAfterDeserialize() // 나열이 시작되기 전에 호출, 나열될 데이터를 호출 가능
    {
        //GetID = new Dictionary<ItemObject, int>();

        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].Id = i;
            GetItem.Add(i, Items[i]);
        }
    }
    //뭐야
    public void OnBeforeSerialize() //  나열 완료 후 호출, 나열된 데이터를 복구 가능
    {
        GetItem = new Dictionary<int, ItemObject>();
    }
    // 보던거 22분부터 이어서
}
