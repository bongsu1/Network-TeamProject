using Firebase.Database;
using Firebase.Extensions;
using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class PanelManager : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameLodingScene;
    [SerializeField] GameObject passwordErrorScene;
    [SerializeField] GameObject optionScene;
    [SerializeField] GameObject LoginErrorPanel;

    [SerializeField] TMP_Text text;
    [SerializeField] TMP_Text currentServer;

    [SerializeField] RectTransform image;

    [SerializeField] string[] loadingMessages;
    [SerializeField] float moveSpeed;

    [SerializeField] Button passwordErrorCloseButton;
    [SerializeField] Button optionCloseButton1;
    [SerializeField] Button optionCloseButton2;
    [SerializeField] Button downButton;
    [SerializeField] Button logoutButton;

    [SerializeField] PlayerInput playerInput;

    [SerializeField] AudioClip lobbySceneBGM;
    [SerializeField] AudioClip gameSceneBGM;

    private static PanelManager instance;
    // public static PanelManager Instance { get { return instance; } }

    [SerializeField] GameObject optionPop;
    [SerializeField] bool optionPopBool = true;

    public void Option(InputAction.CallbackContext value)
    {
        optionPop.SetActive(optionPopBool);
        optionPopBool = !optionPopBool;
    }


    private void Awake()
    {
        CreateInstance();
        passwordErrorCloseButton.onClick.AddListener(PasswordErrorCloseButton);
        optionCloseButton1.onClick.AddListener(OptionCloseButton);
        optionCloseButton2.onClick.AddListener(OptionCloseButton);
        downButton.onClick.AddListener(DownButton);
        logoutButton.onClick.AddListener(Logout);
    }

    public override void OnEnable() //활성화했을 때 같은 아이디로 로그인한 기존 유저 확인 후 로그아웃 적용
    {
        base.OnEnable();

        FirebaseManager.DB
       .GetReference($"UserData/{FirebaseManager.Auth.CurrentUser.UserId}/isLogin")
       .ValueChanged += LoginCheck;
        Manager.Sound.PlayBGM(lobbySceneBGM);
    }

    public override void OnDisable() //비활성화했을 때 포톤 종료
    {
        base.OnDisable();

        //if (PhotonNetwork.CurrentLobby != null)
        //{
        //    PhotonNetwork.LeaveLobby();
        //}

        //if (PhotonNetwork.CurrentRoom != null)
        //{
        //    PhotonNetwork.LeaveRoom();
        //}

        PhotonNetwork.Disconnect();
    }

    public override void OnJoinedRoom()
    {
        GameSceneLoading(); //룸 입장 시 로딩씬 활성화 루틴
        currentServer.text = PhotonNetwork.CurrentRoom.Name; //로딩씬에 현재 접속한 서버이름 표기
        PhotonNetwork.LeaveLobby();
        Manager.Sound.PlayBGM(gameSceneBGM);
    }

    public override void OnLeftRoom()
    {
        Manager.Sound.StopBGM();
    }

    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (message == "Game does not exist") //방 비밀번호 잘못 입력 시 안내창 활성화
        {
            passwordErrorScene.SetActive(true);
        }
    }


    public void GameSceneLoading()
    {
        StartCoroutine(GameSceneLoadingRoutine());
    }

    IEnumerator GameSceneLoadingRoutine() //게임씬 입장할 때 로딩씬 활성화
    {
        gameLodingScene.SetActive(true);
        playerInput.enabled = false; //로딩 중 키 입력 비활성화

        StartCoroutine(TextRoutine()); //텍스트 애니메이션
        StartCoroutine(ImageRoutine()); //이미지 애니메이션

        PhotonNetwork.LoadLevel("GameScene");

        while (PhotonNetwork.LevelLoadingProgress < 1) //LoadLevel의 진행상황, 0~1
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        gameLodingScene.SetActive(false);
        playerInput.enabled = true;
        StopAllCoroutines();

    }

    IEnumerator TextRoutine() //텍스트 애니메이션
    {
        for (int i = 0; i < loadingMessages.Length; i++)
        {
            text.text = loadingMessages[i];

            if (i == loadingMessages.Length - 1)
            {
                i = -1;
            }

            yield return new WaitForSeconds(0.3f);
        }
    }

    IEnumerator ImageRoutine() //이미지 애니메이션
    {
        float startY = image.anchoredPosition.y; // 초기 Y 값
        float direction = 1f; // 이동 방향

        while (true)
        {
            //새로운 Y 위치
            float newY = image.anchoredPosition.y + Time.deltaTime * moveSpeed * direction;

            //이동 방향 반전
            if (newY >= startY || newY <= startY - 30f)
            {
                direction *= -1f;
            }

            //이미지 위치 변경
            image.anchoredPosition = new Vector2(image.anchoredPosition.x, newY);

            yield return null;
        }
    }


    private void LoginCheck(object sender, ValueChangedEventArgs args) //같은 아이디 로그인 체크
    {

        FirebaseManager.DB
                .GetReference("UserData")
                .Child(FirebaseManager.Auth.CurrentUser.UserId)
                .Child("isLogin")
                .GetValueAsync()
                .ContinueWithOnMainThread(task =>
                {
                    if (task.IsCanceled || task.IsFaulted)
                    {
                        Debug.Log("Get userdata canceled");
                        return;
                    }

                    DataSnapshot snapShot = task.Result;

                    bool json = (bool)snapShot.Value;

                    if (json == false) //같은 아이디 접속되어 isLogin = false 될 경우, 로그아웃 안내창 활성화
                    {
                        LoginErrorPanel.SetActive(true);
                    }
                });
    }

    public void DownButton()
    {
        StartCoroutine(DownButtonRoutine());
    }

    IEnumerator DownButtonRoutine()
    {
        LoginErrorPanel.SetActive(false);
        PhotonNetwork.LoadLevel("AuthScene");
        FirebaseManager.Auth.SignOut();

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }

    public void PasswordErrorCloseButton()
    {
        passwordErrorScene.SetActive(false);
    }

    public void OptionCloseButton()
    {
        optionScene.SetActive(false);
    }

    private void CreateInstance()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }
    public void Logout()
    {
        StartCoroutine(LogoutRoutine());
    }

    IEnumerator LogoutRoutine()
    {
        OptionCloseButton();
        FirebaseManager.Auth.SignOut();
        PhotonNetwork.LoadLevel("AuthScene");
        PhotonNetwork.LeaveRoom();

        yield return new WaitForSeconds(1);

        Destroy(gameObject);
    }

    //public void ValueChangeCheck()
    //{
    //    FirebaseManager.DB
    //    //FirebaseDatabase.DefaultInstance
    //   .GetReference($"UserData/{FirebaseManager.Auth.CurrentUser.UserId}/isLogin")
    //   .ValueChanged -= LoginCheck;

    //    FirebaseManager.DB
    //    //FirebaseDatabase.DefaultInstance
    //   .GetReference($"UserData/{FirebaseManager.Auth.CurrentUser.UserId}/isLogin")
    //   .ValueChanged += LoginCheck;

    //    Debug.Log($"ValueChangeCheck 추가{FirebaseManager.Auth.CurrentUser.UserId}");
    //}

    //public void ValueChangeCheckRemove()
    //{
    //    FirebaseManager.DB
    //    //FirebaseDatabase.DefaultInstance
    //    .GetReference($"UserData/{FirebaseManager.Auth.CurrentUser.UserId}/isLogin")
    //   .ValueChanged -= LoginCheck;

    //    Debug.Log($"ValueChangeCheck 제거{FirebaseManager.Auth.CurrentUser.UserId}");
    //}

}






