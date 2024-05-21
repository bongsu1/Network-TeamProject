using Photon.Pun;
using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviourPun, IPunObservable
{
    public enum State { Idle, Roam, Run }
    protected StateMachine<State> stateMachine = new StateMachine<State>();

    protected State curState;
    public State CurState { set { curState = value; } }

    [SerializeField] protected Rigidbody rigid;
    public Rigidbody Rigid { get { return rigid; } }

    [SerializeField] protected NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }

    [SerializeField] protected Animator animator;
    public Animator Animator { get { return animator; } }

    protected bool isMaster; // stateMachine.CurState에 할당해주는 속도가 조금 늦어서 추가하는 조건

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
}
