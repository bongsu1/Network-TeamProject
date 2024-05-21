using Photon.Pun;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory", menuName = "Inventory System/Inventory")]
public class InventoryObject : ScriptableObject/*, IPunObservable*/
{
    public string savePath;
    public ItemDatabaseObject database;
    public Inventory Container;
    public DropItem dropItemPrefab;

    /*  private void OnEnable()
      {
  #if UNITY_EDITOR
          database = (ItemDatabaseObject)AssetDatabase.LoadAssetAtPath("Assets/HSH/Scriptable Objects/Items/Data base/Database.asset", typeof(ItemDatabaseObject));
  #else 
          database = Resources.Load<ItemDatabaseObject>("Database");
  #endif
      }*/

    public bool AddItem(Item _item, int _amount)
    {
        if (EmptySlotCount <= 0)
        {
            return false;
        }
        InventorySlot slot = FindItemOnInventory(_item);
        if (!database.Items[_item.Id].stackable || slot == null) // 합치기 가능한지 여부 체크되있는지 확인하고
        {
            SetEmptySlot(_item, _amount);
            return true;
        }
        slot.AddAmount(_amount);
        return true;



        /*if (_item.buffs.Length > 0)
        {
            SetEmptySlot(_item, _amount);
            return;
        }
        for (int i = 0; i < Container.Items.Length; i++) // 같은 아이템끼리 합치는 부분
        {
            if (Container.Items[i].item.Id == _item.Id) // 요 아이디를 풀면 아이템 1종류당 한칸으로 합쳐지고 이대로 두면 합쳐지지 않고 분리됨. 화살이랑 총알에만 합쳐지게 적용할 수 없나
            {
                Container.Items[i].AddAmount(_amount);
                return;
            }
        }
        SetEmptySlot(_item, _amount);*/

    }
    public void Myposition()
    {
        
    }
    public InventorySlot FindItemOnInventory(Item _item)
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item.Id == _item.Id)// 슬롯에 같은 아이템 있는지 확인하고 있으면 그 슬롯 반환, 없으면 null값 반환
            {
                return Container.Items[i];
            }
        }
        return null;
    }

    public int EmptySlotCount // 빈칸 카운트
    {
        get
        {
            int counter = 0;
            for (int i = 0; i < Container.Items.Length; i++)
            {
                if (Container.Items[i].item.Id <= -1)
                {
                    counter++;
                }
            }
            return counter;
        }
    }

    // 처음에 빈 슬롯 세팅
    public InventorySlot SetEmptySlot(Item _item, int _amount) // 빈슬롯을 상황에 따라 세팅.
    {
        for (int i = 0; i < Container.Items.Length; i++)
        {
            //Debug.Log("set");
            //Debug.Log($"1.{Container}");
            //Debug.Log($"2.{Container.Items.Length}");
            //Debug.Log($"3.{Container.Items[0]}");
            if (Container.Items[i].item.Id <= -1) // 특정 슬롯의 ID가 -1 아래라는건 비어있다는 것
            {
                Container.Items[i].UpdateSlot(_item, _amount); // 빈 슬롯에 아이템을 넣는다.
                return Container.Items[i]; // 그리고 그 슬롯값 반환
            }
        }
        // 인벤이 가득 찼을 떄의 스크립트 필요?
        return null; // 꽉찼으면 null 반환
    }
    // 아이템 두개 위치 교환
    public void SwapItems(InventorySlot item1, InventorySlot item2) // 아이템 스왑하는 함수
    {
        Debug.Log("SwapItems");
        if (item2.CanPlaceInSlot(item1.ItemObject) && item1.CanPlaceInSlot(item2.ItemObject))  //
        {
            InventorySlot temp = new InventorySlot(item2.item, item2.amount);
            item2.UpdateSlot(item1.item, item1.amount);
            item1.UpdateSlot(temp.item, temp.amount);
        }
    }
    public void DropItem(InventorySlot item) // 아이텝 드랍하는 함수
    {
        if (PhotonNetwork.InRoom)
        {
            Debug.Log("dropItem");
            
            // 룸 오브젝트 프리팹 인스턴스화
            GameObject roomObject = PhotonNetwork.Instantiate("dropItemPrefab", Manager.Inven.dropPosition, Quaternion.identity);

            // 룸 오브젝트 내 DropItem 컴포넌트에 액세스해서 변경
            //DropItem 에 MonoBehaviourPun 달면 바로 답나오는 문제를 이래 헤매면 어떡하니 나야
            roomObject.GetComponent<DropItem>().photonView.RPC("SetItemObject", RpcTarget.All, item.item.Id, database.Items[item.item.Id].name);
        }
        else
        {
            return;
        }
            
        /*// 싱글판
        for (int i = 0; i < Container.Items.Length; i++)
        {
            Debug.Log($"02. {Container.Items[i].item.Id}");
            if (Container.Items[i].item.Id != item.item.Id)
            {
                continue;
            }
            else
            {
                Debug.Log($"01. {database.Items[item.item.Id]}");
                DropItem dropItem = PhotonNetwork.Instantiate("dropItemPrefab", Manager.Inven.dropPosition, Quaternion.identity).GetComponent<DropItem>(); //  생성하면서 바로 컴포넌트 할당
                dropItem.itemObject = database.Items[item.item.Id]; // 내부 정보 변경
                dropItem.gameObject.name = database.Items[item.item.Id].name; // 이름 넣어주는곳
                break;
              
            }
        }*/
    }
    


    // 아이템 제거
    public void RemoveItem(Item _item)
    {
        Debug.Log("RemoveItem");
        for (int i = 0; i < Container.Items.Length; i++)
        {
            if (Container.Items[i].item == _item)
            {
                Container.Items[i].UpdateSlot(null, 0);
            }
        }
    }
    [ContextMenu("Clear")]
    public void Clear()
    {
        Container.Clear();
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // photonView.IsMine 일때
        {
            
        }
        else // photonView.IsReading || photonView.InMine == false 일때
        {
            
        }
    }
    //public void OnAfterDeserialize()
    //{
    //    for (int i = 0; i < Container.Items.Count;i++)
    //    {
    //        Container.Items[i].item = database.GetItem[Container.Items[i].ID];
    //    }
    //}

    //public void OnBeforeSerialize()
    //{

    //}
}
[System.Serializable]
public class InvenData
{
    public Inventory invenSave;
    public Inventory equipSave;

    public InvenData(Inventory invenSave, Inventory equipSave)
    {
        this.invenSave = invenSave;
        this.equipSave = equipSave;
    }
}
[System.Serializable]
public class Inventory
{
    //private Inventory inven;

    public InventorySlot[] Items = new InventorySlot[24];
    public void Clear()
    {
        for (int i = 0; i < Items.Length; i++)
        {
            Items[i].RemoveItem();
            //Items[i].UpdateSlot(new Item(), 0);
        }
    }

    //public Inventory(Inventory inven)
    //{
    //    this.inven = inven;
    //}
}

[System.Serializable]
public class InventorySlot
{
    public ItemType[] AllowedItems = new ItemType[0];
    [System.NonSerialized]
    public UserInterface parent;
    //public int ID = -1;
    public Item item; // 아이템
    public int amount; // 아이템 갯수

    public ItemObject ItemObject
    {
        get
        {
            if (item.Id >= 0)
            {
                return parent.inventory.database.Items[item.Id];
            }
            return null;
        }
        set
        {

        }
    }
    public InventorySlot()
    {
        //ID = -1;
        item = new Item();
        amount = 0;
    }
    public InventorySlot(Item _item, int _amount)
    {
        //ID = _id;
        item = _item;
        amount = _amount;
    }
    public void UpdateSlot(Item _item, int _amount)
    {
        Debug.Log("UpdateSlot");
        //ID = _id;
        item = _item;
        amount = _amount;
    }
    public void AddAmount(int value) // 아이템 갯수 더하는 부분
    {
        amount += value;
    }
    public bool CanPlaceInSlot(ItemObject _itemObject) // 아이템 드갈수 있는지 확인하는 부분
    {
        if (AllowedItems.Length <= 0 || _itemObject == null || _itemObject.data.Id < 0) // 장비를 안가리거나, 비어있거나, ID가 -1이면(이것도 비어있거나란 말)
        {
            return true; // 가능함
        }
        for (int i = 0; i < AllowedItems.Length; i++) // 혹은 장비 허가값이 같으면(맞는 장비칸이면)
        {
            if (_itemObject.type == AllowedItems[i])
                return true; // 가능함
        }
        return false; // 둘다 아니면 불가능함
    }
    public void RemoveItem() // 아이템 제거
    {
        item = new Item();
        amount = 0;
    }
}
