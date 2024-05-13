using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEditor;
using JetBrains.Annotations;
using System.Runtime.Serialization;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject/*, ISerializationCallbackReceiver*/
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container;


//    private void OnEnable()
//    {
//#if UNITY_EDITOR
//        database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/HSH/Scriptable Objects/Items/Data base/Database.asset", typeof(ItemDatabaseObject));
//#else 
//        database = Resources.Load<ItemDatabaseObject>("Database");
//#endif
//    }
    public void AddItem(Item _item, int _amount)
    {
        Debug.Log(_item.Id + "add item");
        if ( _item.buffs.Length > 0)
        {
            SetEmptySlot(_item, _amount );
            return;
        }
        //for (int i = 0; i < Container.Items.Length; i++)
        //{
        //    Debug.Log(Container.Items[i].item.Id + "checking item");
        //    if (Container.Items[i].ID/*item/*.Id*/ == _item.Id) // 요 아이디를 풀면 아이템 1종류당 한칸으로 합쳐지고 이대로 두면 합쳐지지 않고 분리됨. 화살이랑 총알에만 합쳐지게 적용할 수 없나
        //    {
        //        Debug.Log(_item.Id + "add amount");
        //        Container.Items[i].AddAmount(_amount);
        //        return;
        //    }
        //}
        SetEmptySlot(_item, _amount);



        //if ( _item.buffs.Length > 0)
        //{
        //    Container.Items.Add(new InventorySlot(_item.Id, _item, _amount));
        //    return;
        //}
        //for (int i = 0; i < Container.Items.Count; i++)
        //{
        //    if (Container.Items[i].item/*.Id*/ == _item/*.Id*/) // 요 아이디를 풀면 아이템 1종류당 한칸으로 합쳐지고 이대로 두면 합쳐지지 않고 분리됨. 화살이랑 총알에만 합쳐지게 적용할 수 없나
        //    {
        //        Container.Items[i].AddAmount(_amount);
        //        return;
        //    }
        //}   
        //Container.Items.Add(new InventorySlot(_item.Id, _item,_amount));
    }
    public InventorySlot SetEmptySlot(Item _item, int _amount) // 처음에 빈 슬롯 세팅
    {
        for (int i = 0;i < Container.Items.Length;i++)
        {
            //Debug.Log("set");
            //Debug.Log($"1.{Container}");
            //Debug.Log($"2.{Container.Items.Length}");
            //Debug.Log($"3.{Container.Items[0]}");
            if (Container.Items[i].ID <= -1)
            {
                Container.Items[i].UpdateSlot(_item.Id, _item, _amount);
                return Container.Items[i];
            }
        }
        // 인벤이 가득 찼을 떄의 스크립트 필요?
        return null;
    }

    public void MoveItem(InventorySlot item1, InventorySlot item2) // 아이템 두개 위치 교환
    {
        InventorySlot temp = new InventorySlot(item2.ID, item2.item, item2.amount);
        item2.UpdateSlot(item1.ID, item1.item, item1.amount);
        item1.UpdateSlot(temp.ID,temp.item, temp.amount);
    }


    [ContextMenu("Save")]
    public void Save()
    {
        Debug.Log("inven save");
        //string saveData = JsonUtility.ToJson(this, true);
        //BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(string.Concat(Application.persistentDataPath, savePath));
        //bf.Serialize(file, saveData);
        //file.Close();

        IFormatter formatter = new BinaryFormatter();
        Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Create, FileAccess.Write);
        formatter.Serialize(stream, Container);
        stream.Close();
    }
    [ContextMenu("Load")]
    public void Load()
    {
        if(File.Exists(string.Concat(Application.persistentDataPath, savePath)))
        {
            Debug.Log("inven Load");
            //BinaryFormatter bf = new BinaryFormatter();
            //FileStream file = File.Open(string.Concat(Application.persistentDataPath, savePath), FileMode.Open);
            //JsonUtility.FromJsonOverwrite(bf.Deserialize(file).ToString(), this);
            //file.Close();

            IFormatter formatter = new BinaryFormatter();
            Stream stream = new FileStream(string.Concat(Application.persistentDataPath, savePath), FileMode.Open, FileAccess.Read);
            Container = (Inventory)formatter.Deserialize(stream);
            stream.Close();
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container = new Inventory();
    }
    //public void OnAfterDeserialize()
    //{
    //    for (int i = 0; i < Container.Items.Count;i++)
    //    {
    //        Container.Items[i].item = database.GetItem[Container.Items[i].ID];
    //    }
    //}

    //public void OnBeforeSerialize()
    //{
        
    //}
}
[System.Serializable]
public class Inventory
{
    public InventorySlot[] Items = new InventorySlot[24];
}

[System.Serializable]
public class InventorySlot
{
    public int ID = -1;
    public Item item; // 아이템
    public int amount; // 아이템 갯수

    public InventorySlot()
    {
        ID = -1;
        item = null;
        amount = 0;
    }
    public InventorySlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(int _id, Item _item, int _amount)
    {
        ID = _id;
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value)
    {
        amount += value;
    }
}
