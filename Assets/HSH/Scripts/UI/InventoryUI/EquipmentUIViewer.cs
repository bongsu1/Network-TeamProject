using UnityEngine;

public class EquipmentUIViewer : PopUpUI
{
    [SerializeField] PopUpUI InventoryViewer;
    [SerializeField] PopUpUI EquipmentViewer;
    public void TurnOnInventory()
    {
        Manager.UI.ClosePopUpUI();
        Manager.UI.ShowPopUpUI(InventoryViewer);
    }
    public void TurnOnEquipMent()
    {
        Manager.UI.ClosePopUpUI();
        Manager.UI.ShowPopUpUI(EquipmentViewer);
    }
}
