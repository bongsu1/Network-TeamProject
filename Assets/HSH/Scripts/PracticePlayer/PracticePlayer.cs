using UnityEngine;

public class PracticePlayer : MonoBehaviour
{
    public InventoryObject inventory;

    //public MouseItem mouseItem = new MouseItem(); // 이게 여기 옮겨지면 편해지는건?
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
            Debug.Log("Save");
            inventory.SaveToJson();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Load");
            inventory.LoadFromJson();
        }
    }
    private void OnDisable()
    {
        Debug.Log("set slot");
        inventory.Container.Items = new InventorySlot[24];
    }
}
