using Photon.Pun;
using UnityEngine;


[System.Serializable]
public class Craft
{
    public string craftName; // 이름
    public GameObject go_prefab; // 실제 설치 될 프리팹
    public GameObject go_PreviewPrefab; // 미리 보기 프리팹
}

public class CraftManual : MonoBehaviourPun
{
    //public static CraftManual instance;

    private bool isActivated = false;  // CraftManual UI 활성 상태
    private bool isPreviewActivated = false; // 미리 보기 활성화 상태


    [SerializeField] GameObject go_BaseUI; // 기본 베이스 UI


    [SerializeField] Craft[] craft_fire;  // 인벤토리에 있는 슬롯들

    private GameObject go_Preview; // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수 


    public Vector3 Pointer;
    //public Vector3 pointer { get { return Pointer; } }

    private RaycastHit hitInfo;

    [SerializeField] LayerMask GroundLayer;

    [SerializeField] LayerMask IgnoreLayer;

    [SerializeField] float range;



    private void PointerPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //카메라에서 레이 쏘기
        if (Physics.Raycast(ray, out hitInfo, range, IgnoreLayer | GroundLayer)) //레이캐스트로 그라운드 체크(range로 범위 조정 가능)                      
        {
            Pointer = hitInfo.point;
            Pointer.y = hitInfo.transform.position.y + hitInfo.transform.up.y;
        }
    }


    public void SlotClick(int _slotNumber) //슬럿 클릭시 프리뷰 프리펩 생성
    {
        go_Preview = Instantiate(craft_fire[_slotNumber].go_PreviewPrefab, Pointer, Quaternion.identity);
        go_Prefab = craft_fire[_slotNumber].go_prefab;
        isPreviewActivated = true;
        go_BaseUI.SetActive(false);
    }

    void Update()
    {
        PointerPosition();

        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated) //인벤토리 창 열기
            Window();

        if (isPreviewActivated) // 프리뷰 상태 중 프리뷰 프리펩 계속 업데이트
            PreviewPositionUpdate();

        if (Input.GetButtonDown("Fire1")) // 짓기
            Build();

        if (Input.GetKeyDown(KeyCode.Escape)) // 짓는 중에 캔슬
            Cancel();

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

        go_Preview.transform.position = Pointer;

    }


    private void Build() // 짓기
    {
        if (isPreviewActivated && go_Preview.GetComponent<PreviewObject>().isBuildable())
        {
            photonView.RPC("CreateCraftObject", RpcTarget.MasterClient, go_Prefab.name, Pointer, go_Preview.transform.rotation); // OthersBuffered를 사용하면 나중에 서버에 들어온 사람도 서버에 쌓인 데이터를 받을 수 있다
            Instantiate(go_Prefab, Pointer, go_Preview.transform.rotation);
            Destroy(go_Preview);
            isActivated = false;
            isPreviewActivated = false;
            go_Preview = null;
            go_Prefab = null;
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
        go_BaseUI.SetActive(true);
    }

    private void CloseWindow()
    {
        isActivated = false;
        go_BaseUI.SetActive(false);
    }

    private void Cancel()
    {
        if (isPreviewActivated)
            Destroy(go_Preview);

        isActivated = false;
        isPreviewActivated = false;

        go_Preview = null;
        go_Prefab = null;

        go_BaseUI.SetActive(false);
    }
}