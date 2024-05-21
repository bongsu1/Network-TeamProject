using System.Collections;
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
        owner.CurState = Animal.State.Idle;

        int rand = Random.Range(0, 2);
        owner.Animator.SetInteger("RandomPattern", rand);
        owner.StartCoroutine(IdleRoutine());
    }

    IEnumerator IdleRoutine()
    {
        // 지금 재생하는 애니메이션의 이름이 "Idle"이면
        yield return new WaitUntil(() => owner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        yield return new WaitForSeconds(2f);
        ChangeState(Animal.State.Roam);
    }

    public C_IdleState(Animal owner)
    {
        this.owner = owner;
    }
}

public class C_RoamState : AnimalState
{
    private Vector3 nextPoint;
    private float time;

    public override void Enter()
    {
        owner.CurState = Animal.State.Roam;

        time = Time.time;

        int count = 0;
        do
        {
            nextPoint = owner.transform.position +
                new Vector3(Random.Range(-3f, 3f), owner.transform.position.y, Random.Range(-3f, 3f));

            if (count++ == 2) // 두번 찾아도 안나오면 제자리
                nextPoint = owner.transform.position;
        } while (!owner.Agent.CalculatePath(nextPoint, owner.Agent.path)); // 다음 포인트가 갈 수 있는 곳 일때까지 반복

        owner.Animator.SetBool("IsRoaming", true);
        SetDestination(nextPoint);
    }

    public override void Update()
    {
        if (Time.time - time > 7f)
        {
            SetDestination(owner.transform.position); // 가끔 네비 경계면에 지정을 해줘서 목적지에 도달하지 못한다
        }
    }

    public override void Exit()
    {
        owner.Animator.SetBool("IsRoaming", false);
    }

    public override void Transition()
    {
        if ((nextPoint - owner.transform.position).sqrMagnitude < 0.01f)
        {
            ChangeState(Animal.State.Idle);
        }
    }

    public C_RoamState(Animal owner)
    {
        this.owner = owner;
    }
}

public class C_RunState : AnimalState
{
    public C_RunState(Animal owner)
    {
        this.owner = owner;
    }
}
#endregion