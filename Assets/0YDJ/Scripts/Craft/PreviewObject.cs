using System.Collections.Generic;
using UnityEngine;

public class PreviewObject : MonoBehaviour
{
    private List<Collider> colliderList = new List<Collider>(); // 충돌한 오브젝트들 저장할 리스트

    private int layerGround = 31; // 지형 레이어 (무시하게 할것)
    //[SerializeField] int IGNORE_RAYCAST_LAYER = 30; // ignore_raycast (무시하게 할것)

    [SerializeField] Material blue;
    [SerializeField] Material red;

    [SerializeField] LayerMask IgnoreLayer;

    private Renderer newMaterials;


    private void Start()
    {
        newMaterials = gameObject.GetComponentInChildren<Renderer>();
    }

    void Update()
    {
        ChangeColor();
    }

    private void ChangeColor()
    {
        if (colliderList.Count > 0)
            SetColor(red);
        else
            SetColor(blue);
    }

    private void SetColor(Material mat)
    {
        newMaterials.material = mat;
    }


    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == 30) //모든 프리펩마다 콜리더 위쪽을 살짝 깍음 위쪽만 탐지 못하게 해서 이단쌓기 가능
        {
            if (!colliderList.Contains(other))
            {
                colliderList.Add(other);
            }
            return;
        }

        if (other.gameObject.layer != layerGround && other.gameObject.layer != 30) //그라운드 레이어 외에는 콜리더 리스트에 추가
        {

            if (!colliderList.Contains(other)) //이미 리스트에 똑같은 other물질이라면 더이상 추가 안하기
            {
                //Debug.Log("Obstacle 감지, 리스트 Add");
                colliderList.Add(other);
            }
        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer != layerGround)
        {
            //Debug.Log("Remove");
            colliderList.Remove(other);
        }
    }
    public bool isBuildable()
    {
        return colliderList.Count == 0;
    }
}




//********************************************** 다정이의 시행착오들 ***********************************************//
//if (other.gameObject.layer == 30) //박스 오버랩으로 콜리더와는 다른 범위 만들기 //콜리더를 쓰면 이단쌓기할때 탐지되서 못쌓음
//{
//    Collider[] colliders = Physics.OverlapBox(transform.position, Vel);
//    foreach (Collider collider in colliders)
//    {
//        if (collider.gameObject.layer == 30)
//        {
//            if (!colliderList.Contains(other))
//            {
//                //Debug.Log("오버랩에 닿아서 지을 수 없다");
//                colliderList.Add(other);
//            }
//            return;
//        }

//        //Debug.Log("아래 블럭이 있어서 지을 수 있다");
//        colliderList.Remove(other);

//    }
//}


//private void Start()
//{
//    newMaterials = gameObject.GetComponent<Renderer>();

//    Vel = transform.localScale; // 오버랩 크기 줄이기
//    Vel.y -= 0.9f;
//    Vel.x -= 0.5f;
//    Vel.z -= 0.3f;

//}






//private void OnTriggerEnter(Collider other)                   //**박스캐스트
//{
//    if (other.gameObject.layer == 30) //IGNORE_RAYCAST_LAYER를 쓰면 이상하게 오류남
//    {
//        //Debug.DrawRay(transform.position, Vector3.down,red.color, 100f);
//        if (Physics.BoxCast(transform.position, transform.lossyScale / 2, Vector3.down, out hitInfo, transform.rotation, 10f , IgnoreLayer)) // 레이캐스트를 밑으로 쏘아서 탑쌓기 기능 구현
//        {

//            Debug.Log("레이캐스트에 닿아서 리턴");
//            if(colliderList.Contains(hitInfo.collider))
//            {
//                colliderList.Remove(hitInfo.collider); 
//            }
//            return;
//        }
//        colliderList.Add(other);
//    }
//    else if (other.gameObject.layer != layerGround) //그라운드 레이어 외에는 콜리더 리스트에 추가
//    {
//        //Debug.Log("Add");
//        colliderList.Add(other);
//    }
//}

//private void OnTriggerEnter(Collider other)                //**포인터 수치로 판단하기
//{
//    if (other.gameObject.layer == 30) //IGNORE_RAYCAST_LAYER를 쓰면 이상하게 오류남
//    {
//        //Debug.Log($"Pointer.y : {CraftManual.instance.Pointer.y}");
//        if (CraftManual.instance.Pointer.y > 1)
//        {
//            Debug.Log("포인터 수치 올라가서 리턴");
//            return;
//        }
//        colliderList.Add(other);
//    }
//    else if (other.gameObject.layer != layerGround) //그라운드 레이어 외에는 콜리더 리스트에 추가
//    {
//        //Debug.Log("Add");
//        colliderList.Add(other);
//    }
//}



//private void OnTriggerEnter(Collider other)                //**오버랩
//{
//    if (other.gameObject.layer == 30) //IGNORE_RAYCAST_LAYER를 쓰면 이상하게 오류남
//    {
//        Collider[] colliders = Physics.OverlapBox(transform.position, new Vector3(0.5f, 5 , 0.5f));
//        foreach (Collider collider in colliders)
//        {
//            if (collider.gameObject.layer == LayerMask.NameToLayer("Ignore"))
//            {

//                Debug.Log("오버랩에 닿아서 리턴");
//                if (colliderList.Contains(other))
//                {
//                    colliderList.Remove(other);
//                }
//                return;
//            }
//        }
//        colliderList.Add(other);
//    }
//    else if (other.gameObject.layer != layerGround) //그라운드 레이어 외에는 콜리더 리스트에 추가
//    {
//        //Debug.Log("Add");
//        colliderList.Add(other);
//    }
//}

//void OnDrawGizmos() // 범위 그리기
//{
//    Gizmos.color = Color.red;
//    Gizmos.DrawWireCube(transform.position, new Vector3(0, -5, 0));

//}
//****************************************************************************************************//



