using UnityEngine;

public class DropItem : MonoBehaviour, HSH_IInteractable
{
    public ItemObject itemObject;
    public void Interact()
    {
        
    }
    public void Interact(InventoryObject Inventory)
    {
        Debug.Log("아이템에서 작동중");
        if (Inventory.AddItem(new Item(itemObject), 1))
        {
            Destroy(gameObject); // DropItem GameObject 제거
        }
    }
}
