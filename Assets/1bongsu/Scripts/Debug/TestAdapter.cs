using UnityEngine;

public class TestAdapter : MonoBehaviour, IInteractable
{
    // 어댑터 적용 테스트
    [SerializeField] TestCube owner;

    public void Interact()
    {
        owner.Action();
    }
}
