using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PracticePlayer : MonoBehaviour
{
    public InventoryObject inventory;

    public void OnTriggerEnter(Collider other)
    {
        var item = other.GetComponent<Item>();
        if (item)
        {
            inventory.AddItem(item.item, 1);
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
        inventory.Container.Clear();
    }
}
