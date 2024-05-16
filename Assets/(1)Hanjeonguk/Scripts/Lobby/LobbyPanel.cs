using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbyPanel : MonoBehaviour
{
    [SerializeField] RectTransform roomContent;
    [SerializeField] RoomEntry roomEntryPrefab;
    [SerializeField] GameObject createRoomPanel;

    [SerializeField] Button createRoomButton;
    [SerializeField] Button createRoomCloseButton;
    [SerializeField] Button LeaveButton;
    [SerializeField] Button createRoomConfirmButton;

    [SerializeField] TMP_InputField roomNameInputField;
    [SerializeField] TMP_InputField maxPlayerInputField;

    [SerializeField] InputFieldTabManager inputFieldTabMrg;

    private Dictionary<string, RoomEntry> roomDictionary;

    private void Awake()
    {
        roomDictionary = new Dictionary<string, RoomEntry>();
        LeaveButton.onClick.AddListener(LeaveLobby);
        createRoomButton.onClick.AddListener(CreateRoomPanel);
        createRoomCloseButton.onClick.AddListener(CreateRoomPanelClose);
        createRoomConfirmButton.onClick.AddListener(CreateRoomConfirm);

        inputFieldTabMrg = new InputFieldTabManager();
        inputFieldTabMrg.Add(roomNameInputField);
        inputFieldTabMrg.Add(maxPlayerInputField);
    }
    private void Start()
    {
        inputFieldTabMrg.SetFocus();
    }
    private void Update()
    {
        inputFieldTabMrg.CheckFocus();
    }

    private void OnDisable()
    {
        for (int i = 0; i < roomContent.childCount; i++)
        {
            Destroy(roomContent.GetChild(i).gameObject);
        }

        roomDictionary.Clear();
    }

    public void CreateRoomConfirm()
    {
        string roomName = roomNameInputField.text; //방 이름 설정
        if (roomName == "") //이름이 비어있을 때
        {
            roomName = $"Room{Random.Range(1000, 1000)}"; //랜덤 이름 적용
        }

        int maxPlayer = maxPlayerInputField.text == "" ? 20 : int.Parse(maxPlayerInputField.text);
        
        maxPlayer = Mathf.Clamp(maxPlayer, 1, 20); 

        RoomOptions options = new RoomOptions();
        options.MaxPlayers = maxPlayer;
        PhotonNetwork.CreateRoom(roomName, options);

    }

    public void LeaveLobby()
    {
        PhotonNetwork.LeaveLobby();
    }

    public void CreateRoomPanel()
    {
        createRoomPanel.SetActive(true); 
    }
    public void CreateRoomPanelClose()
    {
        createRoomPanel.SetActive(false);
    }

    public void UpdateRoomList(List<RoomInfo> roomList)
    {
        //Update room info
        foreach (RoomInfo roomInfo in roomList)
      
            //1. 방이 사라지는 경우
            if (roomInfo.RemovedFromList || roomInfo.IsOpen == false || roomInfo.IsVisible == false)
            //방이 삭제된 경우, 들어갈 수 없는 경우, 비공개인 경우
            {
                if (roomDictionary.ContainsKey(roomInfo.Name) == false) //방을 가지고 있을 때 진행
                    continue;

                RoomEntry roomEntry = roomDictionary[roomInfo.Name]; //이름으로 찾기
                roomDictionary.Remove(roomInfo.Name); //딕셔너리 삭제
                Destroy(roomEntry.gameObject); //오브젝트 삭제
            }

            //2. 방의 내용물이 바뀌는 경우
            else if (roomDictionary.ContainsKey(roomInfo.Name))
            {
                RoomEntry roomEntry = roomDictionary[roomInfo.Name];
                roomEntry.SetRoomInfo(roomInfo);
            }

            //3. 방이 생기는 경우 
            else
            {
                RoomEntry roomEntry = Instantiate(roomEntryPrefab, roomContent);
                roomEntry.SetRoomInfo(roomInfo);
                roomDictionary.Add(roomInfo.Name, roomEntry);
            }
        }
    }

