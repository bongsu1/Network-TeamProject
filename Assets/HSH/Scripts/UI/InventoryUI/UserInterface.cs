using Photon.Pun;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public abstract class UserInterface : MonoBehaviourPun
{
    //public MouseItem mouseItem = new MouseItem();

    //public PracticePlayer practicePlayer;

    //public GameObject inventoryPrefab;
    public InventoryObject inventory;

    //public int X_START;
    //public int Y_START;

    //public int X_SPACE_BETWEEN_ITEM;
    //public int NUMBER_OF_COLUMN;
    //public int Y_SPACE_BETWEEN_ITEMS;

    public Dictionary<GameObject, InventorySlot> slotsOnInterface = new Dictionary<GameObject, InventorySlot>();

    private void Start()
    {
        inventory.Container.UpdateNum();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this;
        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        //CreateDisplay();
        //
    }
    public void Update()
    {
        slotsOnInterface.UpdateSlotDisplay();
    }
    public abstract void CreateSlots();

    //생성된 슬롯에 이벤트 트리거를 추가하는 메서드
    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        //EventTrigger 공란 추가
        var eventTrigger = new EventTrigger.Entry();

        //이벤트의 종류를 설정 PointerClick, PointerEnter 등
        eventTrigger.eventID = type;

        //이벤트가 발생했을 때 호출될 UnityAction 추가
        eventTrigger.callback.AddListener(action);

        //EventTrigger 컴포넌트의 트리거 목록에 새로 생성된 EventTrigger.Entry를 추가
        trigger.triggers.Add(eventTrigger);
    }
    //여기부터
    public void OnEnter(GameObject obj)
    {
        MouseData.slotHoveredOver = obj;
    }

    public void OnExit(GameObject obj) // 마우스가 위치를 벗어나면 null 값으로 초기화 해주는 부분
    {
        MouseData.slotHoveredOver = null;

    }
    // 실수로 파괴되는 일 없도록
    public void OnEnterInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = obj.GetComponent<UserInterface>();
    }
    public void OnExitInterface(GameObject obj)
    {
        MouseData.interfaceMouseIsOver = null;
    }
    // 이 위 두개 수식 추가, 각 인벤토리에 event trigger 추가
    public void OnDragStart(GameObject obj)
    {
        MouseData.tempItemBeingDragged = CreateTempItem(obj);
    }
    public GameObject CreateTempItem(GameObject obj) // 
    {
        GameObject tempItem = null;

        if (slotsOnInterface[obj].item.Id >= 0)
        {
            tempItem = new GameObject();

            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();

            //inventory.database.GetItem[SlotsOnInterface[obj].item.Id].uiDisplay; // 여기서 드래그시작할때 아이템 일러가 같이 움직이게 하는 곳
            img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;
            img.raycastTarget = false;
        }
        return tempItem;

    }

    public void OnDragEnd(GameObject obj)
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
        }
    }
    public void OnDrag(GameObject obj)
    {
        if (MouseData.tempItemBeingDragged != null)
        {
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
}
public static class MouseData
{
    public static UserInterface interfaceMouseIsOver;
    public static GameObject tempItemBeingDragged;
    //public static InventorySlot item;
    //public static InventorySlot hoverItem;
    public static GameObject slotHoveredOver;
}

public static class ExtentionMethods // 이렇게 나눌 이유가 있나?
{
    // 하위 클래스에서 자동으로 스스로의 슬롯을 찾아서 업데이트하기 위해
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
        {
            if (_slot.Value.item.Id >= 0) //  슬롯의 ID 가 0보다 크면(아이템이 있으면)
            {
                //Debug.Log($"0.{_slot.Key.transform}");
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.uiDisplay;//inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
            }
            else
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
                _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
            }
        }
    }
}