using UnityEngine;

public class PracticePlayer : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject EquipInventory;

    public MouseItem mouseItem = new MouseItem(); // 이게 여기 옮겨지면 편해지는건?
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
        if (Input.GetKeyDown(KeyCode.I))
        {
            inventory.Save();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            inventory.Load();
        }
    }
    private void OnDisable()
    {
        Debug.Log("set slot");
        inventory.Container.Items = new InventorySlot[24];
    }
}
