using System.IO;
using System.Text;
using System;
using UnityEngine;

public class PracticePlayer : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;
    
    //public MouseItem mouseItem = new MouseItem(); // 이게 여기 옮겨지면 편해지는건?
    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<DropItem>();
        if (item)
        {
            Item _item = new Item(item.itemObject);

            if (inventory.AddItem(new Item(item.itemObject), 1))
            {
                Destroy(other.gameObject);
            }
            //inventory.AddItem(new Item(item.itemObject), 1);
        }
    }
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Save");
            //inventory.Save();
            //equipment.Save();
            Manager.Inven.SaveToJson();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Load");
            //inventory.Load();
            //equipment.Load();
            Manager.Inven.LoadFromJson();
        }
    }
    private void OnDisable()
    {
        Debug.Log("Clear slot");
        inventory.Container.Clear();
        equipment.Container.Clear();
    }
}