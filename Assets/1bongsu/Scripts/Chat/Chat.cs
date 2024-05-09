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
            playerInput.actions["Move"].Disable();
            chatInput.gameObject.SetActive(true);
            chatInput.ActivateInputField();
        }
        else
        {
            playerInput.actions["Move"].Enable();
            SendChat(chatInput.text);
            chatInput.text = "";
            chatInput.gameObject.SetActive(false);
        }
    }

    private void SendChat(string chat)
    {
        chatUI.SendChat(chat);
    }
}
