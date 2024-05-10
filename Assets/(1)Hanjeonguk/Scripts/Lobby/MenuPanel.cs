using Photon.Pun;
using Photon.Pun.Demo.Cockpit;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class MenuPanel : MonoBehaviour
{
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

        string name = $"Room {Random.Range(1, 1000)}";
        RoomOptions options = new RoomOptions() { MaxPlayers = 20 };
        PhotonNetwork.JoinRandomOrCreateRoom(roomName: name, roomOptions: options);

        PhotonNetwork.LoadLevel("GameScene"); // 게임 씬 추가
    }

    public void JoinLobby()
    {
        PhotonNetwork.JoinLobby();
    }
    public void Quit()
    {
        Application.Quit();
    }
}
