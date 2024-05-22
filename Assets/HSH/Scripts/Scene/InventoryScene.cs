using System.Collections;
using UnityEngine;

public class InventoryScene : BaseScene
{
    [SerializeField] PopUpUI InventoryUI;
    [SerializeField] PopUpUI EquipmentUI;
    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }

    public void TurnOnInventory()
    {
        Manager.UI.ShowPopUpUI(InventoryUI);
    }
    public void TurnOnEquipMent()
    {
        Manager.UI.ShowPopUpUI(EquipmentUI);
    }
}
