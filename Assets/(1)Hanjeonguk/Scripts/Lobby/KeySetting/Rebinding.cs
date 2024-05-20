using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class Rebinding : MonoBehaviour
{
    [SerializeField] InputActionReference currentAction;
    [SerializeField] TMP_Text buttonText;
    [SerializeField] GameObject button;
    [SerializeField] GameObject waitButton;

    private InputActionRebindingExtensions.RebindingOperation operation;

    private string path = null;

    public void StartRebinding()    
    {
        button.SetActive(false);
        waitButton.SetActive(true);

        currentAction.action.Disable(); //키를 변경하기 전 inputaction 비활성화 

        if (currentAction.action.bindings[0].hasOverrides)
            path = currentAction.action.bindings[0].overridePath;
        else
            path = currentAction.action.bindings[0].path;

        operation = currentAction.action.PerformInteractiveRebinding()
            .WithControlsExcluding("Mouse")  //해당 키 무시
            .WithCancelingThrough("<Mouse>/rightButton") //키변경 취소
            .OnMatchWaitForAnother(0.1f)   //키 변경 지연 시간
            .OnCancel(Operation => RebindCancel()) //키변경 취소되었을 때 함수
            .OnComplete(Operation => RebindComplate()) //키변경 완료되었을 때 함수s
            .Start();
    }                          
    
    public void RebindCancel()
    {
        button.SetActive(true);
        waitButton.SetActive(false);

        currentAction.action.Enable(); //키를 변경한 뒤 inputaction 활성화 

        operation.Dispose(); //인스턴스 삭제, 메모리 누수 방지
    }

    public void RebindComplate()
    {
        button.SetActive(true);
        waitButton.SetActive(false);

        currentAction.action.Enable(); //키를 변경한 뒤 inputaction 활성화 

        operation.Dispose(); //인스턴스 삭제, 메모리 누수 방지

        if (CheckBindings(currentAction.action))
        {
            if (path != null)
                currentAction.action.ApplyBindingOverride(path);
            return;
        }

        //버튼 텍스트 변경
        buttonText.text = InputControlPath.ToHumanReadableString(currentAction.action.bindings[0].effectivePath,InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    public void ShowText()
    {
        buttonText.text = InputControlPath.ToHumanReadableString(currentAction.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private bool CheckBindings(InputAction action)
    {
        InputBinding newBinding = action.bindings[0];

        foreach (InputBinding binding in action.actionMap.bindings)
        {
            if (binding.action == newBinding.action)
                continue;

            if (binding.effectivePath == newBinding.effectivePath)
            {
                Debug.Log($"이미 존재하는 키 입니다. {binding.action} {newBinding.effectivePath} ");
                return true;
            }
        }
        return false;
    }
}
