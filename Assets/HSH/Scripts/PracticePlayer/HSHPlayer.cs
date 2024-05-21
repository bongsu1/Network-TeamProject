using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class HSHPlayer : MonoBehaviourPun
{
    [Header("Componet")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Animator animator;
    [Header("Chat")]
    [SerializeField] Chat chat;
    [Header("Stat")]
    [SerializeField] float moveSpeed;
    [Header("Inventory")]
    [SerializeField] InventoryObject inventory;
    [SerializeField] InventoryObject equipment;

    private Vector3 moveDir; // 입력받는 방향
    private bool isWalking; // 애니메이션 작동 변수
    public bool IsWalking { get { return isWalking; } set { isWalking = value; OnChangeWalking?.Invoke(value); } }

    public UnityEvent<bool> OnChangeWalking;

    private void Awake()
    {
        // 플레이어가 자기것이 아닐때
        if (!photonView.IsMine)
        {
            // 플레이어인풋 삭제
            Destroy(playerInput);
        }

        chat.OnGreeting.AddListener(Greeting); // 인사트리거를 이벤트로 받는다
    }
    private void Start()
    {
        Manager.Inven.HSHplayer = this;
    }
    private void Update()
    {
        Turn();

        if (Input.GetKeyDown(KeyCode.I))
        {
            Debug.Log("Save");
            //inventory.Save();
            //equipment.Save();
            Manager.Inven.SaveToJson();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            Debug.Log("Load");
            //inventory.Load();
            //equipment.Load();
            Manager.Inven.LoadFromJson();
        }
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

    // InputSystem - Value : Vector2
    private void OnMove(InputValue value)
    {
        if (isWalking != (value.Get<Vector2>() != Vector2.zero)) // 네트워크 최적화를 위해 달라질때만 호출
        {
            IsWalking = value.Get<Vector2>() != Vector2.zero;
            photonView.RPC("ChangeWalkingAnimation", RpcTarget.All, isWalking); // 애니메이션 작동
        }

        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }

    private void Greeting()
    {
        animator.Play("Greeting");
    }

    [PunRPC]
    private void ChangeWalkingAnimation(bool isWalking, PhotonMessageInfo info)
    {
        animator.SetBool("IsWalking", isWalking);
    }
    private void OnDisable()
    {
        Debug.Log("Clear slot");
        inventory.Container.Clear();
        equipment.Container.Clear();
    }
}
