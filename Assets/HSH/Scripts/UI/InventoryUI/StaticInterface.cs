using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class StaticInterface : UserInterface
{
    public GameObject[] slots;
    public InventoryObject invenOrigin;
    public override void CreateSlots()
    {
        Debug.Log("static createslots");
        slotsOnInterface = new Dictionary<GameObject, InventorySlot>();
        for (int i = 0; i < inventory.Container.Items.Length; i++)
        {
            var obj = slots[i];

            AddEvent(obj, EventTriggerType.PointerEnter, delegate { OnEnter(obj); });
            AddEvent(obj, EventTriggerType.PointerExit, delegate { OnExit(obj); });
            AddEvent(obj, EventTriggerType.BeginDrag, delegate { OnDragStart(obj); });
            AddEvent(obj, EventTriggerType.EndDrag, delegate { OnDragEnd(obj); });
            AddEvent(obj, EventTriggerType.Drag, delegate { OnDrag(obj); });
            AddEvent(obj, EventTriggerType.PointerClick, delegate { OnClickpointer(obj); });

            slotsOnInterface.Add(obj, inventory.Container.Items[i]);
        }
    }
    public void OnClickpointer(GameObject obj) // 장비창 슬롯 클릭 시 빈칸 찾아서 착용 해제
    {
        for (int i = 0; i < invenOrigin.Container.Items.Length; i++)
        {
            if (invenOrigin.Container.Items[i].item.Id < 0)
            {
                inventory.SwapItems(slotsOnInterface[obj], invenOrigin.Container.Items[i]);
            }
        }
    }
}
