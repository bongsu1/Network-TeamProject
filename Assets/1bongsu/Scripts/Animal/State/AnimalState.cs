using System.Collections;
using UnityEngine;

public class AnimalState : BaseState<Animal.State>
{
    protected Animal owner;

    private Coroutine searchRoutine;

    protected void SetDestination(Vector3 nextPoint) // 다음 목적지를 정해주기
    {
        owner.Agent.SetDestination(nextPoint);
    }

    protected void StartSearchRoutine()
    {
        if (searchRoutine != null)
            return;

        searchRoutine = owner.StartCoroutine(owner.SearchRoutine()); // 탐색루틴 실행
    }
    protected void StopSearchRoutine()
    {
        if (searchRoutine == null)
            return;

        owner.StopCoroutine(searchRoutine);
        searchRoutine = null;
    }
}

#region Chicken
public class C_IdleState : AnimalState
{
    Coroutine idleRoutine;

    public override void Enter()
    {
        owner.CurState = Animal.State.Idle;

        int rand = Random.Range(0, 2);
        owner.Animator.SetInteger("RandomPattern", rand); // 대기동작 2가지중 한가지 실행
        idleRoutine = owner.StartCoroutine(IdleRoutine());

        StartSearchRoutine();
    }

    IEnumerator IdleRoutine()
    {
        // 지금 재생하는 애니메이션의 이름이 "Idle"이면
        yield return new WaitUntil(() => owner.Animator.GetCurrentAnimatorStateInfo(0).IsName("Idle"));
        yield return new WaitForSeconds(3f);
        ChangeState(Animal.State.Roam);
    }

    public override void Exit()
    {
        if (idleRoutine == null)
            return;

        owner.StopCoroutine(idleRoutine); // 플레이어를 찾아서 RunState로 바뀌면 아이들 루틴 종료
    }

    public override void Transition()
    {
        if (owner.Target != null) // target이 지정되면 RunState로 상태변경
        {
            ChangeState(Animal.State.Run);
            StopSearchRoutine();
        }
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
                new Vector3(Random.Range(-5f, 5f), owner.transform.position.y, Random.Range(-5f, 5f));

            if (count++ == 2) // 두번 찾아도 안나오면 제자리
                nextPoint = owner.transform.position;
        } while (!owner.Agent.CalculatePath(nextPoint, owner.Agent.path)); // 다음 포인트가 갈 수 있는 곳 일때까지 반복

        owner.Animator.SetBool("IsRoaming", true);
        SetDestination(nextPoint);
    }

    public override void Exit()
    {
        owner.Animator.SetBool("IsRoaming", false);
    }

    public override void Transition()
    {
        if (owner.Target != null)
        {
            ChangeState(Animal.State.Run);
            StopSearchRoutine();
        }
        // 가끔 네비 경계면에 지정을 해줘서 목적지에 도달하지 못한다, 그래서 일정 시간이 지나면 상태변경
        else if (Time.time - time > 7f)
        {
            ChangeState(Animal.State.Idle);
            SetDestination(owner.transform.position);
        }
        else if ((nextPoint - owner.transform.position).sqrMagnitude < 0.01f)
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
    private float time;
    private float originSpd;

    public override void Enter()
    {
        owner.CurState = Animal.State.Run;
        owner.Animator.SetInteger("RandomPattern", 2); // 다른 애니메이션으로 넘어가지 않도록
        owner.Animator.SetBool("IsRun", true);

        owner.isRun = true; // test
        time = Time.time;

        Vector3 toRunDir = (owner.transform.position - owner.Target.position); // 타겟으로 부터 반대방향으로 도망
        toRunDir = new Vector3(toRunDir.x, 0, toRunDir.z).normalized;
        SetDestination(owner.transform.position + toRunDir * 10f);
        originSpd = owner.Agent.speed;
        owner.Agent.speed = 4f;
    }

    public override void Transition()
    {
        if (Time.time - time > 3f)
        {
            ChangeState(Animal.State.Idle);
            SetDestination(owner.transform.position);
        }
        else if ((owner.Agent.destination - owner.transform.position).sqrMagnitude < 0.01f)
        {
            ChangeState(Animal.State.Idle);
        }
    }

    public override void Exit()
    {
        owner.isRun = false; // test

        owner.Target = null;
        owner.Agent.speed = originSpd;
        owner.Animator.SetBool("IsRun", false);
    }

    public C_RunState(Animal owner)
    {
        this.owner = owner;
    }
}
#endregion