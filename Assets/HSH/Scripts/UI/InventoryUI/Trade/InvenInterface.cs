using Photon.Pun;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class InvenInterface : UserInterface
{
    public GameObject inventoryPrefab;
    public InventoryObject tradeInven;
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
            int myViewId = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID;
            int itemId = slotsOnInterface[obj].item.Id;
            int slotId = slotsOnInterface[obj].slotNumber;
            int itemamount = slotsOnInterface[obj].amount;
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);

            if (mouseHoverSlotData.parent.name == "MyTradeScreen")
            {
                object[] instantiationData = { itemId, myViewId, slotId, itemamount };
                photonView.RPC("RequestUpdateOpponent", RpcTarget.MasterClient, instantiationData);
            }
        }
    }

    [PunRPC]
    public void RequestUpdateOpponent(int itemId, int myViewId, int slot, int amount)
    {
        if (Manager.Inven.tradeUserID == myViewId && itemId >= 0) // 받은 ID가 거래 대상 맞는지 확인하고, 아이템 아이디 있는건지 확인하고
        {
            ItemObject _itemobject = opponentInven.database.Items[itemId]; // 아이템 아이디맞춰서 데이터베이스에서 정보 가져오고

            Item _item = _itemobject.data; // 그 아이템 정보 확인

            for (int i = 0; i < opponentInven.Container.Items.Length; i++)
            {
                if (opponentInven.Container.Items[i].slotNumber == slot) // 거래창의 슬롯번호 맞는거 찾아서
                {
                    opponentInven.Container.Items[i].UpdateSlot(_item, amount); // 그 슬롯 템정보 갱신
                    object[] instantiationData = { itemId, myViewId, slot, amount };
                    photonView.RPC("ResultUpdateOpponent", RpcTarget.AllBuffered, instantiationData);
                    return;
                }
            }
        }
        else
        {
            return;
        }
    }

    [PunRPC]
    public void ResultUpdateOpponent(int itemId, int myViewId, int slot, int amount)
    {
        if (Manager.Inven.tradeUserID == myViewId && itemId >= 0) // 받은 ID가 거래 대상 맞는지 확인하고, 아이템 아이디 있는건지 확인하고
        {
            ItemObject _itemobject = opponentInven.database.Items[itemId]; // 아이템 아이디맞춰서 데이터베이스에서 정보 가져오고

            Item _item = _itemobject.data; // 그 아이템 정보 확인

            for (int i = 0; i < opponentInven.Container.Items.Length; i++)
            {
                if (opponentInven.Container.Items[i].slotNumber == slot) // 거래창의 슬롯번호 맞는거 찾아서
                {
                    opponentInven.Container.Items[i].UpdateSlot(_item, amount); // 그 슬롯 템정보 갱신
                    return;
                }
            }
        }
        else
        {
            return;
        }
    }

    public void OnClickpointer(GameObject obj) // 인벤토리 슬롯 아이템 클릭 시 거래창에 넣기
    {
        tradePhotonHelper.MyViewID = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID;
        tradePhotonHelper.OpponentID = Manager.Inven.tradeUserID;
        for (int i = 0; i < tradeInven.Container.Items.Length; i++)
        {
            if (tradeInven.Container.Items[i].item.Id < 0) // 거래칸에 빈칸 확인해보고
            {
                InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver]; // 이건 필요없는데 그냥 냅둠
                int myViewId = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID; // 내view아이디
                int itemId = slotsOnInterface[obj].item.Id; // 내가 보낼 템 ID
                int slotNumber = i; // 내 슬롯 번호 인데 이건 거래창 빈슬롯 찾아가는 거니까 그냥 i로
                int itemAmount = slotsOnInterface[obj].amount; // 내가 보낼 템 갯수

                object[] instantiationData2 = { myViewId };
                tradePhotonHelper.photonView.RPC("OpponentCheckCancel", RpcTarget.All, myViewId);
                tradePhotonHelper.MyOkCancel();

                inventory.SwapItems(slotsOnInterface[obj], tradeInven.Container.Items[i]);
                //Manager.Inven.OpponentUserId = myViewId;
                //Manager.Inven.tradeUserID = myViewId;
                object[] instantiationData = { itemId, myViewId, slotNumber, itemAmount };
                photonView.RPC("RequestUpdateOpponent", RpcTarget.All, instantiationData);

            }
            else
            {
                continue;
            }
        }
        if (slotsOnInterface[obj].item.Id >= 0) // 다 끝났는데 템 남아있으면 스톱
        {
            return;
            //Manager.Inven.DropPositioning();
            ////inventory.GuestDropItem(slotsOnInterface[obj]);
            //Manager.Inven.HSHplayer.GuestDropItem(slotsOnInterface[obj]);
            //slotsOnInterface[obj].RemoveItem();
        }
    }
    public Vector3 GetPositon(int i) // 인벤토리 슬롯 위치 잡는 부분
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    }
}
