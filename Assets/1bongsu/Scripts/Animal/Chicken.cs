using Photon.Pun;
using System.Collections;
using UnityEngine;

public class Chicken : Animal
{
    private void Start()
    {
        stateMachine.AddState(State.Idle, new C_IdleState(this));
        stateMachine.AddState(State.Roam, new C_RoamState(this));
        stateMachine.AddState(State.Run, new C_RunState(this));

        if (photonView.IsMine)
            stateMachine.Start(State.Idle);

        StartCoroutine(WaitMasterChangeRoutine());
    }

    // 마스터가 바뀌면 닭을 다시 동작하게 해준다, 마스터 클라에서만 스테이트머신이 돌아가고 있는 상태라서 해주는 작업
    IEnumerator WaitMasterChangeRoutine()
    {
        yield return new WaitUntil(() => PhotonNetwork.IsMasterClient);
        stateMachine.Start(State.Idle);
        isMaster = true;
    }
}
