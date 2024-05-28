using Photon.Pun;
using UnityEngine;

public class PlayerClickUI : InGameUI
{
    [SerializeField] ClickPhotonHelper clickPhotonHelper;
    public void OpenTradeUI()
    {
        int myViewID = Manager.Inven.MyUserID;
        int opponentID = Manager.Inven.tradeUserID;

        Manager.UI.CloseInGameUI();
        object[] instantiationData = { myViewID, opponentID };
        Manager.Inven.playerController.GetComponent<PhotonView>().RPC("RequestOpenTradeUI", RpcTarget.All, instantiationData);
    }

}

