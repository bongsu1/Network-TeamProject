using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ChatBubble : MonoBehaviour
{
    [SerializeField] TMP_Text chatText;

    private Queue<ChatBubble> queue; // ChatUI Queue<ChatBubble> 할당
    private Stack<ChatBubble> stack; // 오브젝트 풀링(돌아갈곳)

    public TMP_Text ChatText { get { return chatText; } set { chatText = value; } }
    public Queue<ChatBubble> Queue { set { queue = value; } }
    public Stack<ChatBubble> Stack { set { stack = value; } }

    private void OnEnable()
    {
        chatActiveRoutine = StartCoroutine(ChatActiveRoutine());
    }

    private void OnDisable()
    {
        if (chatActiveRoutine != null) // 시간이 남아도 채팅이 많아지면 제거
        {
            StopCoroutine(chatActiveRoutine);
            chatActiveRoutine = null;
            queue.Dequeue();
        }
        stack.Push(this); // 비활성화 되면 스택으로 돌아감
        chatText.text = "";
    }

    // 챗풍선이 3초간 보이다 꺼진다
    Coroutine chatActiveRoutine;
    IEnumerator ChatActiveRoutine()
    {
        yield return new WaitForSeconds(3f);
        chatActiveRoutine = null;
        gameObject.SetActive(false);
    }
}
