using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Chat : MonoBehaviourPun
{
    [SerializeField] PlayerInput playerInput;
    [Header("Chat UI")]
    [SerializeField] ChatUI chatUI;
    [SerializeField] Image chattingImage; // 채팅중에 띄우는 이미지

    private TMP_InputField chatInput;
    private bool isChatting;           // 채팅중이면 true

    private void Start()
    {
        chatInput = chatUI.GetUI<TMP_InputField>("ChatInputField");
    }

    private void OnChat(InputValue value)
    {
        isChatting = !isChatting;

        chatInput.gameObject.SetActive(isChatting);
        photonView.RPC("InChatting", RpcTarget.Others, isChatting); // 채팅중에는 상대방들에게 알리는 이미지 띄우기
        if (isChatting)
        {
            playerInput.actions["Move"].Disable();  // 채팅입력중에는 움직이는 키 비활성
            playerInput.actions["Interact"].Disable();  // 채팅입력중에는 움직이는 키 비활성
            chatInput.ActivateInputField();         // 인풋필드입력 활성화
            chatInput.transform.SetAsLastSibling();
        }
        else
        {
            playerInput.actions["Move"].Enable();
            playerInput.actions["Interact"].Enable();

            if (chatInput.text == "")              // 채팅입력창이 비어 있으면 취소
                return;

            // IME때문에 한글입력이 완료되지 않았다고 판단되어 마지막 글자가 인풋필드에 입력되지 않는다
            string chat = chatInput.textComponent.text;

            // 비속어 필터를 사용하기 위해 마스터에게만 송신
            photonView.RPC("ChatFilter", RpcTarget.MasterClient, chat);

            chatInput.text = "";
        }
    }

    [PunRPC]
    private void ChatFilter(string chat)
    {
        // 비속어가 있다면 거르고 채팅 보내지 않기 or 별표시
        // ex)
        if (chat == "aaaa")
            chat = "xxxx";

        photonView.RPC("SendChat", RpcTarget.AllViaServer, chat);
    }

    [PunRPC]
    private void SendChat(string chat)
    {
        chatUI.SendChat(chat);
        chattingImage.transform.SetAsLastSibling();
    }

    [PunRPC]
    private void InChatting(bool isChatting)
    {
        chattingImage.gameObject.SetActive(isChatting);
        if (isChatting)
            chattingImage.transform.SetAsLastSibling();
    }
}
