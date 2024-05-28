using UnityEngine;
using UnityEngine.UI;

public class PanelController : MonoBehaviour
{
    public enum Panel { Login, SignUp }

    [SerializeField] LoginPanel loginPanel;
    [SerializeField] SignInUpPanel signInPanel;
    [SerializeField] ResetPassPanel resetPassPanel;
    [SerializeField] InfoPanel infoPanel;
    [SerializeField] VerifyPanel verifyPanel;
    [SerializeField] Button shotdownButton;

    private void Awake()
    {
        shotdownButton.onClick.AddListener(Quit);
    }
    private void Start()
    {
        SetActivePanel(Panel.Login);
    }

    public void SetActivePanel(Panel panel)
    {
        loginPanel.gameObject.SetActive(panel == Panel.Login);
        signInPanel.gameObject.SetActive(panel == Panel.SignUp);
    }

    public void ShowInfo(string message)
    {
        infoPanel.ShowInfo(message);
    }

    public void ShowVerify()
    {
        verifyPanel.ShowVerify();
    }
    public void ShowReset()
    {
        resetPassPanel.ShowReset();
    }

    public void Quit()
    {
        FirebaseManager.Auth.SignOut();
        Application.Quit();
    }
}

