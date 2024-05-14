using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UserPanel : MonoBehaviour
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
        FirebaseManager.DB
            .GetReference("UserData")
            .Child(FirebaseManager.Auth.CurrentUser.UserId)
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
                    string json = snapShot.GetRawJsonValue(); //Json형태로 값 읽기 //(int)snapShot.Value 값만 가져오기
                    Debug.Log(json);

                    userData = JsonUtility.FromJson<UserData>(json); //유저데이터 형식으로 역직렬화

                    nickNameText.text = userData.nickName;
                }
                else //스냅샷 없을 때
                {
                    userData = new UserData();
                    nickNameText.text = userData.nickName;

                    FirebaseManager.Instance.DataSave(userData);
                }
            });
    }

    public void ChangeNickName() //닉네임만 변경하기
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
                     Debug.Log("이미 존재하는 닉네임 입니다.");
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
                 Debug.Log($"닉네임 재설정 완료");
             });

             PhotonNetwork.LocalPlayer.NickName = nickNameInputField.text;
         });
    }

    public void CloseInfoPanel()
    {
        infoPanel.SetActive(false);
    }


}
