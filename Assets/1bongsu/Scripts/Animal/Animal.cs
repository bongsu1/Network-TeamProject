using UnityEngine;
using UnityEngine.AI;

public class Animal : MonoBehaviour
{
    public enum State { Idle, Roam, Run }
    protected StateMachine<State> stateMachine = new StateMachine<State>();

    [SerializeField] protected Rigidbody rigid;
    public Rigidbody Rigid { get { return rigid; } }

    [SerializeField] protected NavMeshAgent agent;
    public NavMeshAgent Agent { get { return agent; } }

    [SerializeField] protected Animator animator;
    public Animator Animator { get { return animator; } }

    protected virtual void Update()
    {
        stateMachine.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine.FixedUpdate();
    }

    protected virtual void LateUpdate()
    {
        stateMachine.LateUpdate();
    }
}
