using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Security.Cryptography;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using static System.Net.Mime.MediaTypeNames;

public class MenuPanel : MonoBehaviour
{
    [SerializeField] UserPanel userPanel;

    [SerializeField] TMP_InputField nickNameInputField;

    [SerializeField] InputFieldTabManager inputFieldTabMrg;

    [SerializeField] Button quickStartButton;
    [SerializeField] Button lobbyButton;
    [SerializeField] Button optionButton;
    [SerializeField] Button quitButton;

    [SerializeField] FirebaseUser currentUser;
    [SerializeField] GameObject LoginErrorPanel;
    [SerializeField] Button downButton;

    private void Awake()
    {
        quickStartButton.onClick.AddListener(QuickStart);
        lobbyButton.onClick.AddListener(JoinLobby);
        //optionButton.onClick.AddListener(JoinOption);
        quitButton.onClick.AddListener(Quit);

        inputFieldTabMrg = new InputFieldTabManager();
        inputFieldTabMrg.Add(nickNameInputField);
        downButton.onClick.AddListener(DownButton);
    }
    private void Start()
    {
        inputFieldTabMrg.SetFocus();

    }

    private void OnEnable()
    {
        StartCoroutine(ValueChangedDelay());
    }
    public void QuickStart()
    {
        NickNameCheck();

        string name = $"새로운 섬 {Random.Range(1, 1000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 20 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);

        PhotonNetwork.LoadLevel("GameScene"); // 게임 씬 추가
    }

    public void JoinLobby()
    {
        NickNameCheck();

        PhotonNetwork.JoinLobby();
    }
    public void Quit()
    {
        FirebaseManager.DB
                        .GetReference("UserData")
                        .Child(FirebaseManager.Auth.CurrentUser.UserId)
                        .Child("isLogin")
                        .SetValueAsync(false);

        FirebaseManager.Auth.SignOut();
        Application.Quit();
       
    }

    public void NickNameCheck()
    {
        FirebaseManager.DB
            .GetReference("UserData")
            .Child(FirebaseManager.Auth.CurrentUser.UserId)
            .Child("nickName")
            .GetValueAsync()
            .ContinueWithOnMainThread(task =>
            {
                if (task.IsCanceled || task.IsFaulted)
                {
                    Debug.Log("Get userdata canceled");
                    return;
                }

                DataSnapshot snapShot = task.Result;

                string json = snapShot.Value.ToString();

                if (json == "")
                {
                    nickNameInputField.text = ($"마을사람{Random.Range(1, 10000)}");
                    userPanel.ChangeNickName();
                }
            });
    }

    private void LoginCheck(object sender, ValueChangedEventArgs args)
    {

        FirebaseManager.DB 
                .GetReference("UserData")
                .Child(FirebaseManager.Auth.CurrentUser.UserId)
                .Child("isLogin")
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Debug.Log("Get userdata canceled");
                        return;
                    }

                    DataSnapshot snapShot = task.Result;

                    bool json = (bool)snapShot.Value;

                    if (json == false) 
                    {
                        LoginErrorPanel.SetActive(true);
                    }
                });
    }
    
    IEnumerator ValueChangedDelay()
    {
        yield return new WaitForSeconds(1f);

        FirebaseManager.DB
           .GetReference("UserData")
           .Child(FirebaseManager.Auth.CurrentUser.UserId)
           .OrderByChild("isLogin")
           .ValueChanged += LoginCheck;

        FirebaseManager.DB
                        .GetReference("UserData")
                        .Child(FirebaseManager.Auth.CurrentUser.UserId)
                        .Child("isLogin")
                        .SetValueAsync(true);
    }

    public void DownButton()
    {
        FirebaseManager.Auth.SignOut();
        PhotonNetwork.LoadLevel("AuthScene");
    }
}
