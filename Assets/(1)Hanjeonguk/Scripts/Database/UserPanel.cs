using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour //로비씬에서 닉네임 관리
{
    [SerializeField] TMP_Text nickNameText;
    [SerializeField] TMP_InputField nickNameInputField;
    [SerializeField] Button nickNameChangeButton;

    [SerializeField] GameObject infoPanel;

    [SerializeField] Button closeButton;


    private UserData userData;

    private void Awake()
    {
        nickNameChangeButton.onClick.AddListener(ChangeNickName);
        closeButton.onClick.AddListener(CloseInfoPanel);
    }

    private void Start()
    {
        PhotonNetwork.ConnectUsingSettings(); //포톤네트워크 접속

        FirebaseManager.DB
            .GetReference("UserData")
            .Child(FirebaseManager.Auth.CurrentUser.UserId)
            .Child("nickName")
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

                DataSnapshot snapShot = task.Result; //결과값을 스냅샷에 저장
                if (snapShot.Exists) //스냅샷 있을 때
                {
                    //string json = snapShot.GetRawJsonValue(); //Json형태로 값 읽기 //(int)snapShot.Value 값만 가져오기
                    //userData = JsonUtility.FromJson<UserData>(json); //유저데이터 형식으로 역직렬화
                    //nickNameText.text = userData.nickName;

                    string json = snapShot.Value.ToString();
                    nickNameText.text = json;

                }
                else //스냅샷 없을 때
                {
                    userData = new UserData();
                    nickNameText.text = userData.nickName;
                }
            });
    }

    public void ChangeNickName() //닉네임 변경하기
    {
        string nickName = nickNameInputField.text;


        FirebaseManager.DB // 중복 닉네임 체크 
        .GetReference("UserData")
        .OrderByChild("nickName")
        .GetValueAsync()
        .ContinueWithOnMainThread(task =>
         {
             if (task.IsCanceled)
             {
                 Debug.Log("닉네임 재설정 취소");
                 return;
             }
             else if (task.IsFaulted)
             {
                 Debug.Log($"닉네임 재설정 오류");
                 return;
             }

             DataSnapshot dataSnapshot = task.Result;

             foreach (DataSnapshot child in dataSnapshot.Children)
             {
                 string userName = (string)child.Child("nickName").Value;

                 if (userName == nickName)
                 {
                     infoPanel.SetActive(true);
                     return;
                 }
             }

             FirebaseManager.DB // 닉네임 변경
              .GetReference("UserData")
              .Child(FirebaseManager.Auth.CurrentUser.UserId)
              .Child("nickName")
              .SetValueAsync(nickName)
              .ContinueWithOnMainThread(task =>
             {
                 if (task.IsCanceled)
                 {
                     Debug.Log("닉네임 재설정 취소");
                     return;
                 }
                 else if (task.IsFaulted)
                 {
                     Debug.Log($"닉네임 재설정 오류");
                     return;
                 }
       
                 nickNameText.text = nickName;
             });

             PhotonNetwork.LocalPlayer.NickName = nickNameInputField.text;
         });
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }

}
