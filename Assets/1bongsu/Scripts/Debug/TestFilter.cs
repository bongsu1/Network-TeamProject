using System.Text.RegularExpressions;
using UnityEngine;

public class TestFilter : MonoBehaviour
{
    [Header("Input Text")]
    [SerializeField] string input;
    [TextArea][SerializeField] string pattern;
    [SerializeField] string replacement;
    [Header("Result")]
    [SerializeField] string result;

    [ContextMenu("Regex Test")]
    private void RegexTest()
    {
        result = Regex.Replace(input, pattern, replacement); // 데이터에 추가하기 전에 되는지 실행용
    }

    [ContextMenu("Filter")]
    private void Filtering()
    {
        result = ChatFilter.Filtering(input);
    }

    [ContextMenu("AddText")]
    private void AddText()
    {
        ChatFilter.AddText(input, pattern, replacement);
    }

    [ContextMenu("Load")]
    private void LoadData()
    {
        ChatFilter.LoadData();
    }
}