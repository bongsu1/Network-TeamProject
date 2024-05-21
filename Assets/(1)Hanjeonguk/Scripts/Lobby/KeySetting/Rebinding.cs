using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Rebinding : MonoBehaviour
{
    [SerializeField] InputActionReference currentAction;
    [SerializeField] InputActionAsset actionAsset;

    [SerializeField] TMP_Text buttonText;
    [SerializeField] GameObject button;
    [SerializeField] GameObject waitButton;
    [SerializeField] GameObject duplicationError;

    [SerializeField] Button duplicationErrorClose;

    [SerializeField] ButtonController buttonController;

    private InputActionRebindingExtensions.RebindingOperation operation;

    private string path = null;

    private void Awake()
    {
        duplicationErrorClose.onClick.AddListener(DuplicationErrorClose);
        buttonController = FindAnyObjectByType<ButtonController>();
    }

    public void StartRebinding() //키 변경을 위해, 버튼 클릭했을 때
    {
        //UI
        button.SetActive(false);
        waitButton.SetActive(true);

        currentAction.action.Disable(); //키를 변경하기 전 inputaction 비활성화 

        if (currentAction.action.bindings[0].hasOverrides)  //오버라이드 되었는지 확인
            path = currentAction.action.bindings[0].overridePath; //오버라이드 경로 가져오기
        else
            path = currentAction.action.bindings[0].path; //기본 경로 가져오기

        operation = currentAction.action.PerformInteractiveRebinding() //키변경 함수
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
        buttonController.EnableAllButtons();

        currentAction.action.Enable(); //키를 변경한 뒤 inputaction 활성화 

        operation.Dispose(); //인스턴스 삭제, 메모리 누수 방지
    }

    public void RebindComplate()
    {
        button.SetActive(true);
        waitButton.SetActive(false);
        buttonController.EnableAllButtons();

        currentAction.action.Enable(); //키를 변경한 뒤 inputaction 활성화 

        operation.Dispose(); //인스턴스 삭제, 메모리 누수 방지

        if (CheckBindings(currentAction.action)) //중복 값이 존재할 때
        {
            if (path != null)
                currentAction.action.ApplyBindingOverride(path); //기존 값 다시 적용

            return;
        }

        ShowText(); 
    }

    public void ShowText() //버튼 텍스트 변경
    {
        buttonText.text = InputControlPath.ToHumanReadableString(currentAction.action.bindings[0].effectivePath, InputControlPath.HumanReadableStringOptions.OmitDevice);
    }

    private bool CheckBindings(InputAction action)
    {
        InputBinding newBinding = action.bindings[0];

        foreach (InputActionMap actionMap in actionAsset.actionMaps) //actionMaps 수만큼 반복
        {
            foreach (InputBinding binding in actionMap.bindings) //binding 수만큼 반복
            {
                Debug.Log($"{binding.action} / {newBinding.action}");
                if (binding.action == newBinding.action) //자신과 같은 값일 때 무시하고 진행
                    
                    continue;

                Debug.Log($"{binding.effectivePath} / {newBinding.effectivePath}");
                if (binding.effectivePath == newBinding.effectivePath) //경로가 같은 값이 있을 때 중복으로 판단
                {
                    duplicationError.SetActive(true);
                    Debug.Log($"이미 존재하는 키 입니다. {binding.action} {newBinding.effectivePath} ");

                    return true;
                }
            }
        }

        return false;
    }

    public void DuplicationErrorClose()
    {
        duplicationError.SetActive(false);
    }
}
