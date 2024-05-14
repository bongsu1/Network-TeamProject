using UnityEngine;
using UnityEngine.UI;

public class HealthUI : MonoBehaviour
{
    [SerializeField] Image healthBar;

    private void LateUpdate()
    {
        // fillAmount기능을 사용하려면 Image type을 filled로 변경
        healthBar.fillAmount = DebugDataManager.Instance.UserData.health / 90f;
    }
}