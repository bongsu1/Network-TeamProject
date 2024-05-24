using System.Collections.Generic;
using UnityEngine;

public class InventoryUIViewer : PopUpUI
{
    [SerializeField] PopUpUI InventoryViewer;
    [SerializeField] PopUpUI EquipmentViewer;

    private static List<InventoryUIViewer> m_PopUpUIs = new List<InventoryUIViewer>();

    // <summary>
    // Return the current InventoryUIViewer.
    // </summary>
    public static InventoryUIViewer current
    {
        get { return m_PopUpUIs.Count > 0 ? m_PopUpUIs[0] : null; }
        set
        {
            int index = m_PopUpUIs.IndexOf(value);

            if (index > 0)
            {
                m_PopUpUIs.RemoveAt(index);
                m_PopUpUIs.Insert(0, value);
            }
            else if (index < 0)
            {
                Debug.Log("Failed setting InventoryUIViewer.current to unknown InventoryUIViewer");
            }
        }
    }
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
