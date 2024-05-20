using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting.FullSerializer;
using UnityEngine;
using UnityEngine.InputSystem;

public class Controller : MonoBehaviour
{
    [SerializeField] PlayerInput playerInput;
    [SerializeField] CharacterController characterController;

    [SerializeField] float moveSpeed = 5f;
    [SerializeField] Vector3 moveDir;

    public PlayerInput PlayerInput => playerInput;

    private void Update()
    {
        Vector3 finalDir = moveDir;
        finalDir *= moveSpeed * Time.deltaTime;
        characterController.Move(finalDir);    

        Vector3 velocity = characterController.velocity;
        velocity.y = 0f;

        if (velocity.magnitude > 0.2f)
        {
            transform.rotation = Quaternion.LookRotation(finalDir);
        }

    }

    public void Move(InputAction.CallbackContext ctx)
    {
        Vector3 inputValue = ctx.ReadValue<Vector2>();
        moveDir = new Vector3(inputValue.x, 0f, inputValue.y);
    }

    public void Jump(InputAction.CallbackContext ctx)
    {
        Debug.Log("Jump");
    }
}
