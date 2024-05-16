using System.Collections.Generic;

public static class ChatFilter
{
    private static List<string> badWordList;

    public static void SetList()
    {
        badWordList = new List<string>();
    }

    public static void AddScript(string script)
    {
        badWordList.Add(script);
    }

    /// <summary>
    /// "script"에 비속어 입력 시 필터링
    /// </summary>
    /// <param name="str"></param>
    public static string Filtering(string script)
    {
        foreach (string word in badWordList)
        {
            // test
            script = script.Replace(word, $"xxx");
        }
        return script;
    }
}
