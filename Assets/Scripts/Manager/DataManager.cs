using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System;
using System.IO;
using UnityEngine;

public class DataManager : Singleton<DataManager>
{
    #region Local
    private GameData gameData;
    public GameData GameData { get { return gameData; } }

#if UNITY_EDITOR
    private string path => Path.Combine(Application.dataPath, $"Resources/Data/SaveLoad");
#else
    private string path => Path.Combine(Application.persistentDataPath, $"Resources/Data/SaveLoad");
#endif

    public void NewData()
    {
        gameData = new GameData();
    }

    public void SaveData(int index = 0)
    {
        if (Directory.Exists(path) == false)
        {
            Directory.CreateDirectory(path);
        }

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText($"{path}/{index}.txt", json);
    }

    public void LoadData(int index = 0)
    {
        if (File.Exists($"{path}/{index}.txt") == false)
        {
            NewData();
            return;
        }

        string json = File.ReadAllText($"{path}/{index}.txt");
        try
        {
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Load data fail : {ex.Message}");
            NewData();
        }
    }

    public bool ExistData(int index = 0)
    {
        return File.Exists($"{path}/{index}.txt");
    }
    #endregion

    #region Firebase
    private UserData userData;
    public UserData UserData { get { return userData; } }

    private RoomData roomData;
    public RoomData RoomData { get { return roomData; } }

    public void LoadUserData()
    {
        if (!FirebaseManager.IsValid)
            return;

        string userID = FirebaseManager.Auth.CurrentUser.UserId;

        FirebaseManager.DB
            .GetReference($"UserData/{userID}")
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
                    userData = new UserData(); // 수정이 필요함
                }
            });
    }

    public void SaveUserData()
    {
        if (!FirebaseManager.IsValid)
            return;

        if (userData == null)
            userData = new UserData();

        string userID = FirebaseManager.Auth.CurrentUser.UserId;
        string json = JsonUtility.ToJson(userData);

        FirebaseManager.DB.GetReference($"UserData/{userID}").SetRawJsonValueAsync(json);
    }

    public void LoadRoomData()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (!FirebaseManager.IsValid)
            return;

        string roomName = PhotonNetwork.CurrentRoom.Name;
        string userID = FirebaseManager.Auth.CurrentUser.UserId;

        FirebaseManager.DB
            .GetReference($"RoomData/{roomName}/{userID}")
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
    }

    public void SaveRoomData()
    {
        if (!PhotonNetwork.InRoom)
            return;

        if (!FirebaseManager.IsValid)
            return;

        if (roomData == null)
            roomData = new RoomData();

        string roomName = PhotonNetwork.CurrentRoom.Name;
        string userID = FirebaseManager.Auth.CurrentUser.UserId;
        string json = JsonUtility.ToJson(roomData);

        FirebaseManager.DB.GetReference($"RoomData/{roomName}/{userID}").SetRawJsonValueAsync(json);
    }
    #endregion
}
