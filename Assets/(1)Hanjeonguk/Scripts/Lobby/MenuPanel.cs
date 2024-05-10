using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
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

    private void Awake()
    {
        quickStartButton.onClick.AddListener(QuickStart);
        lobbyButton.onClick.AddListener(JoinLobby);
        //optionButton.onClick.AddListener(JoinOption);
        quitButton.onClick.AddListener(Quit);

        inputFieldTabMrg = new InputFieldTabManager();
        inputFieldTabMrg.Add(nickNameInputField);
    }
    private void Start()
    {
        inputFieldTabMrg.SetFocus();
    }

    public void QuickStart()
    {
        NickNameCheck();

        string name = $"Room {Random.Range(1, 1000)}";
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
                    nickNameInputField.text = ($"Name {Random.Range(1, 1000)}");
                    userPanel.ChangeNickName();
                }
            });
    }
}
