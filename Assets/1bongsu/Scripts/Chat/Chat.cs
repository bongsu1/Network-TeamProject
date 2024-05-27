using Photon.Pun;
using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Chat : MonoBehaviourPun
{
    [SerializeField] PlayerInput playerInput;
    [Header("Chat UI")]
    [SerializeField] ChatUI chatUI;
    [SerializeField] Image chattingImage; // 채팅중에 띄우는 이미지

    private TMP_InputField chatInput;
    private CameraMover cameraMover;
    private bool isChatting;           // 채팅중이면 true

    public UnityEvent<bool> OnChatting;

    public UnityEvent OnGreeting;

    [Header("Sound")]
    [SerializeField] AudioSource ChatSound;

    private void Start()
    {
        chatInput = chatUI.GetUI<TMP_InputField>("ChatInputField");

        cameraMover = FindObjectOfType<CameraMover>();
        if (cameraMover != null)
            OnChatting.AddListener(cameraMover.SetIsChatting);
    }

    private void OnDisable()
    {
        if (cameraMover != null)
            OnChatting.RemoveListener(cameraMover.SetIsChatting);
    }

    private void OnChat(InputValue value)
    {
        isChatting = !isChatting;

        OnChatting?.Invoke(isChatting); // 채팅중이라는 것을 이벤트로 알리기

        chatInput.gameObject.SetActive(isChatting);
        photonView.RPC("InChatting", RpcTarget.Others, isChatting); // 채팅중에는 상대방들에게 알리는 이미지 띄우기
        if (isChatting)
        {
            playerInput.actions["Move"].Disable();  // 채팅입력중에는 움직이는 키 비활성
            playerInput.actions["Interact"].Disable();  // 채팅입력중에는 움직이는 키 비활성
            playerInput.actions["Greet"].Disable();  // 채팅입력중에는 인사 키 비활성
            chatInput.ActivateInputField();         // 인풋필드입력 활성화
            chatInput.transform.SetAsLastSibling();
        }
        else
        {
            playerInput.actions["Move"].Enable();
            playerInput.actions["Interact"].Enable();
            playerInput.actions["Greet"].Enable();

            int empty = chatInput.textComponent.text.Trim().Length; // 입력필드가 활성화 되면 비어있어도 하나가 남는다? 이유 모름
            if (empty <= 1) // 채팅입력창이 비어 있으면 취소
                return;

            // IME때문에 한글입력이 완료되지 않았다고 판단되어 마지막 글자가 인풋필드에 입력되지 않는다
            // 한글로 입력시 마지막 글자는 미완성 처리가 되어서 <u>'글자'</u> 로 전달되어서
            // Replace로 앞에글자를 지워서 비속어 필터에서 사용할 수 있어진다.
            string chat = chatInput.textComponent.text.Replace("<u>", "");

            //string chat = chatInput.text;

            #region TextFilter addText
            // 명령어로 비속어 추가 하기 "/add 비속어,패턴,(대체될 단어 -선택사항-)"
            // 추가 명령어는 마스터클라이언트만 가능
            Match addCommand = Regex.Match(chat, @"^/add\s[\w\W]+");
            if (addCommand.Success && PhotonNetwork.IsMasterClient)
            {
                string[] group = chat.Split(',', ' ');
                switch (group.Length)
                {
                    case 3:
                        TextFilter.AddText(group[1], group[2]);
                        break;
                    case 4:
                        TextFilter.AddText(group[1], group[2], group[3]);
                        break;
                    default:
                        break;
                }

                chatInput.text = "";
                return;
            }
            #endregion

            // 비속어 필터를 사용하기 위해 마스터에게만 송신
            photonView.RPC("ChatFilter", RpcTarget.MasterClient, chat);

            chatInput.text = "";
        }
    }

    // InputSystem - Button
    private void OnGreet()
    {
        playerInput.actions["Greet"].Disable(); // 인사가 끝날때 까지 다시 누르지 못하게
        playerInput.actions["Move"].Disable(); // 인사가 끝날때 까지 움직이지 못하게

        photonView.RPC("Greeting", RpcTarget.AllViaServer, Manager.Data.UserData.nickName);
        StartCoroutine(GreetingRoutine());
    }
    IEnumerator GreetingRoutine()
    {
        yield return new WaitForSeconds(2f);
        if (!isChatting) // 채팅중이라면 인사가 끝나도 못 움직이게 설정
        {
            playerInput.actions["Greet"].Enable();
            playerInput.actions["Move"].Enable();
        }
    }

    [PunRPC]
    private void Greeting(string nickName, PhotonMessageInfo info)
    {
        OnGreeting?.Invoke(); // 이벤트로 플레이어의 애니메이션 동작을 트리거
        chatUI.SendChat(nickName, false);
    }

    [PunRPC]
    private void ChatFilter(string chat)
    {
        // 비속어를 필터링후에 메세지 보내기
        photonView.RPC("SendChat", RpcTarget.AllViaServer, chat.Filtering());
    }

    [PunRPC]
    private void SendChat(string chat)
    {
        chatUI.SendChat(chat);
        chattingImage.transform.SetAsLastSibling();
        ChatSound.Play();
    }

    [PunRPC]
    private void InChatting(bool isChatting)
    {
        chattingImage.gameObject.SetActive(isChatting);
        if (isChatting)
            chattingImage.transform.SetAsLastSibling();
    }
}
