using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class MyTradeInterface : UserInterface
{
    public GameObject inventoryPrefab;
    public InventoryObject invenOrigin;
    public InventoryObject opponentInven;

    public TradePhotonHelper tradePhotonHelper;

    public int X_START;
    public int Y_START;

    public int X_SPACE_BETWEEN_ITEM;
    public int NUMBER_OF_COLUMN;
    public int Y_SPACE_BETWEEN_ITEMS;

    public override void CreateSlots()
    {
        Debug.Log("dynamic createslots");
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
            obj.GetComponent<RectTransform>().localPosition = GetPositon(i);

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            AddEvent(obj, EventTriggerType.PointerClick, delegate { OnClickpointer(obj); });

            slotsOnInterface.Add(obj, inventory.Container.Items[i]);
        }
    }
    public new void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        if (MouseData.interfaceMouseIsOver == null)
        {
            if (PhotonNetwork.IsMasterClient == false)
            {
                Manager.Inven.DropPositioning();
                //inventory.GuestDropItem(slotsOnInterface[obj]);
                Manager.Inven.playerController.GuestDropItem(slotsOnInterface[obj]);
                slotsOnInterface[obj].RemoveItem();
            }
            else
            {
                Manager.Inven.DropPositioning();
                inventory.MasterDropItem(slotsOnInterface[obj]);
                slotsOnInterface[obj].RemoveItem();
            }

        }
        if (MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];

            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);

            int myViewId = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID;
            int itemId = slotsOnInterface[obj].item.Id;
            int slotNumber = slotsOnInterface[obj].slotNumber;
            int itemAmount = slotsOnInterface[obj].amount;
            object[] instantiationData = { itemId, myViewId, slotNumber, itemAmount };

            Manager.Inven.OpponentUserId = myViewId;
            Manager.Inven.tradeUserID = myViewId;

            photonView.RPC("RequestTrade", RpcTarget.MasterClient, instantiationData);

        }
    }
    public void OnClickpointer(GameObject obj) // 거래창 슬롯 클릭 시 빈 칸 찾아서 복귀하고 상대 거래창 업뎃
    {
        for (int i = 0; i < invenOrigin.Container.Items.Length; i++)
        {
            if (invenOrigin.Container.Items[i].item.Id < 0)
            {
                inventory.SwapItems(slotsOnInterface[obj], invenOrigin.Container.Items[i]);

                int myViewId = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID;
                int itemId = slotsOnInterface[obj].item.Id;
                int slotNumber = slotsOnInterface[obj].slotNumber; // 여긴 상대거래창 슬롯번호보고 동기화니까 이게 맞음
                int itemAmount = slotsOnInterface[obj].amount;
                object[] instantiationData = { itemId, myViewId, slotNumber, itemAmount };

                //Manager.Inven.OpponentUserId = myViewId;
                tradePhotonHelper.MyViewID = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID;
                tradePhotonHelper.OpponentID = Manager.Inven.tradeUserID;
                tradePhotonHelper.photonView.RPC("OpponentCheckCancel", RpcTarget.MasterClient, myViewId);
                tradePhotonHelper.MyOkCancel();

                photonView.RPC("RequestTrade", RpcTarget.MasterClient, instantiationData);
            }
            else
            {
                continue;
            }
        }
        if (slotsOnInterface[obj].item.Id >= 0) // 다 끝났는데 템 남아있으면 드롭
        {
            Manager.Inven.DropPositioning();
            //inventory.GuestDropItem(slotsOnInterface[obj]);
            Manager.Inven.playerController.GuestDropItem(slotsOnInterface[obj]);
            slotsOnInterface[obj].RemoveItem();
        }
    }
    [PunRPC]
    public void RequestTrade(int _itemId, int _myViewId, int _slot, int _amount)
    {
        if (Manager.Inven.tradeUserID == _myViewId && _itemId >= 0)
        {
            ItemObject _itemobject = opponentInven.database.Items[_itemId];
            
            Item _item = _itemobject.data;
            
            for (int i = 0; i < opponentInven.Container.Items.Length; i++)
            {
                if (opponentInven.Container.Items[i].slotNumber == _slot)
                {
                    opponentInven.Container.Items[i].UpdateSlot(_item, _amount);
                    return;
                }
            }
        }
        else if (Manager.Inven.tradeUserID == _myViewId && _itemId < 0)
        {
            Item _item = new Item();
            for (int i = 0; i < opponentInven.Container.Items.Length; i++)
            {
                if (opponentInven.Container.Items[i].slotNumber == _slot)
                {
                    opponentInven.Container.Items[i].UpdateSlot(_item, _amount);
                    return;
                }
            }
        }
        else
        {
            return;
        }
    }
    public Vector3 GetPositon(int i) // 인벤토리 슬롯 위치 잡는 부분
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    }

    private void OnDisable()
    {
        inventory.Clear();
    }
    [PunRPC]
    private void ReturnItem() // 이건 나중에
    {
        for (int i = 0; i <= inventory.Container.Items.Length; ++i)
        {
            for (int j = 0; j < invenOrigin.Container.Items.Length; j++)
            {
                if (inventory.Container.Items[i].item.Id < 0) // 내 거래창 슬롯에 템 없으면 다음으로
                {
                    continue;
                }
                else if (inventory.Container.Items[i].item.Id >= 0 && invenOrigin.Container.Items[j].item.Id < 0)// 거래창 슬롯에 템이 있고, 인벤토리 슬롯에 템이 없으면 되돌린다
                {
                    inventory.SwapItems(inventory.Container.Items[i], invenOrigin.Container.Items[j]);
                }
                else
                {

                }
            }
        }
    }
}
