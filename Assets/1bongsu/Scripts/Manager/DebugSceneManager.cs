using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class DebugSceneManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string debugRoomName;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera;

    private void Start()
    {
        PhotonNetwork.LocalPlayer.NickName = $"player {Random.Range(1000, 10000)}";
        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        RoomOptions options = new RoomOptions();
        options.MaxPlayers = 8;
        TypedLobby typeLobby = new TypedLobby("Debug", LobbyType.Default); 

        PhotonNetwork.JoinOrCreateRoom(debugRoomName, options, typeLobby);
    }

    public override void OnJoinedRoom()
    {
        StartCoroutine(GameStartDelay());
    }

    IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(1f);
        GameStart();
    }
    private void GameStart()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", Vector3.zero, Quaternion.identity);
        playerFollowCamera.gameObject.SetActive(true);
        playerFollowCamera.Follow = player.transform;
        playerFollowCamera.LookAt = player.transform;
    }
}
