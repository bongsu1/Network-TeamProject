using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Chat : MonoBehaviourPun
{
    [SerializeField] PlayerInput playerInput;
    [Header("Chat UI")]
    [SerializeField] ChatUI chatUI;

    private TMP_InputField chatInput;
    private bool isChatting;           // 채팅중이면 true

    private void Start()
    {
        chatInput = chatUI.GetUI<TMP_InputField>("ChatInputField");
    }

    private void OnChat(InputValue value)
    {
        isChatting = !isChatting;
        if (isChatting)
        {
            playerInput.actions["Move"].Disable();  // 채팅입력중에는 움직이는 키 비활성
            chatInput.gameObject.SetActive(true);
            chatInput.ActivateInputField();         // 인풋필드입력 활성화
        }
        else
        {
            playerInput.actions["Move"].Enable();
            chatInput.gameObject.SetActive(false);
            if (chatInput.text == "")              // 채팅입력창이 비어 있으면 취소
                return;

            chatUI.SendChat(chatInput.text);
            chatInput.text = "";
        }
    }
}
