using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using UnityEngine;

// test..
public class DebugDataManager : MonoBehaviour
{
    [Header("Auth")]
    [SerializeField] string email;
    [SerializeField] string pass;

    private UserData userData;
    public UserData UserData { get { return userData; } }

    private RoomData roomData;
    public RoomData RoomData { get { return roomData; } }

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
        string roomName = PhotonNetwork.CurrentRoom.Name;
        string userID = FirebaseManager.Auth.CurrentUser.UserId;
        #region userData
        FirebaseManager.DB.GetReference($"UserData/{userID}")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log($"Get UserData canceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log($"Get UserData failed : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    userData = JsonUtility.FromJson<UserData>(json);
                }
                else
                {
                    // test..
                    userData = new UserData("번개", true);
                    FirebaseManager.Instance.DataSave(userData);
                }
            });
        #endregion
        #region roomData
        FirebaseManager.DB.GetReference($"RoomData/{roomName}/{userID}")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled)
                {
                    Debug.Log($"Get RoomData canceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    Debug.Log($"Get RoomData failed : {task.Exception.Message}");
                    return;
                }

                DataSnapshot snapshot = task.Result;
                if (snapshot.Exists)
                {
                    string json = snapshot.GetRawJsonValue();
                    roomData = JsonUtility.FromJson<RoomData>(json);
                }
                else
                {
                    roomData = new RoomData();
                }
            });
        #endregion
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
            SaveRoomData();
        }
    }
    private void StopSaveRoutine()
    {
        SaveRoomData();
        StopCoroutine(saveRoutine);
    }

    [ContextMenu("Save")]
    public void SaveRoomData()
    {
        string json = JsonUtility.ToJson(roomData);
        string roomName = PhotonNetwork.CurrentRoom.Name;
        string userID = FirebaseManager.Auth.CurrentUser.UserId;

        FirebaseManager.DB.GetReference($"RoomData/{roomName}/{userID}").SetRawJsonValueAsync(json);
    }
}
