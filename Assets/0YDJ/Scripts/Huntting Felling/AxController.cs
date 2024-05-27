using UnityEngine;

public class AxController : MonoBehaviour
{
    [SerializeField] Action action;

    public void EnableWeapon()
    {
        action.EnableWeapon();
    }

    public void DisableWeapon()
    {
        action.DisableWeapon();
    }
}
