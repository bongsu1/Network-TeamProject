using Firebase;
using Firebase.Auth;
using Firebase.Database;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class FirebaseManager : MonoBehaviourPunCallbacks
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance { get { return instance; } }

    private static FirebaseApp app;
    public static FirebaseApp App { get { return app; } }

    private static FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return auth; } }

    private static FirebaseDatabase db;
    public static FirebaseDatabase DB { get { return db; } }

    private static FirebaseUser user;
    public static FirebaseUser User { get { return user; } }

    private static bool isValid;
    public static bool IsValid { get { return isValid; } }


    private void Awake()
    {
        CreateInstance();

        CheckDependency();
    }

    private void OnDisable() //게임 종료시 로그인 상태 false
    {
        db.GetReference("UserData").Child(FirebaseManager.Auth.CurrentUser.UserId).Child("isLogin").SetValueAsync(false);
        PhotonNetwork.LeaveRoom();
    }
    private void CreateInstance() //싱글톤
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private async void CheckDependency() //Firebase 인증 초기화
    {
        DependencyStatus dependencyStatus = await FirebaseApp.CheckAndFixDependenciesAsync();
        if (dependencyStatus == DependencyStatus.Available)
        {
            app = FirebaseApp.DefaultInstance;
            auth = FirebaseAuth.DefaultInstance;
            db = FirebaseDatabase.DefaultInstance;

            Debug.Log("Firebase Check and FixDependencies success");
            isValid = true;


        }
        else
        {
            Debug.LogError("Firebase Check and FixDependencies fail");
            isValid = false;

            app = null;
            auth = null;
            db = null;
        }
    }

    public void DataSave(UserData userData) //현재 데이터 Firebase에 저장
    {
        string json = JsonUtility.ToJson(userData);

        db
            .GetReference("UserData")
            .Child(auth.CurrentUser.UserId)
            .SetRawJsonValueAsync(json);
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("접속종료");
        db.GetReference("UserData").Child(FirebaseManager.Auth.CurrentUser.UserId).Child("isLogin").SetValueAsync(false);
    }
}
