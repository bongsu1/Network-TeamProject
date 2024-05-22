using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Sound : MonoBehaviour
{
    [SerializeField] AudioClip loginBGM;
        
    private void Start()
    {
        Manager.Sound.PlayBGM(loginBGM);
    }
}
