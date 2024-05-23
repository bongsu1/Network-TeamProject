using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Events;

public class Animal : MonoBehaviourPun, IPunObservable, IDamageble
{
    public enum State { Idle, Roam, Run }
    protected StateMachine<State> stateMachine = new StateMachine<State>();

    [SerializeField] protected State curState; // 디버그 확인용
    public State CurState { set { curState = value; } }

    [Header("Component")]
    [SerializeField] protected NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }

    [SerializeField] protected Animator animator;
    public Animator Animator { get { return animator; } }

    [Header("Player Search")]
    [SerializeField] LayerMask targetLayer;
    [SerializeField] float searchRadius;
    [SerializeField] float searchAngle;
    [SerializeField] float onHitSearchRadius; // 맞았을때는 좀 더 넓은 범위를 탐색

    [Header("Status")]
    [SerializeField] protected int maxHP;
    [SerializeField] protected int curHP;

    private float cosRange;

    private Transform target;
    public Transform Target { get { return target; } set { target = value; } }

    protected bool isMaster; // stateMachine.CurState에 할당해주는 속도가 조금 늦어서 추가하는 조건

    public UnityEvent OnDie;

    protected virtual void Awake()
    {
        cosRange = Mathf.Cos(Mathf.Deg2Rad * searchAngle);
        curHP = maxHP;
    }

    protected virtual void Update()
    {
        if (!photonView.IsMine || !isMaster)
            return;

        stateMachine.Update();
    }

    protected virtual void FixedUpdate()
    {
        if (!photonView.IsMine || !isMaster)
            return;

        stateMachine.FixedUpdate();
    }

    protected virtual void LateUpdate()
    {
        if (!photonView.IsMine || !isMaster)
            return;

        stateMachine.LateUpdate();
    }

    private Collider[] colliders = new Collider[10];
    private void Search(float searchRadius, bool isHit = false)
    {
        int size = Physics.OverlapSphereNonAlloc(transform.position, searchRadius, colliders, targetLayer);
        if (size > 0)
        {
            foreach (Collider collider in colliders)
            {
                if (collider == null)
                    continue;

                PlayerController target = collider.GetComponent<PlayerController>();
                if (target == null)
                    continue;

                // 맞았을때는 시야와 상관없이 전범위 탐색
                if (!isHit)
                {
                    Vector3 toTargetDir = (target.transform.position - transform.position).normalized;
                    if (Vector3.Dot(toTargetDir, transform.forward) < cosRange)
                        continue;
                }

                this.target = target.transform;
                break;
            }
        }
    }

    public IEnumerator SearchRoutine()
    {
        while (true)
        {
            Search(searchRadius); // 기본 범위만 탐색
            yield return new WaitForSeconds(.5f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curState);
            stream.SendNext(curHP);
        }
        else
        {
            curState = (State)(int)stream.ReceiveNext();
            curHP = (int)stream.ReceiveNext();
        }
    }

    private bool onHit;
    public void Damaged(int Damage)
    {
        if (onHit)
            return;

        curHP -= Damage;
        onHit = true;

        if (curHP <= 0)
        {
            curHP = 0;
            Die();
        }
        else
        {
            Search(onHitSearchRadius, true);
            StartCoroutine(HitRoutine());
        }
    }
    IEnumerator HitRoutine()
    {
        yield return new WaitForSeconds(0.1f);
        onHit = false;
    }

    [ContextMenu("Die")]
    private void Die()
    {
        Destroy(gameObject);
        // 추가적인 것
        // 아이템 드랍
        OnDie?.Invoke();
        OnDie.RemoveAllListeners();
    }

    // test..
    public bool isRun;
    private void OnDrawGizmosSelected()
    {
        if (isRun)
            return;

        float search = onHit ? onHitSearchRadius : searchRadius;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, search);

        Gizmos.color = Color.green;
        Vector3 angleDir1 = transform.TransformDirection(new Vector3(Mathf.Sin(Mathf.Deg2Rad * searchAngle),
            0, Mathf.Cos(Mathf.Deg2Rad * searchAngle)));
        Vector3 angleDir2 = transform.TransformDirection(new Vector3(Mathf.Sin(Mathf.Deg2Rad * -searchAngle),
            0, Mathf.Cos(Mathf.Deg2Rad * -searchAngle)));
        Vector3 angleDir3 = transform.TransformDirection(new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * -searchAngle),
             Mathf.Cos(Mathf.Deg2Rad * -searchAngle)));
        Vector3 angleDir4 = transform.TransformDirection(new Vector3(0, Mathf.Sin(Mathf.Deg2Rad * searchAngle),
             Mathf.Cos(Mathf.Deg2Rad * searchAngle)));

        Gizmos.DrawRay(transform.position, angleDir1 * searchRadius);
        Gizmos.DrawRay(transform.position, angleDir2 * searchRadius);
        Gizmos.DrawRay(transform.position, angleDir3 * searchRadius);
        Gizmos.DrawRay(transform.position, angleDir4 * searchRadius);
    }
}
