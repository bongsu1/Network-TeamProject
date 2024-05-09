using System.Collections.Generic;
using UnityEngine;

public class ChatUI : BillboardUI
{
    private Stack<ChatBubble> chatBubblesPool; // 챗풍선을 담는 용도, 오브젝트 풀링
    private Queue<ChatBubble> chatLog;         // 채팅이 너무 많이 올라오면 오래된것부터 비활성시켜 주기 위해 큐사용

    protected override void Awake()
    {
        base.Awake();

        chatBubblesPool = new Stack<ChatBubble>();
        foreach (ChatBubble chatBubble in GetComponentsInChildren<ChatBubble>())
        {
            chatBubblesPool.Push(chatBubble);
            chatBubble.Stack = chatBubblesPool;
        }
        chatLog = new Queue<ChatBubble>();
        Debug.Log(chatBubblesPool.Count);
    }

    public void SendChat(string chat)
    {
        ChatBubble chatBubble = chatBubblesPool.Pop();
        chatBubble.ChatText.text = chat;
        chatBubble.gameObject.SetActive(true);

        chatBubble.Queue = chatLog;
        chatLog.Enqueue(chatBubble);
        if (chatLog.Count > 5)
        {
            ChatBubble oldChatBubble = chatLog.Dequeue();
            chatBubblesPool.Push(oldChatBubble);
            oldChatBubble.gameObject.SetActive(false);
        }
    }
}
