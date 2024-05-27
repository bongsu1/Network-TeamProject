using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class RoomEntry : MonoBehaviour
{
    [SerializeField] TMP_Text roomName;
    [SerializeField] TMP_Text currentPlayer;
    [SerializeField] TMP_Text ping;

    [SerializeField] GameObject passwordPopPanel;

    [SerializeField] TMP_InputField passwordInputField;

    [SerializeField] GameObject image;

    [SerializeField] Button joinRoomButton;
    [SerializeField] Button closePopButton;
    [SerializeField] Button connectButton;


    private RoomInfo roomInfo;
    private void Awake()
    {
        joinRoomButton.onClick.AddListener(JoinRoom);
        closePopButton.onClick.AddListener(ClosePopButton);
        connectButton.onClick.AddListener(ConnectButton);
        StartCoroutine(PingCheck());
    }

    public void SetRoomInfo(RoomInfo roomInfo)
    {
        this.roomInfo = roomInfo;
        roomName.text = (string)roomInfo.CustomProperties["roomName"]; //이름만 표시되도록
        currentPlayer.text = $"{roomInfo.PlayerCount} / {roomInfo.MaxPlayers}";
        joinRoomButton.interactable = roomInfo.PlayerCount < roomInfo.MaxPlayers;

        string password = (string)roomInfo.CustomProperties["password"];

        if (password != "" && password != null)
        {
            image.SetActive(true);
        }
    }

    public void JoinRoom()
    {
        // 선택한 방이 비밀번호가 있는지 체크
        string password = (string)roomInfo.CustomProperties["password"];

        if (password == "" || password == null)
        {
            // 비밀번호가 없다면
            PhotonNetwork.LeaveLobby();
            PhotonNetwork.JoinRoom(roomName.text);
        }
        else
        {
            passwordPopPanel.SetActive(true);
        }
    }

    public void ClosePopButton()
    {
        passwordPopPanel.SetActive(false);
    }

    public void ConnectButton()
    {
        //PhotonNetwork.LeaveLobby();
        PhotonNetwork.JoinRoom($"{roomName.text}{passwordInputField.text}");
    }

    IEnumerator PingCheck()
    {
        while (true)
        {
            ping.text = $"{PhotonNetwork.GetPing()} ms";
            yield return new WaitForSeconds(1f);
        }
    }
}
