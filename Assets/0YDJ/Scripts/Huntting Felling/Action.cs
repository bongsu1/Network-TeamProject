using Photon.Pun;
using UnityEngine;


public class Action : MonoBehaviourPun //총쏘기, 벌목
{
    [Header("Component")]
    [SerializeField] Animator animator;
    [SerializeField] Transform firePoint;
    [SerializeField] Bullet bulletPrefab;

    [Header("Property")]
    [SerializeField] float fireCoolTime;
    [SerializeField] float gunRange;
    [SerializeField] float spinSpeed;
    [SerializeField] LayerMask layerMask;
    [SerializeField] int holdingItem; // 1은 총, 2는 도끼

    [Header("holdObject")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject ax;
    private GameObject holdObject = null;
    private float lastFireTime = float.MinValue;
    private bool isSetReady;
    private Transform Target = null;

    private ObjectPool pool;




    private void Start()
    {
        pool = ObjectPool.instance;
        //InvokeRepeating("SearchEnemy", 0f, 0.5f); //SetReady 함수 초마다 호출시켜주기
    }

    private void Update()
    {
        if (photonView.IsMine) // 조준
            SetItem();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gunRange);
    }

    private void SetItem() // 들고 있는 아이템에 따라 맞는 함수 호출
    {
        switch (holdingItem)
        {
            case 1:
                SetGun();
                if (isSetReady && Input.GetButton("Fire1")) // 총 발사
                    GunFire();
                break;

            case 2:
                SetAx();
                if (isSetReady && Input.GetButtonDown("Fire1")) // 도끼 스윙
                    AxFire();
                break;
        }
    }


    // 총 구역 *************************************************************************************************************************************************************************************
    private void SetGun()
    {
        Debug.Log("총 조준");
        isSetReady = Input.GetButton("Fire2");

        photonView.RPC("GunChangeSetReadyAnimation", RpcTarget.All, isSetReady); // 애니메이션 작동

        gun.SetActive(isSetReady);

        if (isSetReady)
        {
            SearchEnemy();
        }

        if (TargetIn()) // 타켓 방향으로 몸 돌리기
        {
            Vector3 dir = Target.position - transform.position; //타겟과 터렛의 위치를 뺀 값
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 euler = Quaternion.RotateTowards(transform.rotation, lookRotation, spinSpeed * Time.deltaTime).eulerAngles;
            transform.rotation = Quaternion.Euler(0, euler.y, 0);
        }
    }

    [PunRPC]
    private void GunChangeSetReadyAnimation(bool isSetReady, PhotonMessageInfo info)
    {
        animator.SetBool("GunIsSetReady", isSetReady);
    }


    private void SearchEnemy()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, gunRange, layerMask);
        Transform shortestTarget = null;

        if (targets.Length > 0)
        {
            float shortestDistance = Mathf.Infinity;
            foreach (Collider target in targets)
            {
                float distance = Vector3.SqrMagnitude(transform.position - target.transform.position);
                if (shortestDistance > distance)
                {
                    shortestDistance = distance;
                    shortestTarget = target.transform;
                }
            }
        }

        Target = shortestTarget;
    }

    public bool TargetIn() //PlayerController에게 전달할 함수 (타켓 있을 때 Turn 방지)
    {
        if (Target != null && Input.GetButton("Fire2"))
        {
            return true;
        }
        return false;
    }



    private void GunFire() // 발사
    {
        // 마스터클라이언트에게 쏜거 확인(나 쏜다 말함 -> 마스터에게 전달 -> 마스터가 모두에게 전달)
        photonView.RPC("GunFire2", RpcTarget.MasterClient, firePoint.transform.position, transform.rotation);
    }

    [PunRPC]
    private void GunFire2(Vector3 position, Quaternion rotation) // 발사
    {
        if (Time.time < lastFireTime + fireCoolTime) //쿨타임
            return;
        lastFireTime = Time.time;

        //서버에게 쿨타임 확인하고 보내기 (서버를 거쳐서 쿨타임 됐나 확인)
        photonView.RPC("CreateBullet", RpcTarget.AllViaServer, position, rotation);

    }

    [PunRPC]
    private void CreateBullet(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime)); //서버 지연시간 보정

        //Quaternion gunShake = transform.rotation;
        //gunShake.y = transform.rotation.y + Random.RandomRange(-0.03f, 0.03f);

        Bullet bullet = Instantiate(bulletPrefab, position, rotation);
        bullet.transform.position += bullet.Velocity * lag;

        //ObjectPool pool = pool.GetPool(position, rotation);
    }



    // 도끼 구역 *************************************************************************************************************************************************************************************
    private void SetAx()
    {
        Debug.Log("도끼 조준");
        isSetReady = Input.GetButton("Fire2");

        photonView.RPC("AxChangeSetReadyAnimation", RpcTarget.All, isSetReady); // 애니메이션 작동

        ax.SetActive(isSetReady);
    }

    [PunRPC]
    private void AxChangeSetReadyAnimation(bool isSetReady, PhotonMessageInfo info)
    {
        animator.SetBool("AxIsSetReady", isSetReady);
    }

    [PunRPC]
    private void AxFire() // 발사
    {
        animator.SetBool("AxIsSetReady", isSetReady);
    }

        //if (Time.time < lastFireTime + fireCoolTime) //쿨타임
        //    return;
        //lastFireTime = Time.time;

}












//**************************************************************************


//인풋시스템은 멀티접속 시 안먹음
//private void OnSetReady(InputValue value)
//{
//    Debug.Log("OnSetReady");
//    isSetReady = value.isPressed;
//    photonView.RPC("ChangeSetReadyAnimation", RpcTarget.All, isSetReady); // 애니메이션 작동
//}