using Cinemachine;
using Photon.Pun;
using System.Collections;
using UnityEngine;

public class GameScene : MonoBehaviour
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] CinemachineVirtualCamera highAngleCamera;

    private IEnumerator Start()
    {
        LoadRoomData();
        yield return new WaitUntil(() => Manager.Data.RoomData != null);
        GameStart();
    }

    private void LoadRoomData()
    {
        Manager.Data.LoadUserData();
        Manager.Data.LoadRoomData();
    }

    private void GameStart()
    {
        GameObject player = PhotonNetwork.Instantiate(playerPrefab.name, Manager.Data.RoomData.position, Quaternion.identity);
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

    private void OnDisable()
    {
        Manager.Data.SaveRoomData();
    }
}
