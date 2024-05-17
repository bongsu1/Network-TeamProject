using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageObject : MonoBehaviour, HSH_IInteractable
{
    public InventoryObject storage;

    private void OnApplicationQuit()
    {
        storage.Container.Clear();
    }

    public void Interact()
    {
        
    }
}
