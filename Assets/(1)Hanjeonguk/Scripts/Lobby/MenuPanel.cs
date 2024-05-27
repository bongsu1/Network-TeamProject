using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

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

    [SerializeField] GameObject OptionsCanvas;


    private void Awake()
    {
        quickStartButton.onClick.AddListener(QuickStart);
        lobbyButton.onClick.AddListener(JoinLobby);
        optionButton.onClick.AddListener(JoinOption);
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

        string name = $"새로운 섬 {Random.Range(1, 1000)}";
        RoomOptions options = new RoomOptions();


        ExitGames.Client.Photon.Hashtable table = new ExitGames.Client.Photon.Hashtable();
        table.Add("roomName", name);

        options.MaxPlayers = 20;
        options.CustomRoomProperties = table;

        options.CustomRoomPropertiesForLobby = new string[] { "roomName" };

        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);
    }

    public void JoinLobby()
    {
        NickNameCheck();

        PhotonNetwork.JoinLobby();
    }
    public void JoinOption()
    {
        OptionsCanvas.SetActive(true);
    }
    public void Quit()
    {
        FirebaseManager.Auth.SignOut();
        Application.Quit();
    }

    public void NickNameCheck() //닉네임이 공란인지 체크
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
                string json = "";

                if (snapShot.Exists) //닉네임 있을 때 기존 닉네임 사용
                {
                    json = snapShot.Value.ToString();


                    if (json == "") //닉네임 있지만 공란일 때 랜덤 닉네임 사용
                    {
                        nickNameInputField.text = ($"섬 주민{Random.Range(1, 10000)}");
                        userPanel.ChangeNickName();
                    }
                    else
                    {
                        nickNameInputField.text = json;
                    }
                }

                else  //닉네임 없을 때 랜덤 닉네임 사용
                {
                    nickNameInputField.text = ($"섬 주민{Random.Range(1, 10000)}");
                    userPanel.ChangeNickName();
                }
            });
    }
}
