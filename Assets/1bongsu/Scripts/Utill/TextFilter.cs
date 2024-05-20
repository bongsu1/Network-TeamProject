using Firebase.Database;
using Firebase.Extensions;
using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;

public static class TextFilter
{
    private const string DB_PATH = "BadWord";

    private static List<BadWord> badWordList = new List<BadWord>();
    private static Dictionary<string, RegexGroup> regexDic = new Dictionary<string, RegexGroup>();

    // text를 필터링, 필터링할 글자를 replacement로 변경
    public static string Filtering(this string text)
    {
        foreach (BadWord word in badWordList)
        {
            text = regexDic[word.name].regex.Replace(text, word.replacement);
        }
        return text;
    }

    public static void LoadData() // 데이터베이스에서 불러오기
    {
        FirebaseManager.DB.GetReference(DB_PATH)
            .GetValueAsync().ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log("get data canceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log($"get data failed : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    BadWordList badWordList = JsonUtility.FromJson<BadWordList>(json);

                    InsertData(badWordList);
                }
            });
    }

    private static void SaveData() // 데이터베이스에 저장하기
    {
        string json = JsonUtility.ToJson(new BadWordList(badWordList));

        FirebaseManager.DB.GetReference(DB_PATH).SetRawJsonValueAsync(json);
    }

    // in runtime // isLoad 게임시작시 처음 로딩할 때만 true 적용할 것
    public static void AddText(string input, string pattern, string replacement = "xx", bool isLoad = false)
    {
        BadWord newBadWord = new BadWord(input, pattern, replacement);

        // 이미 있는 단어 일때(수정)
        if (regexDic.ContainsKey(input))
        {
            Debug.Log($"이미 있어서 여기로 들어옴 index : {regexDic[input].index}");
            regexDic[input].regex = new Regex(pattern, RegexOptions.Compiled);
            badWordList[regexDic[input].index] = newBadWord;
        }
        // 없는 단어를 추가 했을때
        else
        {
            regexDic.Add(newBadWord.name,
                new RegexGroup(new Regex(pattern, RegexOptions.Compiled), badWordList.Count));
            badWordList.Add(newBadWord);
        }

        if (!isLoad) // 런타임 도중에 추가 할때만 데이터 베이스에 저장
            SaveData();
    }

    // 데이터베이스에서 로딩후에 딕셔너리와 리스트에 데이터 추가
    private static void InsertData(BadWordList badWordList)
    {
        foreach (BadWord badWord in badWordList.badWords)
        {
            AddText(badWord.name, badWord.pattern, badWord.replacement, true);
        }
    }
}

[Serializable]
public class BadWordList
{
    public List<BadWord> badWords;

    public BadWordList(List<BadWord> badWords)
    {
        this.badWords = badWords;
    }
}

// 비속어 이름과 패턴, 변경될 단어를 묶어서 데이터관리
[Serializable]
public class BadWord
{
    public string name;
    public string pattern;
    public string replacement;

    public BadWord(string name, string pattern, string replacement)
    {
        this.name = name;
        this.pattern = pattern;
        this.replacement = replacement;
    }
}

public class RegexGroup
{
    public Regex regex;
    public int index; // list에 들어가 있는 BadWord의 index를 찾기 위해 쓰임

    public RegexGroup(Regex regex, int index = -1)
    {
        this.regex = regex;
        this.index = index;
    }
}