using Photon.Pun;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class PlayerDataController : MonoBehaviourPun
{
    // 체력 겸 스태미나 역할
    // 공격당했을 때 소모 // 시간이 흐르거나 행동을 할 때 소모 // 모닥불 근처에 있으면 회복
    [Header("Health")]
    [SerializeField] HealthUI healthUI;
    [Header("Player")]
    [SerializeField] PlayerController playerController; // 플레이어가 걷는지 확인용

    public UnityEvent<int> OnChangeHealth; // 체력이 바뀔때 보내려는 이벤트

    private void OnEnable()
    {
        // test..
        if (DebugDataManager.Instance != null)
        {
            // 내것이 아니면 UI도 생성하지 않고 삭제
            if (photonView.IsMine)
            {
                healthUI = Instantiate(healthUI);
                OnChangeHealth.AddListener(healthUI.UpdateHealthBar); // 생성하자 마자 이벤트에 추가
            }
            else
                Destroy(this);
        }
        else
        {
            Destroy(this);
        }
    }

    private void Update()
    {
        PositionUpdate(transform.position);
    }

    // 체력 지속 소모
    private void HealthConsumption() // TODO.. 로직을변경해야함 움직일때만 체력소모가 되도록
    {
        // 체력이 1칸 남았을때는 지속 소모 없음
        if (DebugDataManager.Instance.RoomData.health < 10)
            return;

        // test.. 2씩 소모
        DebugDataManager.Instance.RoomData.health -= 2;
        OnChangeHealth.Invoke(DebugDataManager.Instance.RoomData.health);
    }
    #region HealthConsumptionRoutine
    Coroutine healthConsumptionRoutine;
    IEnumerator HealthConsumptionRoutine()
    {
        while (true)
        {
            HealthConsumption();
            yield return new WaitForSeconds(5f);
        }
    }
    public void StartHealthConsumptionRoutine()
    {
        healthConsumptionRoutine = StartCoroutine(HealthConsumptionRoutine());
    }
    public void StopHealthConsumptionRoutine()
    {
        StopCoroutine(healthConsumptionRoutine);
    }
    #endregion

    // 체력 회복
    public void RecoveryHealth(int recovery)
    {
        DebugDataManager.Instance.RoomData.health += recovery;

        // 최대 체력을 넘으면 최대 체력으로
        if (DebugDataManager.Instance.RoomData.health > 90)
            DebugDataManager.Instance.RoomData.health = 90;

        OnChangeHealth.Invoke(DebugDataManager.Instance.RoomData.health);
    }

    // 데미지 받는 함수
    public void TakeDamage(int damage)
    {
        DebugDataManager.Instance.RoomData.health -= damage;

        // 체력이 0보다 작아지면 체력을 0으로
        if (DebugDataManager.Instance.RoomData.health < 0)
            DebugDataManager.Instance.RoomData.health = 0;

        OnChangeHealth.Invoke(DebugDataManager.Instance.RoomData.health);
    }

    // 플레이어 위치 데이터 저장
    private void PositionUpdate(Vector3 position)
    {
        DebugDataManager.Instance.RoomData.position = position;
    }

    // test.. 
    [ContextMenu("Test RecoveryHealth 50")]
    private void TestHeal()
    {
        RecoveryHealth(50);
    }
    [ContextMenu("Test TakeDamage 5")]
    private void TestDamage()
    {
        TakeDamage(5);
    }
}
