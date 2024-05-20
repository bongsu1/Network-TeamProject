using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class TestController : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;

    public void Option(InputAction.CallbackContext value)
    {
        Debug.Log("환경 설정");
    }
    public void Greeting(InputAction.CallbackContext value)
    {
        Debug.Log("인사");
    }
    public void CameraUp(InputAction.CallbackContext value)
    {
        Debug.Log("카메라 각도 변경");
    }
    public void Inventory(InputAction.CallbackContext value)
    {
        Debug.Log("인벤토리");
    }
}
