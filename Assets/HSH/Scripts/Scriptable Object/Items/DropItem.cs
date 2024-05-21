using ExitGames.Client.Photon;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class DropItem : MonoBehaviourPun, HSH_IInteractable<InventoryObject>
{
    public ItemObject itemObject;
    public ItemDatabaseObject database;
    
    //public void Interact(InventoryObject Inventory)
    //{
    //    Debug.Log("아이템에서 작동중");
    //    if (Inventory.AddItem(new Item(itemObject), 1))
    //    {
    //        PhotonNetwork.Destroy(gameObject); // DropItem GameObject 제거
    //    }
    //}
    public void Interact(InventoryObject inventory)
    {
        Debug.Log("아이템에서 작동중");
        if (inventory.AddItem(new Item(itemObject), 1))
        {
            photonView.RPC("DestroyRpc", RpcTarget.AllBuffered);
        }
    }

    [PunRPC]
    public IEnumerator DestroyRpc()
    {
        Destroy(this.gameObject);
        yield return 0; // if you allow 1 frame to pass, the object's OnDestroy() method gets called and cleans up references.
    }


    [PunRPC]
    public void SetItemObject(int id, string name)
    {
        Debug.Log("SetItemObject");
        this.itemObject = database.Items[id];
        this.gameObject.name = name;
    }
}
