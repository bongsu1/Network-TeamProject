using System.Collections.Generic;
using UnityEngine;

public class TestFilter : MonoBehaviour
{
    [Header("Add Script")]
    [SerializeField] List<string> addList;
    [Header("filter")]
    [SerializeField] string script;
    [Header("Result")]
    [SerializeField] string result;

    private void Start()
    {
        ChatFilter.SetList();
        foreach (string item in addList)
        {
            ChatFilter.AddScript(item);
        }
    }

    [ContextMenu("Add List")]
    private void AddScript()
    {
        ChatFilter.AddScript(script);
        addList.Add(script);
    }

    [ContextMenu("Filter Script")]
    private void FilterScript()
    {
        result = ChatFilter.Filtering(script);
    }
}
