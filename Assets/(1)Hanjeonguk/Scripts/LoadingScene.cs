using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnitySceneManager = UnityEngine.SceneManagement.SceneManager;

public class LoadingScene : MonoBehaviour
{
    [SerializeField] GameObject LodingScene;

    private static LoadingScene instance;

    private void Awake()
    {
        CreateInstance();
    }

    public void GameSceneLoading()
    {
        StartCoroutine(LoadingRoutine());
    }

    IEnumerator LoadingRoutine()
    {
        LodingScene.SetActive(true);

        Time.timeScale = 0f;

        AsyncOperation oper = UnitySceneManager.LoadSceneAsync("GameScene");
        while (oper.isDone == false)
        {
            yield return null;
        }

        Time.timeScale = 1f;

        LodingScene.SetActive(false);
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
