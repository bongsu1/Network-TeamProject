using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
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

    private void Awake()
    {
        signUpButton.onClick.AddListener(SignUp);
        loginButton.onClick.AddListener(Login);
        resetPassButton.onClick.AddListener(ResetPass);
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

            if (FirebaseManager.Auth.CurrentUser.IsEmailVerified) 
            {
                PhotonNetwork.LoadLevel("LobbyScene");
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
}
