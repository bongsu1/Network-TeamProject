using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StorageObject : MonoBehaviour
{
    public InventoryObject storage;

    private void OnApplicationQuit()
    {
        storage.Container.Clear();
    }

}
