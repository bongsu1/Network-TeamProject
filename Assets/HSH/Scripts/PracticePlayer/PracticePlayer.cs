using UnityEngine;

public class PracticePlayer : MonoBehaviour
{
    public InventoryObject inventory;
    public InventoryObject equipment;


    private void Start()
    {
        //Manager.Inven.player = this;
    }
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
        //if (Input.GetKeyDown(KeyCode.I))
        //{
        //    Debug.Log("Save");
        //    Manager.Inven.SaveToJson();
        //}
        //if (Input.GetKeyDown(KeyCode.U))
        //{
        //    Debug.Log("Load");
        //    Manager.Inven.LoadFromJson();
        //}
    }
    private void OnDisable()
    {
        Debug.Log("Clear slot");
        inventory.Container.Clear();
        equipment.Container.Clear();
    }
}