using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class InventoryScene : BaseScene
{
    [SerializeField] PopUpUI InventoryUIViewer;
    public override IEnumerator LoadingRoutine()
    {
        yield return null;
    }
    /*private void Update()
    {
        if (Input.GetKeyDown(KeyCode.B))
        {
            Manager.UI.ShowPopUpUI(InventoryUIViewer);
        }
    }*/
}
