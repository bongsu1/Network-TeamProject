using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class HSHDebugManager : MonoBehaviourPunCallbacks
{
    [SerializeField] string debugRoomName;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] CinemachineVirtualCamera highAngleCamera;

    [Header("Auth")]
    [SerializeField] string email;
    [SerializeField] string pass;

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

    [SerializeField] bool isDebug;
    public override void OnJoinedRoom()
    {
        if (!isDebug)
            StartCoroutine(GameStartDelay());
    }

    IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(1f);
        // test..
        if (DebugDataManager.Instance != null)
        {
            DebugDataManager.Instance.Login();
            yield return new WaitUntil(() => DebugDataManager.Instance.RoomData != null);
        }
        GameStart();
    }
    private void GameStart()
    {
        // test..
        Vector3 spawnPosition = DebugDataManager.Instance == null ? Vector3.zero : DebugDataManager.Instance.RoomData.position;

        GameObject player = PhotonNetwork.Instantiate("YDJ_Player_Test", spawnPosition, Quaternion.identity);
        if (playerFollowCamera != null)
        {
            playerFollowCamera.gameObject.SetActive(true);
            playerFollowCamera.Follow = player.transform;
            playerFollowCamera.LookAt = player.transform;
        }
        if (highAngleCamera != null)
        {
            highAngleCamera.gameObject.SetActive(true);
            highAngleCamera.Follow = player.transform;
            highAngleCamera.LookAt = player.transform;
        }

    }
}
