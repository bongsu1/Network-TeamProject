using UnityEngine;

public class Sound : MonoBehaviour
{
    [SerializeField] AudioClip loginBGM;

    private void Start()
    {
        Manager.Sound.PlayBGM(loginBGM);
    }
}
