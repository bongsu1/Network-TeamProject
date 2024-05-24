using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildManager : Singleton<BuildManager>
{
    [SerializeField] public GameObject go_preview;
    [SerializeField] public GameObject go_prefab;
    [SerializeField] public bool isPreviewActivated;
}
