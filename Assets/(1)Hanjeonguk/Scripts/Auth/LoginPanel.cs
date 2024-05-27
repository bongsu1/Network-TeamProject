using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passInputField;

    [SerializeField] Button loginButton;
    [SerializeField] Button signUpButton;
    [SerializeField] Button resetPassButton;

    [SerializeField] InputFieldTabManager inputFieldTabMrg;

    [SerializeField] bool selectCheck;


    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
        resetPassButton.onClick.AddListener(ResetPass);

        inputFieldTabMrg = new InputFieldTabManager();
        inputFieldTabMrg.Add(emailInputField);
        inputFieldTabMrg.Add(passInputField);
    }

    private void OnEnable()
    {
        inputFieldTabMrg.SetFocus();
    }

    private void Update()
    {
        inputFieldTabMrg.CheckFocus();
    }

    public void Login()
    {

        SetInteractable(false);

        string email = emailInputField.text;
        string pass = passInputField.text;

        FirebaseManager.Auth.SignInWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {

            if (task.IsCanceled)
            {
                panelController.ShowInfo("SignInWithEmailAndPasswordAsync canceled");
                SetInteractable(true);
                return;
            }
            else if (task.IsFaulted)
            {
                panelController.ShowInfo($"SignInWithEmailAndPasswordAsync failed : {task.Exception.Message}");
                SetInteractable(true);
                return;
            }

            if (FirebaseManager.Auth.CurrentUser.IsEmailVerified)  //이메일 인증된 계정
            {
                FirebaseManager.DB //로그인 상태 체크
                .GetReference("UserData")
                .Child(FirebaseManager.Auth.CurrentUser.UserId)
                .Child("isLogin")
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {

                    if (task.IsCanceled)
                    {
                        Debug.Log("Get userdata canceled");
                        return;
                    }
                    else if (task.IsFaulted)
                    {
                        Debug.Log($"Get userdata failed : {task.Exception.Message}");
                        return;
                    }

                    DataSnapshot snapShot = task.Result;

                    bool json = false;

                    if (snapShot.Exists)
                    {
                        json = (bool)snapShot.Value;
                    }

                    if (json == false) //로그인중이 아닌 경우, 로그인 상태로 전환
                    {
                        FirebaseManager.DB
                         .GetReference("UserData")
                         .Child(FirebaseManager.Auth.CurrentUser.UserId)
                         .Child("isLogin")
                         .SetValueAsync(true);

                        PhotonNetwork.LoadLevel("LobbyScene");
                    }

                    else //로그인중인 경우, 기존 접속자 로그아웃 코루틴
                    {
                        StartCoroutine(LogOutAndLogIn());
                    }
                });
            }

            else //이메일 미인증 계정
            {
                panelController.ShowVerify();
            }

            SetInteractable(true);

        });
    }

    public void SignUp()
    {
        panelController.SetActivePanel(PanelController.Panel.SignUp);
    }

    public void ResetPass()
    {
        panelController.ShowReset();
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        passInputField.interactable = interactable;
        signUpButton.interactable = interactable;
        loginButton.interactable = interactable;
        resetPassButton.interactable = interactable;
    }

    IEnumerator LogOutAndLogIn()
    {
        FirebaseManager.DB
                         .GetReference("UserData")
                         .Child(FirebaseManager.Auth.CurrentUser.UserId)
                         .Child("isLogin")
                         .SetValueAsync(false);

        yield return new WaitForSeconds(0.5f);

        FirebaseManager.DB
                         .GetReference("UserData")
                         .Child(FirebaseManager.Auth.CurrentUser.UserId)
                         .Child("isLogin")
                         .SetValueAsync(true);

        yield return new WaitForSeconds(0.5f);

        // PanelManager.Instance.ValueChangeCheck();

        PhotonNetwork.LoadLevel("LobbyScene");
    }
}
