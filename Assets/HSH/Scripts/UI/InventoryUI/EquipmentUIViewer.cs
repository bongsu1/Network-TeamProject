using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EquipmentUIViewer : PopUpUI
{
    [SerializeField] PopUpUI InventoryViewer;
    [SerializeField] PopUpUI EquipmentViewer;
    public void TurnOnInventory()
    {
        Manager.UI.ShowPopUpUI(InventoryViewer);
    }
    public void TurnOnEquipMent()
    {
        Manager.UI.ShowPopUpUI(EquipmentViewer);
    }
}
