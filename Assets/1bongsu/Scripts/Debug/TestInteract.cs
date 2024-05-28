using UnityEngine;

public class TestInteract : MonoBehaviour, IInteractable
{
    // 인터페이스 적용 테스트

    public void Interact()
    {
        Debug.Log("인터페이스 적용");
    }
}
