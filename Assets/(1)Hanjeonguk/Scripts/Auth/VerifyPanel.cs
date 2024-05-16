using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class VerifyPanel : MonoBehaviour
{
    [SerializeField] PanelController panelController;

    [SerializeField] Button closeButton;
    [SerializeField] Button sendButton;

    private void Awake()
    {
        closeButton.onClick.AddListener(Close);
        sendButton.onClick.AddListener(SendEmail);
    }
    private void OnEnable()
    {
        if (FirebaseManager.Auth == null)
            return;

        StartCoroutine(VerifyCheckRoutine());
    }
    private void OnDisable()
    {
        StopAllCoroutines();
    }

    public void ShowVerify()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        FirebaseManager.Auth.SignOut();
        gameObject.SetActive(false);
    }
    public void SendEmail()
    {
        FirebaseManager.Auth.CurrentUser.SendEmailVerificationAsync().ContinueWithOnMainThread(task =>  
        {
            if (task.IsCanceled)
            {
                panelController.ShowInfo("SendEmailVerificationAsync canceled");
                return;
            }
            else if (task.IsFaulted)
            {
                panelController.ShowInfo($"SendEmailVerificationAsync faild : {task.Exception.Message}");
                return;
            }

            panelController.ShowInfo("SendEmailVerificationAsync success");
        });
    }

    IEnumerator VerifyCheckRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(3f);

            FirebaseManager.Auth.CurrentUser.ReloadAsync().ContinueWithOnMainThread(task => 
            {
                if (task.IsCanceled)
                {
                    panelController.ShowInfo("ReloadAsync canceled");
                    return;
                }
                else if (task.IsFaulted)
                {
                    panelController.ShowInfo($"ReloadAsync failed: {task.Exception.Message}");
                    return;
                }

                if (FirebaseManager.Auth.CurrentUser.IsEmailVerified) 
                {
                    PhotonNetwork.LoadLevel("LobbyScene");
                }
            });
        }
    }
}
