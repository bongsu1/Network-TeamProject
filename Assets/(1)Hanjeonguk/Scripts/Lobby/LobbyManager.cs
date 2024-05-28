using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;

public class LobbyManager : MonoBehaviourPunCallbacks
{
    public enum Panel { Menu, Lobby}

    [SerializeField] MenuPanel menuPanel;
    [SerializeField] LobbyPanel lobbyPanel;

    private void Start()
    {
        SetActivePanel(Panel.Menu);
    }
 
    public override void OnJoinedLobby()
    {
        SetActivePanel(Panel.Lobby);
    }
    public override void OnLeftLobby()
    {
        SetActivePanel(Panel.Menu);
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        lobbyPanel.UpdateRoomList(roomList);
    }

    private void SetActivePanel(Panel panel)
    {
        menuPanel.gameObject.SetActive(panel == Panel.Menu);
        lobbyPanel.gameObject.SetActive(panel == Panel.Lobby);
    }
}
