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
        
        for (int i = 0; i < Container.Items.Count; i++)
        {
            if (Container.Items[i].item/*.Id*/ == _item/*.Id*/) // 요 아이디를 풀면 아이템 1종류당 한칸으로 합쳐지고 이대로 두면 합쳐지지 않고 분리됨. 화살이랑 총알에만 합쳐지게 적용할 수 없나
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        
        Container.Items.Add(new InventorySlot(_item.Id, _item,_amount));
        
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
    public List<InventorySlot> Items = new List<InventorySlot>();
}

[System.Serializable]
public class InventorySlot
{
    public int ID;
    public Item item; // 아이템
    public int amount; // 아이템 갯수
    public InventorySlot(int _id, Item _item, int _amount)
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
