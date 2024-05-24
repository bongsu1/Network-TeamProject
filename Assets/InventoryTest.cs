using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventoryTest : MonoBehaviourPun
{

    //public static CraftManual instance;

    private bool isActivated = false;  // CraftManual UI 활성 상태
    private bool isPreviewActivated = false; // 미리 보기 활성화 상태

    [SerializeField] PopUpUI InvenView; // 기본 베이스 UI

    private GameObject go_Preview; // 미리 보기 프리팹을 담을 변수
    private GameObject go_Prefab; // 실제 생성될 프리팹을 담을 변수 

    //dk
    public Vector3 Pointer;
    //public Vector3 pointer { get { return Pointer; } }

    private RaycastHit hitInfo;

    [SerializeField] LayerMask GroundLayer;

    [SerializeField] LayerMask IgnoreLayer;

    [SerializeField] float range;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !isPreviewActivated) //인벤토리 창 열기
            Window();

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

    private void PointerPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition); //카메라에서 레이 쏘기
        if (Physics.Raycast(ray, out hitInfo, range, IgnoreLayer | GroundLayer)) //레이캐스트로 그라운드 체크(range로 범위 조정 가능)                      
        {
            Pointer = hitInfo.point;

            if (hitInfo.transform.gameObject.layer == 31) // 바닥에 둘 때 오차 보정
            {
                Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y - 0.5f;
                return;
            }

            if (go_Preview.transform.localScale.y > 1) // 두 칸 이상 전용
            {
                if (hitInfo.transform.localScale.y > 1) // 두 칸 이상이 두 칸 이상 위에 쌓을 때
                {
                    Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y;
                    return;
                }
                Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y - (go_Preview.transform.localScale.y * 0.5f - 0.5f);  // 두 칸이 땅에 닿을 때 -0.5f
                return;
            }
            Pointer.y = hitInfo.transform.position.y + go_Preview.transform.localScale.y + (hitInfo.transform.localScale.y * 0.5f - 0.5f);
        }
    }
    public void SlotClick() //슬럿 클릭시 프리뷰 프리펩 생성
    {
        Debug.Log("DLDDD");
        GameObject go_preview = Manager.Build.go_preview;
        GameObject go_prefab = Manager.Build.go_prefab;
        go_Preview = Manager.Build.go_preview;
        go_Prefab = Manager.Build.go_prefab;
        Debug.Log($"00. {go_prefab}");
        Debug.Log($"01. {go_preview}");
        isPreviewActivated = Manager.Build.isPreviewActivated;
        go_Preview = Instantiate(go_Preview, Pointer, Quaternion.Euler(0, 0, 0));
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
        Debug.Log(go_Preview.name);
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
        Manager.UI.ShowPopUpUI(InvenView);
        //Manager.UI.ShowPopUpUI(invenUI);
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

        Manager.UI.ClosePopUpUI();
    }
}
