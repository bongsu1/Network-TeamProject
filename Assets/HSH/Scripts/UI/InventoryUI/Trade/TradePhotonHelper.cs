using Photon.Pun;
using UnityEngine;

public class TradePhotonHelper : MonoBehaviourPun
{
    public bool MyTradeOk = false;
    public bool TradeUserTradeOk = false;
    public int MyViewID;
    public int OpponentID;
    [SerializeField] GameObject MyOkButton;
    [SerializeField] GameObject MyCancelButton;
    [SerializeField] GameObject TradeUserOk;
    [SerializeField] GameObject TradeUserCancel;
    [SerializeField] InventoryObject originInven;
    [SerializeField] InventoryObject tradeInven;
    [SerializeField] InventoryObject opponentInven;

    [SerializeField] GameObject TradeUI;
    private void Start()
    {
        MyTradeOk = false;
        TradeUserTradeOk = false;
    }

    private void Update()
    {
        TradeSuccess();
    }
    public void MyOk()
    {
        //MyViewID = Manager.Inven.MyUserID;
        MyTradeOk = true;
        MyOkButton.SetActive(false);
        MyCancelButton.SetActive(true);
        int myViewId = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID;
        object[] instantiationData = { myViewId };
        photonView.RPC("OpponentCheckOK", RpcTarget.All, instantiationData);
    }
    public void MyOkCancel()
    {
        MyTradeOk = false;
        MyOkButton.SetActive(true);
        MyCancelButton.SetActive(false);
        int myViewId = Manager.Inven.playerController.GetComponent<PhotonView>().ViewID;
        object[] instantiationData = { myViewId };
        photonView.RPC("OpponentCheckCancel", RpcTarget.All, instantiationData);
    }
    [PunRPC]
    public void OpponentCheckOK(int ViewId)
    {
        if (ViewId == Manager.Inven.tradeUserID)
        {
            TradeUserTradeOk = true;
            TradeUserOk.SetActive(false);
            TradeUserCancel.SetActive(true);
        }
        else
        {
            return;
        }
    }
    [PunRPC]
    public void OpponentCheckCancel(int ViewId)
    {
        TradeUserTradeOk = false;
        TradeUserOk.SetActive(true);
        TradeUserCancel.SetActive(false);
        if (ViewId == Manager.Inven.tradeUserID)
        {
            TradeUserTradeOk = false;
            TradeUserOk.SetActive(true);
            TradeUserCancel.SetActive(false);
            MyTradeOk = false;
            MyOkButton.SetActive(true);
            MyCancelButton.SetActive(false);
        }
        else
        {
            return;
        }
    }
    public void TradeSuccess()
    {
        Debug.Log("TradeSuccess");
        int myID = Manager.Inven.MyUserID;
        int tradeID = Manager.Inven.tradeUserID;
        object[] instantiationData = { myID, tradeID };
        if (TradeUserTradeOk == true && MyTradeOk == true)
        {
            photonView.RPC("EndTrade", RpcTarget.All, instantiationData);
        }
    }

    [PunRPC]
    public void EndTrade(int myID, int tradeID)
    {
        if (tradeID == Manager.Inven.MyUserID || myID == Manager.Inven.MyUserID)
        {
            for (int i = 0; i < opponentInven.Container.Items.Length; i++)
            {
                if (opponentInven.Container.Items[i].item.Id >= 0)
                {
                    for (int j = 0; j < originInven.Container.Items.Length; j++)
                    {
                        if (originInven.Container.Items[j].item.Id >= 0)
                        {
                            continue;
                        }
                        else if (originInven.Container.Items[j].item.Id < 0)
                        {
                            originInven.SwapItems(opponentInven.Container.Items[i], originInven.Container.Items[j]);
                            continue;
                        }
                    }
                }
            }
            MyTradeOk = false;
            TradeUserTradeOk = false;
            opponentInven.Container.Clear();
            tradeInven.Container.Clear();
            TradeUI.SetActive(false);
            return;
        }
    }
}
