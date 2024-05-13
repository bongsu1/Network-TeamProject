using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using Unity.VisualScripting;
using UnityEngine.Experimental.AI;
public abstract class UserInterface : MonoBehaviour
{
    //public MouseItem mouseItem = new MouseItem();

    public PracticePlayer practicePlayer;

    public GameObject inventoryPrefab;
    public InventoryObject inventory;

    //public int X_START;
    //public int Y_START;

    //public int X_SPACE_BETWEEN_ITEM;
    //public int NUMBER_OF_COLUMN;
    //public int Y_SPACE_BETWEEN_ITEMS;

    public Dictionary<GameObject, InventorySlot> itemsDisplayed = new Dictionary<GameObject, InventorySlot>();

    private void Start()
    {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            Debug.Log(inventory);
            Debug.Log(inventory.Container);
            Debug.Log(inventory.Container.Items[i]);
            
            inventory.Container.Items[i].parent = this;
        }
        CreateSlots();
        //CreateDisplay();
    }
    private void Update()
    {
        UpdateSlots();
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
    public void UpdateSlots()
    {
        foreach (KeyValuePair<GameObject, InventorySlot> _slot in itemsDisplayed)
        {
            Debug.Log($"1. {_slot}");
            Debug.Log($"2. {_slot.Value}");
            Debug.Log($"3. {_slot.Value.ID}");
            Debug.Log($"위 버그는 유니티 엔진 내에서 플레이어 인벤토리 인스펙터창을 고정하고 재실행하면 해결됨");
            if (_slot.Value.ID >= 0)
            {
                _slot.Key.transform.GetChild(0).GetComponentInChildren<Image>().sprite = inventory.database.GetItem[_slot.Value.item.Id].uiDisplay;
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
        practicePlayer.mouseItem.hoverobj = obj;
        if (itemsDisplayed.ContainsKey(obj))
        {
            practicePlayer.mouseItem.hoverItem = itemsDisplayed[obj];
        }
    }

    public void OnExit(GameObject obj)
    {
        practicePlayer.mouseItem.hoverobj = null;
        practicePlayer.mouseItem.hoverItem = null;

    }
    public void OnDragStart(GameObject obj)
    {
        var mouseObject = new GameObject();
        var rt = mouseObject.AddComponent<RectTransform>();
        rt.sizeDelta = new Vector2(50, 50);
        mouseObject.transform.SetParent(transform.parent);
        if (itemsDisplayed[obj].ID >= 0)
        {
            var img = mouseObject.AddComponent<Image>();
            img.sprite = inventory.database.GetItem[itemsDisplayed[obj].ID].uiDisplay; // 여기서 드래그시작할때 아이템 일러가 같이 움직이게 하는데
            img.raycastTarget = false;
        }
        practicePlayer.mouseItem.obj = mouseObject;
        practicePlayer.mouseItem.item = itemsDisplayed[obj];

    }
    public void OnDragEnd(GameObject obj)
    {
        if (practicePlayer.mouseItem.hoverobj)
        {
            inventory.MoveItem(itemsDisplayed[obj], practicePlayer.mouseItem.hoverItem.parent.itemsDisplayed[practicePlayer.mouseItem.hoverobj]); // 아이템 슬롯 이동
        }
        else
        {
            inventory.RemoveItem(itemsDisplayed[obj].item); // 아이템 드래그해서 드롭시 파괴.
        }
        Destroy(practicePlayer.mouseItem.obj);
        practicePlayer.mouseItem.item = null;
    }
    public void OnDrag(GameObject obj)
    {
        if (practicePlayer.mouseItem.obj != null)
        {
            practicePlayer.mouseItem.obj.GetComponent<RectTransform>().position = Input.mousePosition;
        }
    }
    // 여기까지 아이템 드래그앤 드랍
    //public Vector3 GetPositon(int i) // 인벤토리 슬롯 위치 잡는 부분
    //{
    //    return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    //}

}
public class MouseItem
{
    public GameObject obj;
    public InventorySlot item;
    public InventorySlot hoverItem;
    public GameObject hoverobj;
}