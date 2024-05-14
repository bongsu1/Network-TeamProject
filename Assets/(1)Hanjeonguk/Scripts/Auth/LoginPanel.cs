using Firebase.Auth;
using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Tilemaps;
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
            Debug.Log("SignInWithEmailAndPasswordAsync 통과 ");

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

            if (FirebaseManager.Auth.CurrentUser.IsEmailVerified)  //이메일 확인된 계정
            {
                Debug.Log("이메일 확인 통과 ");

                FirebaseManager.DB //로그인 체크
                .GetReference("UserData")
                .Child(FirebaseManager.Auth.CurrentUser.UserId)
                .Child("isLogin")
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {

                    Debug.Log("GetValueAsync() 통과 ");

                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Debug.Log("Get userdata canceled");
                        return;
                    }
                    
                    DataSnapshot snapShot = task.Result;

                    bool json = false;

                    if (snapShot.Exists)
                    {
                       json = (bool)snapShot.Value;
                    }
                    
                    if(json == false) //로그인중이 아닌 경우 
                    {
                        FirebaseManager.DB
                         .GetReference("UserData")
                         .Child(FirebaseManager.Auth.CurrentUser.UserId)
                         .Child("isLogin")
                         .SetValueAsync(true);

                        PhotonNetwork.LoadLevel("LobbyScene");
                    }

                    else //로그인중인 경우
                    {
                        StartCoroutine(LogOutAndLogIn());
                    }
                });
            }
                
            else 
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

        PhotonNetwork.LoadLevel("LobbyScene");
    }

}
