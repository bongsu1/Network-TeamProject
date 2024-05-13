using Firebase.Database;
using Firebase.Extensions;
using System.Collections;
using UnityEngine;

// test..
public class DebugDataManager : MonoBehaviour
{
    [Header("Auth")]
    [SerializeField] string email;
    [SerializeField] string pass;

    [Header("player Data")]
    private Test.UserData userData;
    public Test.UserData UserData { get { return userData; } }

    private static DebugDataManager instance;
    public static DebugDataManager Instance { get { return instance; } }

    private void Awake()
    {
        if (instance == null)
            instance = this;
    }

    [ContextMenu("Login")]
    public void Login()
    {
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                return;
            }
            else if (task.IsFaulted)
            {
                return;
            }

            Load();
            StartSaveRoutine(); // 자동저장 루틴
        });
    }

    private void Load()
    {
        FirebaseManager.DB.GetReference($"UserData/{FirebaseManager.Auth.CurrentUser.UserId}")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log($"Get Userdata canceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log($"Get Userdata failed : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists) // 
                {
                    string json = snapshot.GetRawJsonValue();
                    userData = JsonUtility.FromJson<Test.UserData>(json);
                }
                else
                {
                    userData = new Test.UserData();
                }
            });
    }

    [ContextMenu("Logout")]
    private void Logout()
    {
        FirebaseManager.Auth.SignOut();
        StopSaveRoutine();
    }

    private void StartSaveRoutine()
    {
        saveRoutine = StartCoroutine(SaveRoutine());
    }
    Coroutine saveRoutine;
    IEnumerator SaveRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(10f);
            Save();
        }
    }
    private void StopSaveRoutine()
    {
        Save();
        StopCoroutine(saveRoutine);
    }

    [ContextMenu("Save")]
    public void Save()
    {
        string json = JsonUtility.ToJson(userData);

        FirebaseManager.DB.GetReference($"UserData/{FirebaseManager.Auth.CurrentUser.UserId}")
            .SetRawJsonValueAsync(json)
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log($"Set Userdata canceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log($"Set Userdata failed : {task.Exception.Message}");
                    return;
                }

                Debug.Log("svae success");
            });
    }
}
