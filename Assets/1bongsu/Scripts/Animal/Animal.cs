using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviourPun, IPunObservable
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

    private float cosRange;

    private Transform target;
    public Transform Target { get { return target; } set { target = value; } }

    protected bool isMaster; // stateMachine.CurState에 할당해주는 속도가 조금 늦어서 추가하는 조건

    protected virtual void Awake()
    {
        cosRange = Mathf.Cos(Mathf.Deg2Rad * searchAngle);
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
    private void Search()
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

                Vector3 toTargetDir = (target.transform.position - transform.position).normalized;
                if (Vector3.Dot(toTargetDir, transform.forward) < cosRange)
                    continue;

                this.target = target.transform;
                break;
            }
        }
    }

    public IEnumerator SearchRoutine()
    {
        while (true)
        {
            Search();
            yield return new WaitForSeconds(1f);
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(curState);
        }
        else
        {
            curState = (State)(int)stream.ReceiveNext();
        }
    }

    // test..
    public bool isRun;
    private void OnDrawGizmosSelected()
    {
        if (isRun)
            return;

        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, searchRadius);

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
