using Firebase;
using Firebase.Auth;
using Firebase.Database;
using UnityEngine;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class FirebaseManager : MonoBehaviour
{
    private static FirebaseManager instance;
    public static FirebaseManager Instance { get { return instance; } }

    private static FirebaseApp app;
    public static FirebaseApp App { get { return app; } }

    private static FirebaseAuth auth;
    public static FirebaseAuth Auth { get { return auth; } }

    private static FirebaseDatabase db;
    public static FirebaseDatabase DB { get { return db; } }

    private static bool isValid;
    public static bool IsValid { get { return isValid; } }

    private void Awake()
    {
        CreateInstance();
        CheckDependency();
    }

    private void CreateInstance()
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

    private async void CheckDependency()
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
}
