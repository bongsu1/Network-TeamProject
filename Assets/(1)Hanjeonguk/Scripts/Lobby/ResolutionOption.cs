using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ResolutionOption : MonoBehaviour
{
    FullScreenMode screenMode;

    [SerializeField] Toggle fullScreenButton;
    [SerializeField] Dropdown dropdown;
    [SerializeField] int resolutionNumber;

    List<Resolution> resolutions = new List<Resolution>();

    private void Start()
    {
        ResolutionSet();
    }

    public void ResolutionSet()
    {
        //resolutions.AddRange(Screen.resolutions); //지원 가능한 해상도 리스트에 추가
        for (int i = 0; i < Screen.resolutions.Length; i++)
        {
            if (Screen.resolutions[i].refreshRateRatio.value >= 60f && Screen.resolutions[i].width >= 800f)
                resolutions.Add(Screen.resolutions[i]);
        }
        dropdown.options.Clear(); //드롭다운 청소

        int optionNum = 0;

        foreach (Resolution resolution in resolutions)
        {
            Dropdown.OptionData optionData = new Dropdown.OptionData();
            optionData.text = $"{resolution.width}X{resolution.height} {resolution.refreshRateRatio.value}hz";
            dropdown.options.Add(optionData);

            if (resolution.width == Screen.width && resolution.height == Screen.height)
                dropdown.value = optionNum;
            optionNum++;

        }

        dropdown.RefreshShownValue(); //새로고침
        fullScreenButton.isOn = Screen.fullScreenMode.Equals(FullScreenMode.FullScreenWindow) ? true : false;
    }

    public void DropdownOptionChange(int value)
    {
        resolutionNumber = value;
    }

    public void FullScreenButton(bool isFull)
    {
        screenMode = isFull ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.SetResolution(resolutions[resolutionNumber].width, resolutions[resolutionNumber].height, screenMode);
    }

    public void SetButton()
    {
        Screen.SetResolution(resolutions[resolutionNumber].width, resolutions[resolutionNumber].height, screenMode);
    }
}
