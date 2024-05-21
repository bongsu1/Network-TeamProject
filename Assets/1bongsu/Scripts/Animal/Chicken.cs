public class Chicken : Animal
{
    private void Start()
    {
        stateMachine.AddState(State.Idle, new C_IdleState(this));
        stateMachine.AddState(State.Roam, new C_RoamState(this));
        stateMachine.AddState(State.Run, new C_RunState(this));

        stateMachine.Start(State.Idle);
    }
}
