using Photon.Pun;
using UnityEngine;

public class DropItem : MonoBehaviourPun, HSH_IInteractable
{
    public ItemObject itemObject;
    public ItemDatabaseObject database;
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
    [PunRPC]
    public void SetItemObject(int id, string name)
    {
        Debug.Log("SetItemObject");
        this.itemObject = database.Items[id];
        this.gameObject.name = name;
    }
}
