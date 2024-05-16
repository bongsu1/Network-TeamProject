using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LoadingScene : MonoBehaviourPunCallbacks
{
    [SerializeField] GameObject gameLodingScene;
    [SerializeField] GameObject passwordErrorScene;
    [SerializeField] TMP_Text text;
    [SerializeField] string[] loadingMessages;
    [SerializeField] RectTransform image;
    [SerializeField] float moveSpeed ;
    [SerializeField] Button closeButton;

    private static LoadingScene instance;

    private void Awake()
    {
        CreateInstance();
        closeButton.onClick.AddListener(CloseButton);
    }

    public override void OnEnable()
    {
        base.OnEnable();

        StartCoroutine(TextRoutine());
        StartCoroutine(ImageRoutine());
    }

    public override void OnJoinedRoom()
    {
        GameSceneLoading();
    }
    public override void OnJoinRoomFailed(short returnCode, string message)
    {
        if (message == "Game does not exist")
        {
            passwordErrorScene.SetActive(true);
        }
    }

    public void CloseButton()
    {
        passwordErrorScene.SetActive(false);
    }

    public void GameSceneLoading()
    {
        StartCoroutine(GameSceneLoadingRoutine());
    }

    IEnumerator GameSceneLoadingRoutine()
    {
        gameLodingScene.SetActive(true);

        PhotonNetwork.LoadLevel("GameScene");

        while (PhotonNetwork.LevelLoadingProgress < 1)
        {
            yield return null;
        }

        yield return new WaitForSeconds(3f);

        gameLodingScene.SetActive(false);
    }
    IEnumerator TextRoutine()
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

    IEnumerator ImageRoutine()
    {
        float startY = image.anchoredPosition.y; // 초기 Y 값
        float direction = 1f; // 이동 방향

        while (true)
        {
            // 새로운 Y 위치
            float newY = image.anchoredPosition.y + Time.deltaTime * moveSpeed * direction;

            //  이동 방향 반전
            if (newY >= startY || newY <= startY -30f)
            {
                direction *= -1f;
            }

            // 이미지의 위치 변경
            image.anchoredPosition = new Vector2(image.anchoredPosition.x, newY);

            yield return null; 
        }
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

}






