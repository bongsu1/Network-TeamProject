using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image healthBar;

    public void UpdateHealthBar(int curHp)
    {
        // fillAmount기능을 사용하려면 Image type을 filled로 변경
        healthBar.fillAmount = curHp / 90f; // 체력 최대값이 90이라서 90으로 나눠줌
    }
}