using Photon.Pun;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviourPun
{
    [Header("Componet")]
    [SerializeField] Rigidbody rigid;
    [SerializeField] PlayerInput playerInput;
    [SerializeField] Animator animator;
    [SerializeField] Action action;
    [Header("Chat")]
    [SerializeField] Chat chat;
    [Header("Stat")]
    [SerializeField] float moveSpeed;
    [Header("Inventory")]
    [SerializeField] InventoryObject inventory;
    [SerializeField] InventoryObject equipment;
    [SerializeField] PopUpUI InventoryUIViewer;
    [SerializeField] DynamicInterface inventorySlots;
    [Header("Sound")]
    [SerializeField] AudioSource walkingSound;

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
        if (photonView.IsMine)
        {
            Manager.Inven.HSHplayer = this;
        }
    }
    private void Update()
    {
        Turn();
        if (Input.GetKeyDown(KeyCode.B))
        {
            Window();
        }
        if (Input.GetKeyDown(KeyCode.I))
        {
            //Debug.Log("Save");
            //inventory.Save();
            //equipment.Save();
            Manager.Inven.SaveToJson();
        }
        if (Input.GetKeyDown(KeyCode.U))
        {
            //Debug.Log("Load");
            //inventory.Load();
            //equipment.Load();
            Manager.Inven.LoadFromJson();
        }
        // 이 아래 빌드

        //if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated) //인벤토리 창 열기
        //{
        //    Manager.UI.ShowPopUpUI(InvenView);
        //}

        //if (Input.GetKeyDown(KeyCode.B) && !isPreviewActivated) //인벤토리 창 열기
        //    Window();

        if (isPreviewActivated) // 프리뷰 상태 중 프리뷰 프리펩 계속 업데이트
        {
            PointerPosition();
            PreviewPositionUpdate();
        }

        if (Input.GetButtonDown("Fire1")) // 짓기
            Build();

        if (Input.GetKeyDown(KeyCode.Escape)) // 짓는 중에 캔슬
            Cancel();

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
        if (action.TargetIn())
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

    // InputSystem - Value : Vector2
    private void OnMove(InputValue value)
    {
        if (isWalking != (value.Get<Vector2>() != Vector2.zero)) // 네트워크 최적화를 위해 달라질때만 호출
        {
            IsWalking = value.Get<Vector2>() != Vector2.zero;
            //photonView.RPC("ChangeWalkingAnimation", RpcTarget.All, isWalking); // 애니메이션 작동
            photonView.RPC("SetAnimationParameter", RpcTarget.All, Parameter.SetBool, "IsWalking", isWalking);
            walkingSound.Play();
        }
        else if(!isWalking)
        {
            walkingSound.Stop();
        }
        moveDir.x = value.Get<Vector2>().x;
        moveDir.z = value.Get<Vector2>().y;
    }

    private void Greeting()
    {
        animator.Play("Greeting");
    }

    //[PunRPC]
    //private void ChangeWalkingAnimation(bool isWalking, PhotonMessageInfo info)
    //{
    //    animator.SetBool("IsWalking", isWalking);
    //}
    public void GuestDropItem(InventorySlot item)
    {
        photonView.RPC("RequestGuestDropItem", RpcTarget.MasterClient, item.item.Id, Manager.Inven.database.Items[item.item.Id].name);
        //Debug.Log($"000. {item.item.Id}");
        //Debug.Log($"001. {Manager.Inven.database.Items[item.item.Id].name}");
    }
    [PunRPC]
    private void RequestGuestDropItem(int id, string name)
    {
        if (PhotonNetwork.InRoom)
        {
            object[] instantiationData = { id, name };
            // Allbuffered로 해결 안되면 photonView.InstantiationData 
            // 룸 오브젝트 프리팹 인스턴스화
            GameObject roomObject = PhotonNetwork.InstantiateRoomObject("dropItemPrefab", transform.position, Quaternion.identity);

            roomObject.GetComponent<DropItem>().photonView.RPC("SetItemObject", RpcTarget.AllBuffered, id, name);
        }
        //photonView.RPC("ResultGuestDropItem", RpcTarget.AllViaServer, id, name);
        /*else
        {
            Debug.Log("dropItem in offline");

            // 룸 오브젝트 프리팹 인스턴스화
            GameObject roomObject = PhotonNetwork.InstantiateRoomObject("dropItemPrefab", Manager.Inven.dropPosition, Quaternion.identity);


            // 룸 오브젝트 내 DropItem 컴포넌트에 액세스해서 변경
            //DropItem 에 MonoBehaviourPun 달면 바로 답나오는 문제를 이래 헤매면 어떡하니 나야
            roomObject.GetComponent<DropItem>().photonView.RPC("SetItemObject", RpcTarget.All, item.item.Id, database.Items[item.item.Id].name);
        }*/
    }
    [PunRPC]
    private void ResultGuestDropItem(int id, string name)
    {
        //Debug.Log($"004. {id}");
        //Debug.Log($"005. {name}");
    }


    private void OnDisable()
    {
        //Debug.Log("Clear slot");
        if (photonView.IsMine)
        {
            Manager.Inven.HSHplayer = this;
            inventory.Container.Clear();
            equipment.Container.Clear();
        }

    }

    [PunRPC]
    public void SetAnimationParameter(Parameter type, string parameterName, object value)
    {
        switch (type)
        {
            case Parameter.SetBool:
                if (value is bool boolean)
                    animator.SetBool(parameterName, boolean);
                break;
            case Parameter.SetFloat:
                if (value is float single)
                    animator.SetFloat(parameterName, single);
                break;
            case Parameter.SetInteger:
                if (value is int integer)
                    animator.SetInteger(parameterName, integer);
                break;
            case Parameter.SetTrigger:
                animator.SetTrigger(parameterName);
                break;
        }
    }

    // 이 아래 빌드용


    //public static CraftManual instance;

    private bool isActivated = false;  // CraftManual UI 활성 상태
    private bool isPreviewActivated = false; // 미리 보기 활성화 상태

    [SerializeField] PopUpUI invenUI; // 기본 베이스 UI

    private GameObject go_Preview; // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수 

    //dk
    public Vector3 Pointer;
    //public Vector3 pointer { get { return Pointer; } }

    private RaycastHit hitInfo;

    [SerializeField] LayerMask ableLayer;

    [SerializeField] float range;

    public InventorySlot inventorySlot;


    private void PointerPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //카메라에서 레이 쏘기
        if (Physics.Raycast(ray, out hitInfo, range, ableLayer)) //레이캐스트로 그라운드 체크(range로 범위 조정 가능)                      
        {
            Pointer = hitInfo.point;

            if (hitInfo.transform.gameObject.layer == 31) // 바닥에 둘 때 오차 보정
            {
                Pointer.y = hitInfo.transform.position.y;
                return;
            }
            else
            {
                Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y;
                return;
            }

            //Debug.Log($"hitInfo.transform.position.y : {hitInfo.transform.position.y} , go_Preview.transform.localScale.y : {go_Preview.transform.localScale.y}");
            //if (hitInfo.transform.gameObject.layer == 31) // 바닥에 둘 때 오차 보정
            //{
            //    Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y - 0.5f;
            //    return;
            //}

            //if (go_Preview.transform.localScale.y > 1) // 두 칸 이상 전용
            //{
            //    if (hitInfo.transform.localScale.y > 1) // 두 칸 이상이 두 칸 이상 위에 쌓을 때
            //    {
            //        Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y;
            //        return;
            //    }
            //    Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y - (go_Preview.transform.localScale.y * 0.5f - 0.5f);  // 두 칸이 땅에 닿을 때 -0.5f
            //    return;
            //}
            //Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y + (hitInfo.transform.localScale.y * 0.5f - 0.5f);

            //Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y;

        }
    }
    public void SlotClick(InventorySlot Slot) //슬럿 클릭시 프리뷰 프리펩 생성
    {
        GameObject go_preview = Manager.Build.go_preview;
        GameObject go_prefab = Manager.Build.go_prefab;
        inventorySlot = Slot;
        go_Preview = Manager.Build.go_preview;
        go_Prefab = Manager.Build.go_prefab;
        //Debug.Log($"00. {go_prefab}");
        //Debug.Log($"01. {go_preview}");
        isPreviewActivated = Manager.Build.isPreviewActivated;
        go_Preview = Instantiate(go_Preview, Pointer, Quaternion.Euler(0, 0, 0));
        //go_Prefab = Manager.Build.go_prefab;
        isPreviewActivated = true;
    }


    private void PreviewPositionUpdate() // 미리보기 프리펩 위치 업데이트
    {

        if (Input.GetKey(KeyCode.R) && Input.GetKey(KeyCode.LeftShift)) // 로테이션
        {
            go_Preview.transform.Rotate(0, -2f, 0);
        }
        else if (Input.GetKey(KeyCode.R))
        {
            go_Preview.transform.Rotate(0, 2f, 0);
        }
        //Debug.Log(go_Preview.name);
        go_Preview.transform.position = Pointer;

    }


    private void Build() // 짓기
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            photonView.RPC("CreateCraftObject", RpcTarget.MasterClient, go_Prefab.name, Pointer, go_Preview.transform.rotation); // OthersBuffered를 사용하면 나중에 서버에 들어온 사람도 서버에 쌓인 데이터를 받을 수 있다
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
            inventorySlot.RemoveItem();
        }
    }

    [PunRPC]
    private void CreateCraftObject(string name, Vector3 pointer, Quaternion rotation)
    {
        PhotonNetwork.InstantiateRoomObject(name, pointer, rotation);
    }

    private void Window()
    {
        if (!isActivated)
            OpenWindow();
        else
            CloseWindow();
    }

    private void OpenWindow()
    {
        isActivated = true;
        Manager.UI.ShowPopUpUI(InventoryUIViewer);
    }

    private void CloseWindow()
    {
        isActivated = false;
        Manager.UI.ClosePopUpUI();
    }

    private void Cancel()
    {
        if (isPreviewActivated)
            Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;

        go_Preview = null;
        go_Prefab = null;

        //Manager.UI.ClosePopUpUI();
    }
}
