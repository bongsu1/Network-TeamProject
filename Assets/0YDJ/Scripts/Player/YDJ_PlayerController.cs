using Photon.Pun;
using UnityEngine;
using UnityEngine.InputSystem;

public class YDJ_PlayerController : MonoBehaviourPun
{
    [Header("Componet")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Animator animator;
    [SerializeField] Huntting huntting;
    [Header("Stat")]
    [SerializeField] float moveSpeed;
    [SerializeField] int hp;

    private Vector3 moveDir; // 입력받는 방향
    private bool isWalking; // 애니메이션 작동 변수

    private void Awake()
    {
        // 플레이어가 자기것이 아닐때
        if (!photonView.IsMine)
        {
            // 플레이어인풋 삭제
            Destroy(playerInput);

            gameObject.layer = 3;
        }
    }

    private void Update()
    {
        Turn();
        
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void Move()
    {
        rigid.MovePosition(transform.position + moveDir * moveSpeed * Time.deltaTime);
    }

    private void Turn()
    {
        if (huntting.TargetIn())
            return;

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
        

        if (isWalking != (value.Get<Vector2>() != Vector2.zero)) // 네트워크 최적화를 위해 달라질때만 호출
        {
            isWalking = value.Get<Vector2>() != Vector2.zero;
            photonView.RPC("ChangeWalkingAnimation", RpcTarget.All, isWalking); // 애니메이션 작동
        }

        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }

    //[PunRPC]
    //private void ChangeWalkingAnimation(bool isWalking, PhotonMessageInfo info)
    //{
    //    animator.SetBool("IsWalking", isWalking);
    //}



    //// 다정
    //private void PlayerDied()
    //{

    //    Destroy(gameObject);

    //}


}
