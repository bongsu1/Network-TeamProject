using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.UI;
using static Cinemachine.DocumentationSortingAttribute;
//using UnityEngine.UIElements;

public class Options : MonoBehaviour
{
    [SerializeField] AudioMixer audioMixer;

    [SerializeField] Toggle qualityLow;
    [SerializeField] Toggle qualityMedium;
    [SerializeField] Toggle qualityHigh;

    [SerializeField] Slider bgmSlider;
    [SerializeField] Slider sfxSlider;

    [SerializeField] Button quitButton;
    [SerializeField] Button keyPopButton;
    [SerializeField] Button keyPopCloseButton;
    [SerializeField] Button keyPopCloseButton2;

    [SerializeField] GameObject keyRebinding;   

    private void Awake()
    {
        audioMixer.GetFloat("BGM", out float b_value);
        bgmSlider.value = b_value;

        audioMixer.GetFloat("SFX", out float s_value);
        sfxSlider.value = s_value;

        bgmSlider.onValueChanged.AddListener(delegate { SliderCheck(bgmSlider); });
        sfxSlider.onValueChanged.AddListener(delegate { SliderCheck(sfxSlider); });

       // QualitySetting(QualitySettings.GetQualityLevel());

        qualityLow.onValueChanged.AddListener(delegate { QualitySetting(0); });
        qualityMedium.onValueChanged.AddListener(delegate { QualitySetting(1); });
        qualityHigh.onValueChanged.AddListener(delegate { QualitySetting(2); });

        quitButton.onClick.AddListener(Quit);

        keyPopButton.onClick.AddListener(KeyPop);
        keyPopCloseButton.onClick.AddListener(KeyPopClose);
        keyPopCloseButton2.onClick.AddListener(KeyPopClose);
    }

    public void QualitySetting(int level)
    {
        QualitySettings.SetQualityLevel(level); //퀄리티 세팅 변경
        Debug.Log(QualitySettings.GetQualityLevel()); //퀄리티 세팅 읽기
    }

    public void SliderCheck(Slider slider)
    {
        if (slider.value <= -30f)
        {   
            if (slider == bgmSlider)
            {
                audioMixer.SetFloat("BGM", -80f);
            }
            else
                audioMixer.SetFloat("SFX", -80f);
        }
        else
        {
            if (slider == bgmSlider)
            {
                audioMixer.SetFloat("BGM", slider.value);
            }
            else
                audioMixer.SetFloat("SFX", slider.value);
        }
    }

    public void SoundOn()
    {
        audioMixer.SetFloat("Master", 20);
    }

    public void SoundOff()
    {
        audioMixer.SetFloat("Master", -80);
    }

    public void Quit()
    {
        FirebaseManager.Auth.SignOut();
        Application.Quit();
    }

    public void KeyPop()
    {
        keyRebinding.SetActive(true);
    }
    public void KeyPopClose()
    {
        keyRebinding.SetActive(false);
    }
}
