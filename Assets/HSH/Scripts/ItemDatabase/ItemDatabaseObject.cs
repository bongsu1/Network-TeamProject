using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;
    //public Dictionary<ItemObject, int> GetID = new Dictionary<ItemObject, int>();
    //public Dictionary<int, ItemObject> GetItem = new Dictionary<int, ItemObject>();
    [ContextMenu("Update ID's")]
    public void UpdateID() // 데이터베이스에 들어있는 아이템에 ID 부여
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].data.Id != i)
            {
                Items[i].data.Id = i;
            }
        }
    }
    public void OnAfterDeserialize() // 나열이 시작되기 전에 호출, 나열될 데이터를 호출 가능
    {
        UpdateID();
        //GetID = new Dictionary<ItemObject, int>();

        //for (int i = 0; i < Items.Length; i++)
        //{
        //    Items[i].data.Id = i;
        //    //GetItem.Add(i, Items[i]);
        //}
    }
    public void OnBeforeSerialize() //  나열 완료 후 호출, 나열된 데이터를 복구 가능
    {
        //GetItem = new Dictionary<int, ItemObject>();
    }
}
