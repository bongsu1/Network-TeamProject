using Cinemachine;
using Firebase.Extensions;
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
    bool isLogin;
    IEnumerator GameStartDelay()
    {
        yield return new WaitForSeconds(1f);

        // test..
        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                return;
            }
            else if (task.IsFaulted)
            {
                return;
            }

            isLogin = true;
        }); ;


        yield return new WaitUntil(() => isLogin);
        Manager.Data.LoadRoomData();
        yield return new WaitUntil(() => Manager.Data.RoomData != null);
        
        GameStart();
    }
    private void GameStart()
    {
        // test..
        Vector3 spawnPosition = Manager.Data.RoomData == null ? Vector3.zero : Manager.Data.RoomData.position;

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
