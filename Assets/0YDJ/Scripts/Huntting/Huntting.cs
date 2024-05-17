using Photon.Pun;
using UnityEngine;


public class Huntting : MonoBehaviourPun
{

    [SerializeField] Animator animator;
    [SerializeField] GameObject muzzlePoint;
    private ObjectPool pool;

    [SerializeField] float fireCoolTime;
    [SerializeField] float range = 0f;
    [SerializeField] float spinSpeed;
    [SerializeField] LayerMask layerMask;
    [SerializeField] Bullet bulletPrefab;
    private float lastFireTime = float.MinValue;
    private bool isSetReady;
    Transform Target = null;


    private void Start()
    {
        pool = ObjectPool.instance;
        InvokeRepeating("SearchEnemy", 0f, 0.5f); //SetReady 함수 초마다 호출시켜주기
    }

    private void Update()
    {
        if (photonView.IsMine) // 조준
            SetReady();

        if (isSetReady && Input.GetButton("Fire1")) // 발사
            Fire();
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    private void SetReady()// 조준
    {
        isSetReady = Input.GetButton("Fire2");

        photonView.RPC("ChangeSetReadyAnimation", RpcTarget.All, isSetReady); // 애니메이션 작동

        if (isSetReady && Target != null)
        {
            Debug.Log(Target.name);
            Vector3 dir = Target.position - transform.position; //타겟과 터렛의 위치를 뺀 값
            Quaternion lookRotation = Quaternion.LookRotation(dir);
            Vector3 euler = Quaternion.RotateTowards(transform.rotation, lookRotation, spinSpeed * Time.deltaTime).eulerAngles;
            transform.rotation = Quaternion.Euler(0, euler.y, 0);
        }
    }


    [PunRPC]
    private void ChangeSetReadyAnimation(bool isSetReady, PhotonMessageInfo info)
    {
        animator.SetBool("IsSetReady", isSetReady);
    }


    void SearchEnemy()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, range, layerMask);
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
        if(Target != null && Input.GetButton("Fire2"))
        {
            return true;
        }
        return false;
    }



    private void Fire() // 발사
    {
        photonView.RPC("CreateBullet", RpcTarget.All, muzzlePoint.transform.position, transform.rotation);
    }

    [PunRPC]
    private void CreateBullet(Vector3 position, Quaternion rotation, PhotonMessageInfo info)
    {
        if (Time.time < lastFireTime + fireCoolTime) //쿨타임
            return;
        lastFireTime = Time.time;

        float lag = Mathf.Abs((float)(PhotonNetwork.Time - info.SentServerTime)); //서버 지연시간 보정

        Quaternion gunShake = transform.rotation;
        //gunShake.y = transform.rotation.y + Random.RandomRange(-0.03f, 0.03f);
        Bullet bullet = Instantiate(bulletPrefab, position, rotation);

        //pool.GetPool(muzzlePoint.transform.position, transform.rotation);
        Debug.Log($"총알 속도 : {bullet.Velocity}");
        bullet.AwakeN(position, position + bullet.Velocity * lag);  
    }

}



//**************************************************************************


//인풋시스템은 멀티접속 시 안먹음
//private void OnSetReady(InputValue value)
//{
//    Debug.Log("OnSetReady");
//    isSetReady = value.isPressed;
//    photonView.RPC("ChangeSetReadyAnimation", RpcTarget.All, isSetReady); // 애니메이션 작동
//}