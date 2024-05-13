using Photon.Pun;
using UnityEngine;

public class PlayerHealth : MonoBehaviourPun
{
    // 체력 겸 스태미나 역할
    // 공격당했을 때 소모 // 시간이 흐르거나 행동을 할 때 소모 // 모닥불 근처에 있으면 회복

    [SerializeField] HealthUI healthUI;

    private void OnEnable()
    {
        // 내것이 아니면 UI도 생성하지 않고 삭제
        if (photonView.IsMine)
            healthUI = Instantiate(healthUI);
        else
            Destroy(this);
    }

    // test..
    [SerializeField] bool isUseHealth;
    private void Update()
    {
        if (isUseHealth)
            UseHealth();
    }

    // 체력 지속 소모
    public void UseHealth()
    {
        // 체력이 1칸 남았을때는 지속 소모 없음
        if (DebugDataManager.Instance.UserData.health < 10)
            return;

        // test.. 총체력이 90 체력 전체 소모하는데 걸리는 시간 180초
        DebugDataManager.Instance.UserData.health -= Time.deltaTime * 0.5f;
    }

    // 체력 회복
    public void RecoveryHealth(float recovery)
    {
        DebugDataManager.Instance.UserData.health += recovery;

        // 최대 체력을 넘으면 최대 체력으로
        if (DebugDataManager.Instance.UserData.health > 90)
            DebugDataManager.Instance.UserData.health = 90;
    }

    public void TakeDamage(int damage)
    {
        DebugDataManager.Instance.UserData.health -= damage;
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
