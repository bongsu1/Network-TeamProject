using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StaticInterface : UserInterface
{
    public GameObject[] slots;
    public InventoryObject equipment;
    public override void CreateSlots()
    {
        Debug.Log("static createslots");
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < equipment.Container.Items.Length; i++)
        {
            var obj = slots[i];

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            AddEvent(obj, EventTriggerType.PointerClick, delegate { OnClickpointer(obj); });

            slotsOnInterface.Add(obj, equipment.Container.Items[i]);
        }
    }
    public void OnClickpointer(GameObject obj) // 인벤토리 슬롯 클릭 시 ID 같은 칸 찾아서 장비칸에 자동 장착
    {
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            if(inventory.Container.Items[i] ==null)
            {
                inventory.SwapItems(slotsOnInterface[obj], equipment.Container.Items[i]);
            }
        }
    }
}
