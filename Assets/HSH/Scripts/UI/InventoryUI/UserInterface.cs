using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;
public abstract class UserInterface : MonoBehaviour
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
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            inventory.Container.Items[i].parent = this;
        }
        CreateSlots();
        AddEvent(gameObject, EventTriggerType.PointerEnter, delegate { OnEnterInterface(gameObject); });
        AddEvent(gameObject, EventTriggerType.PointerExit, delegate { OnExitInterface(gameObject); });
        //CreateDisplay();
    }
    public void Update()
    {
        slotsOnInterface.UpdateSlotDisplay();
        //UpdateSlots();
        //UpdateDisplay();
    }
    public abstract void CreateSlots();

    //public void CreateDisplay()
    //{

    //for (int i = 0; i < inventory.Container.Items.Count; i++)
    //{
    //    InventorySlot slot = inventory.Container.Items[i];

    //    var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
    //    obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
    //    obj.GetComponent<RectTransform>().localPosition = GetPositon(i);
    //    obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");

    //    itemsDisplayed.Add(slot, obj);
    //}
    //}
    //public void UpdateSlots() // 슬롯 이미지를 여기서 바꿈
    //{
    //    foreach (KeyValuePair<GameObject, InventorySlot> _slot in slotsOnInterface)
    //    {
    //        Debug.Log($"1. {_slot}");
    //        Debug.Log($"2. {_slot.Value}");
    //        Debug.Log($"3. {_slot.Value.item.Id}");
    //        Debug.Log($"위 버그는 유니티 엔진 내에서 플레이어 인벤토리 인스펙터창을 고정하고 재실행하면 해결됨");
    //        if (_slot.Value.item.Id >= 0)
    //        {
    //            _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = _slot.Value.ItemObject.uiDisplay;//inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
    //            _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 1);
    //            _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = _slot.Value.amount == 1 ? "" : _slot.Value.amount.ToString("n0");
    //        }
    //        else
    //        {
    //            _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = null;
    //            _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().color = new Color(1, 1, 1, 0);
    //            _slot.Key.GetComponentInChildren<TextMeshProUGUI>().text = "";
    //        }
    //    }
    //}

    //public void UpdateDisplay()
    //{
    //    for (int i = 0; i < inventory.Container.Items.Count; i++)
    //    {
    //        InventorySlot slot = inventory.Container.Items[i];

    //        if (itemsDisplayed.ContainsKey(slot))
    //        {
    //            itemsDisplayed[slot].GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");
    //        }
    //        else
    //        {
    //            var obj = Instantiate(inventoryPrefab, Vector3.zero, Quaternion.identity, transform);
    //            obj.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[slot.item.Id].uiDisplay;
    //            obj.GetComponent<RectTransform>().localPosition = GetPositon(i);
    //            obj.GetComponentInChildren<TextMeshProUGUI>().text = slot.amount.ToString("n0");

    //            itemsDisplayed.Add(slot, obj);
    //        }
    //    }
    //}
    protected void AddEvent(GameObject obj, EventTriggerType type, UnityAction<BaseEventData> action)
    {
        EventTrigger trigger = obj.GetComponent<EventTrigger>();
        var eventTrigger = new EventTrigger.Entry();
        eventTrigger.eventID = type;
        eventTrigger.callback.AddListener(action);
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

        if (slotsOnInterface[obj].item.Id>= 0)
        {
            tempItem = new GameObject();

            var rt = tempItem.AddComponent<RectTransform>();
            rt.sizeDelta = new Vector2(50, 50);
            tempItem.transform.SetParent(transform.parent);
            var img = tempItem.AddComponent<Image>();


            img.sprite = slotsOnInterface[obj].ItemObject.uiDisplay;//inventory.database.GetItem[SlotsOnInterface[obj].item.Id].uiDisplay; // 여기서 드래그시작할때 아이템 일러가 같이 움직이게 하는데
            img.raycastTarget = false;
        }
        return tempItem;
        
    }

    public void OnDragEnd(GameObject obj)
    {
        Destroy(MouseData.tempItemBeingDragged);
        if(MouseData.interfaceMouseIsOver == null)
        {
            slotsOnInterface[obj].RemoveItem();
            //slotsOnInterface[obj].DropItem();
            return;
        }
        if(MouseData.slotHoveredOver)
        {
            InventorySlot mouseHoverSlotData = MouseData.interfaceMouseIsOver.slotsOnInterface[MouseData.slotHoveredOver];
            //inventory.SwapItems
            inventory.SwapItems(slotsOnInterface[obj], mouseHoverSlotData);
        }
        
    }
    public void OnDrag(GameObject obj)
    {
        Debug.Log("Ondrag 는 작동하나");
        if (MouseData.tempItemBeingDragged != null)
        {
            MouseData.tempItemBeingDragged.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
    // 여기까지 아이템 드래그앤 드랍
    //public Vector3 GetPositon(int i) // 인벤토리 슬롯 위치 잡는 부분
    //{
    //    return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    //}

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
    // 하위 클래스에서 자동으로 스스로의 슬롯을 찾아서 업데이트하기 위해 this.
    public static void UpdateSlotDisplay(this Dictionary<GameObject, InventorySlot> _slotsOnInterface)
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in _slotsOnInterface)
        {
            Debug.Log($"00. {_slotsOnInterface}");
            Debug.Log($"01. {_slot}");
            Debug.Log($"02. {_slot.Value.item.Id}"); // 보통 땐 잘 된다. json 으로 로드 했을 때만 기능이 반응을 안한다....돌아가고는 있는데 스크립터블 오브젝트에서 읽지를 않는다.
            Debug.Log($"03. {_slot.Value.item.Name}"); // 이게 널값인것까지 확인
            Debug.Log($"위 버그는 유니티 엔진 내에서 플레이어 인벤토리 인스펙터창을 고정하고 재실행하면 해결됨, updateSlotDisplay");
            // 참조는 깨진거 아님. 
            // 스크립터블 오브젝트(인벤토리)는 정상적으로 변경됨
            // 스크립터블 오브젝트도 변경되고 참조가 끊어진것도 아니고 업데이트도 무사히 돌아가는데 왜 안될까
            if (_slot.Value.item.Id >= 0) //  슬롯의 ID 가 0보다 크면(아이템이 있으면)
            {
                Debug.Log($"0.{_slot.Key.transform}");
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