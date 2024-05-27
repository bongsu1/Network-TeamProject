using Photon.Pun;
using UnityEngine;


public enum WeaponType
{
    None,
    Gun,
    Ax,
    Fishing,
}

public class Action : MonoBehaviourPun //총쏘기, 벌목
{
    [Header("Component")]
    [SerializeField] Animator animator;
    [SerializeField] Transform firePoint;
    [SerializeField] Bullet bulletPrefab;
    [SerializeField] Collider axCollider;

    [Header("Property")]
    [SerializeField] float fireCoolTime;
    [SerializeField] float gunRange;
    [SerializeField] float spinSpeed;
    [SerializeField] WeaponType holdingItem;
    public WeaponType HoldingItem { set { holdingItem = value; } }
    [SerializeField] InventoryObject Eqiupment;

    [Header("holdObject")]
    [SerializeField] GameObject gun;
    [SerializeField] GameObject ax;
    private GameObject holdObject = null;
    private float lastFireTime = float.MinValue;
    private bool isSetReady;
    private bool axFire;
    private Transform Target = null;

    private ObjectPool pool;

    [Header("TargetLayerMask")]
    [SerializeField] LayerMask TargetMask;

    [Header("Sound")]
    [SerializeField] AudioSource ShottingSound;
    //[SerializeField] AudioSource ReadySound;


    private void Start()
    {
        pool = ObjectPool.instance;
        //InvokeRepeating("SearchEnemy", 0f, 0.5f); //SetReady 함수 초마다 호출시켜주기
    }

    private void Update()
    {
        isSetReady = Input.GetButton("Fire2");

        if (photonView.IsMine) // 조준
            SetItem();

        //Debug.Log(Eqiupment.Container.Items[3].item.weaponType);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, gunRange);
    }

    private void SetItem() // 들고 있는 아이템에 따라 맞는 함수 호출
    {
        switch (Eqiupment.Container.Items[3].item.weaponType)
        {
            case WeaponType.Gun: //총
                SetGun();
                if (isSetReady && Input.GetButton("Fire1")) // 총 발사
                    GunFire();
                break;

            case WeaponType.Ax://도끼
                SetAx();
                break;

            case WeaponType.Fishing:
                break;

            case WeaponType.None: // 나중에 장비 풀기 추가
                break;
        }
    }


    // 총 구역 *************************************************************************************************************************************************************************************
    private void SetGun() // 총 조준
    {
        photonView.RPC("SetAnimationParameter", RpcTarget.All, Parameter.SetBool, "GunIsSetReady", isSetReady);

        photonView.RPC("WeaponSetActive", RpcTarget.All, WeaponType.Gun, isSetReady);

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
    private void WeaponSetActive(WeaponType type, bool isSetReady)
    {
        switch (type)
        {
            case WeaponType.None:
                break;
            case WeaponType.Gun:
                gun.SetActive(isSetReady);
                break;
            case WeaponType.Ax:
                ax.SetActive(isSetReady);
                break;
            case WeaponType.Fishing:
                break;
        }


    }

    private void SearchEnemy() // 제일 가까운 타켓 찾기
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, gunRange, TargetMask);
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


        //pool.GetPool(position, rotation);
        //PooledObject pooledObject = pool.GetPool(position, rotation);
        //pooledObject.GetComponent<Bullet>().transform.position += pooledObject.GetComponent<Bullet>().Velocity * lag;

        //pooledObject.gameObject.transform.position += pooledObject.Velocity * lag;

        ShottingSound.Play();
    }



    // 도끼 구역 *************************************************************************************************************************************************************************************
    private void SetAx() //도끼 조준
    {
        Debug.Log("SetAx");
        photonView.RPC("SetAnimationParameter", RpcTarget.All, Parameter.SetBool, "AxIsSetReady", isSetReady);

        //ax.SetActive(isSetReady);
        photonView.RPC("WeaponSetActive", RpcTarget.All, WeaponType.Ax, isSetReady);

        Target = null;

        AxFire();
    }


    private void AxFire() // 도끼 스윙
    {
        axFire = (isSetReady && Input.GetButton("Fire1"));

        photonView.RPC("SetAnimationParameter", RpcTarget.All, Parameter.SetBool, "AxSwing", axFire);
    }

    public void EnableWeapon()
    {
        axCollider.enabled = true;
    }

    public void DisableWeapon()
    {
        axCollider.enabled = false;
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