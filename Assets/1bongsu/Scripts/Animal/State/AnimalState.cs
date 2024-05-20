using UnityEngine;

public class AnimalState : BaseState<Animal.State>
{
    protected Animal owner;

    protected void SetDestination(Vector3 nextPoint) // 다음 목적지를 정해주기
    {
        owner.Agent.SetDestination(nextPoint);
    }
}

#region Chicken
public class C_IdleState : AnimalState
{
    public override void Enter()
    {
        owner.Animator.Play("Idle");
    }

    public C_IdleState(Animal owner)
    {
        this.owner = owner;
    }
}

public class C_RoamState : AnimalState
{
    public override void Enter()
    {
        owner.Animator.Play("Walk");

        Vector3 nextPoint = new Vector3(Random.Range(-3f, 3f), owner.transform.position.y, Random.Range(-3f, 3f));
        SetDestination(nextPoint);
    }

    public C_RoamState(Animal owner)
    {
        this.owner = owner;
    }
}

public class C_RunState : AnimalState
{
    public override void Enter()
    {
        owner.Animator.Play("Run");
    }

    public C_RunState(Animal owner)
    {
        this.owner = owner;
    }
}
#endregion