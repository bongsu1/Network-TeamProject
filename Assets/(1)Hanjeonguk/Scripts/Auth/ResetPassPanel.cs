using Firebase.Extensions;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ResetPassPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] TMP_InputField emailInputField;

    [SerializeField] InputFieldTabManager inputFieldTabMrg;

    [SerializeField] Button closeButton;
    [SerializeField] Button sendButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        sendButton.onClick.AddListener(SendReset);

        inputFieldTabMrg = new InputFieldTabManager();

        inputFieldTabMrg.Add(emailInputField);
    }
    private void Start()
    {
        inputFieldTabMrg.SetFocus();
    }

    private void SendReset()
    {
        SetInteractable(false);

        string email = emailInputField.text;

        FirebaseManager.Auth.SendPasswordResetEmailAsync(email).ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled)
            {
                panelController.ShowInfo("SendPasswordResetEmailAsync canceled");
                SetInteractable(true);
                return;
            }
            else if (task.IsFaulted)
            {
                panelController.ShowInfo($"SendPasswordResetEmailAsync failed: {task.Exception.Message}");
                SetInteractable(true);
                return;
            }

            panelController.ShowInfo("SendPasswordResetEmailAsync success");
            Close();
            SetInteractable(true);
        });
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    public void ShowReset()
    {
        gameObject.SetActive(true);
    }

    private void SetInteractable(bool interactable)
    {
        emailInputField.interactable = interactable;
        sendButton.interactable = interactable;
        closeButton.interactable = interactable;
    }
}
