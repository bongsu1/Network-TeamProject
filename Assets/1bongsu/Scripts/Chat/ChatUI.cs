using System.Collections.Generic;

public class ChatUI : BillboardUI
{
    private Stack<ChatBubble> chatBubblesPool; // 챗풍선을 담는 용도, 오브젝트 풀링
    private Queue<ChatBubble> chatLog;         // 채팅이 너무 많이 올라오면 오래된것부터 비활성시켜 주기 위해 큐사용

    protected override void Awake()
    {
        base.Awake();

        chatBubblesPool = new Stack<ChatBubble>();
        foreach (ChatBubble chatBubble in GetComponentsInChildren<ChatBubble>(true))
        {
            chatBubblesPool.Push(chatBubble);
            chatBubble.Stack = chatBubblesPool;
        }
        chatLog = new Queue<ChatBubble>();
    }

    /// <summary>
    /// "chat"에 채팅에 보낼 메시지 입력,
    /// 채팅에 "chat"을 띄워줌
    /// </summary>
    /// <param name="chat"></param>
    public void SendChat(string chat)
    {
        ChatBubble chatBubble = chatBubblesPool.Pop();
        chatBubble.ChatText.text = chat;
        chatBubble.transform.SetAsLastSibling();      // 자식들중에 제일 마지막으로 보낸다 // 최신채팅은 아래쪽에 배치 
        chatBubble.gameObject.SetActive(true);

        chatBubble.Queue = chatLog;
        chatLog.Enqueue(chatBubble);
        if (chatLog.Count > 5)
        {
            ChatBubble oldChatBubble = chatLog.Dequeue();
            oldChatBubble.gameObject.SetActive(false);
        }
    }
}
