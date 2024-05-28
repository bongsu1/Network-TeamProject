using Cinemachine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using UnityEngine;

public class GameScene : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject playerPrefab;
    [SerializeField] CinemachineVirtualCamera playerFollowCamera;
    [SerializeField] CinemachineVirtualCamera highAngleCamera;

    private IEnumerator Start()
    {
        LoadRoomData();
        yield return new WaitUntil(() => Manager.Data.RoomData != null);

        if (PhotonNetwork.IsMasterClient)
            TextFilter.LoadData();
        GameStart();
    }

    private void LoadRoomData()
    {
        Manager.Data.LoadUserData();
        Manager.Data.LoadRoomData();
        Manager.Inven.LoadFromJson();
    }

    private void GameStart()
    {
        if (playerPrefab == null)
            return;

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

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (newMasterClient.IsLocal)
        {
            TextFilter.LoadData();
        }
    }

    public override void OnDisable()
    {
        base.OnDisable();

        Manager.Data.SaveRoomData();
        //Manager.Inven.SaveToJson();
    }
}
