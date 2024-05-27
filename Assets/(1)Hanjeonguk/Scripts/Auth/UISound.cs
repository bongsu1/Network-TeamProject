using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UISound : MonoBehaviour
{
    [SerializeField] AudioClip buttonClick;

    private void Awake()
    {
        Bind();
    }

    private void Bind()
    {
        Component[] components = GetComponentsInChildren<Component>(true);

        foreach (Component child in components)
        {

            if (child is Button button)
            {
                button.onClick.AddListener(ButtonSoundPlay);

            }
        }
    }

    public void ButtonSoundPlay()
    {
        Manager.Sound.PlaySFX(buttonClick);
    }
}
