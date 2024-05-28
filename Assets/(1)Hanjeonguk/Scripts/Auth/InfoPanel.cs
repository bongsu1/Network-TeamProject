using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoPanel : MonoBehaviour
{
    [SerializeField] TMP_Text infoText;
    [SerializeField] Button closeButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
    }

    public void ShowInfo(string message) //메세지 입력받아 정보창 띄우기
    {
        gameObject.SetActive(true);
        infoText.text = message;
    }
    public void Close()
    {
        gameObject.SetActive(false);
    }
}
