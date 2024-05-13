using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticePlayer : MonoBehaviour
{
    public InventoryObject inventory;

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<DropItem>();
        if (item)
        {
            inventory.AddItem(new Item(item.itemObject), 1);
            Destroy(other.gameObject);
        }
    }
    private void Update()
    {
        if(Input.GetKeyDown(KeyCode.I))
        {
            inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            inventory.Load();
        }
    }
    private void OnApplicationQuit()
    {
        inventory.Container.Items = new InventorySlot[24];
    }
}
