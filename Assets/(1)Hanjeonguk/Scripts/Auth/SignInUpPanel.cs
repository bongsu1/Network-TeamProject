using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class SignInUpPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] TMP_InputField emailInputField;
    [SerializeField] TMP_InputField passInputField;
    [SerializeField] TMP_InputField confirmInputField;

    [SerializeField] InputFieldTabManager inputFieldTabMrg;

    [SerializeField] Button cancelButton;
    [SerializeField] Button signUpButton;

    private void Awake()
    {
        cancelButton.onClick.AddListener(Cancel);
        signUpButton.onClick.AddListener(SignUp);

        inputFieldTabMrg = new InputFieldTabManager();
        inputFieldTabMrg.Add(emailInputField);
        inputFieldTabMrg.Add(passInputField);
        inputFieldTabMrg.Add(confirmInputField);
    }
    private void Start()
    {
        inputFieldTabMrg.SetFocus();
    }

    private void Update()
    {
        inputFieldTabMrg.CheckFocus();
    }

    public void SignUp()
    {
        SetInteractable(false); 

        string email = emailInputField.text;
        string pass = passInputField.text;
        string confirm = confirmInputField.text;

        if (pass != confirm) 
        {
            panelController.ShowInfo("Password doesn't matched"); 
            SetInteractable(true); 
            return;
        }

        FirebaseManager.Auth.CreateUserWithEmailAndPasswordAsync(email, pass).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled) 
            {
                panelController.ShowInfo("CreateUserWithEmailAndPasswordAsync canceled"); 
                SetInteractable(true); 
                return;
            }
            else if (task.IsFaulted) 
            {
                panelController.ShowInfo($"CreateUserWithEmailAndPasswordAsync failed : {task.Exception.Message}"); 
                SetInteractable(true); 
                return;
            }

            panelController.ShowInfo("CreateUserWithEmailAndPasswordAsync success"); 
            panelController.SetActivePanel(PanelController.Panel.Login); 
            SetInteractable(true); 
        });
    }

    public void Cancel()
    {
        panelController.SetActivePanel(PanelController.Panel.Login);
    }

    private void SetInteractable(bool interactable) 
    {
        emailInputField.interactable = interactable;
        passInputField.interactable = interactable;
        confirmInputField.interactable = interactable;
        cancelButton.interactable = interactable;
        signUpButton.interactable = interactable;
    }
}
    