using UnityEngine;
using UnityEngine.UI;

public class ButtonController : MonoBehaviour
{
    [SerializeField] Button[] buttons; //키 변경 버튼 배열

    private void Start()
    {
        foreach (Button button in buttons) //버튼에 애드리스너로 함수추가
        {
            button.onClick.AddListener(ButtonClick);
        }
    }

    private void ButtonClick() //버튼 클릭했을 때 전체 버튼 비활성화
    {
        foreach (Button button in buttons)
        { 
            button.interactable = false; 
        }
    }

    public void EnableAllButtons() //전체 버튼 활성화
    {
        foreach (Button button in buttons)
        {
            button.interactable = true;
        }
    }
}
