using Photon.Pun;
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
        if (inventory.AddItem(new Item(itemObject), 1))
        {
            //PhotonNetwork.Destroy(gameObject);
            photonView.RPC("DestroyRpc", RpcTarget.MasterClient);
        }
    }
    // 드랍 아이템을 룸오브젝트로 바꾸자

    [PunRPC]
    public void DestroyRpc()
    {
        PhotonNetwork.Destroy(this.gameObject);
    }
    //public IEnumerator DestroyRpc()
    //{
    //    Destroy(this.gameObject);
    //    yield return 0; // if you allow 1 frame to pass, the object's OnDestroy() method gets called and cleans up references.
    //}

    [PunRPC]
    public void SetItemObject(int id, string name)
    {
        Debug.Log("SetItemObject");
        this.itemObject = database.Items[id];
        this.gameObject.name = name;
    }
}
