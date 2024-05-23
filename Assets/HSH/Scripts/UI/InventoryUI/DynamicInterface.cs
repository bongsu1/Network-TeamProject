using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class DynamicInterface : UserInterface
{
    public GameObject inventoryPrefab;
    public InventoryObject equipment;

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
    public void OnClickpointer(GameObject obj) // 인벤토리 슬롯 클릭 시 ID 같은 칸 찾아서 장비칸에 자동 장착
    {
        for( int i = 0; i < equipment.Container.Items.Length;i++)
        {
            for (int j = 0; j < equipment.Container.Items[i].AllowedItems.Length; j++)
            {
                if ( Manager.Inven.database.Items[slotsOnInterface[obj].item.Id].type != equipment.Container.Items[i].AllowedItems[j])
                {
                    continue;
                }
                else
                {
                    inventory.SwapItems(slotsOnInterface[obj], equipment.Container.Items[i]);
                    return;
                }
            }
            
        }
    }
    public Vector3 GetPositon(int i) // 인벤토리 슬롯 위치 잡는 부분
    {
        return new Vector3(X_START + (X_SPACE_BETWEEN_ITEM * (i % NUMBER_OF_COLUMN)), Y_START + (-Y_SPACE_BETWEEN_ITEMS * (i / NUMBER_OF_COLUMN)), 0f);
    }
}
