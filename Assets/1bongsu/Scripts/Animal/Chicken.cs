using System.Collections;
using UnityEngine;

public class Chicken : Animal
{
    private IEnumerator Start()
    {
        /*stateMachine.AddState(State.Idle, new C_IdleState(this));
        stateMachine.AddState(State.Roam, new C_RoamState(this));
        stateMachine.AddState(State.Run, new C_RunState(this));

        stateMachine.Start(State.Idle);*/

        yield return new WaitForSeconds(5f);
        stateMachine.ChangeState(State.Roam);
        yield return new WaitForSeconds(5f);
        stateMachine.ChangeState(State.Run);
    }

    private void OnEnable()
    {
        stateMachine.AddState(State.Idle, new C_IdleState(this));
        stateMachine.AddState(State.Roam, new C_RoamState(this));
        stateMachine.AddState(State.Run, new C_RunState(this));

        stateMachine.Start(State.Idle);
    }
}
