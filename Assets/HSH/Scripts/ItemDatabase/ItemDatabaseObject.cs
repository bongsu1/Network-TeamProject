using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "New Item Database", menuName = "Inventory System/Items/Database")]
public class ItemDatabaseObject : ScriptableObject, ISerializationCallbackReceiver
{
    public ItemObject[] Items;
    [ContextMenu("Update ID's")]
    // 데이터베이스에 들어있는 아이템에 ID 부여
    public void UpdateID() 
    {
        for (int i = 0; i < Items.Length; i++)
        {
            if (Items[i].data.Id != i)
            {
                Items[i].data.Id = i;
            }
        }
    }
    public void OnBeforeSerialize() // 직렬화 전에 호출, 나열될 데이터를 호출 가능 
    {
        
    }
    public void OnAfterDeserialize() //  역직렬화 후 호출, 나열된 데이터를 복구 가능
    {
        UpdateID();
    }
}
