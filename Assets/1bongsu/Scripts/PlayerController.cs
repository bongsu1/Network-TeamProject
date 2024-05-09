using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun
{
    [Header("Componet")]
    [SerializeField] CharacterController characterController;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Animator animator;
    [Header("Stat")]
    [SerializeField] float moveSpeed;

    private Vector3 moveDir; // 입력받는 방향

    private void Awake()
    {
        // 플레이어가 자기것이 아닐때
        if (!photonView.IsMine)
        {
            // 플레이어인풋 삭제
            Destroy(playerInput);
        }
    }

    private void Update()
    {
        Move();
        Turn();
    }

    private void Move()
    {
        characterController.Move(moveDir * moveSpeed * Time.deltaTime);
    }

    private void Turn()
    {
        Vector3 inputDir = transform.position + moveDir; // 입력하는 방향
        Vector3 nextDir;                                 // 바라볼 방향
        if (-transform.forward == moveDir) // 바라보는 방향과 입력방향이 반대일때는 방향이 바뀌지 않아서 추가
        {
            nextDir = Vector3.Lerp(transform.position + transform.forward, transform.position + transform.right, .25f);
        }
        else
        {
            nextDir = Vector3.Lerp(transform.position + transform.forward, inputDir, .5f);
        }
        transform.LookAt(nextDir);
    }

    // 인풋시스템으로 움직이는 방향 입력 받기
    private void OnMove(InputValue value)
    {
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }
}
