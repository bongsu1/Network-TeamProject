using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class RebindSaveLoad : MonoBehaviour
{
    [SerializeField] InputActionAsset actions;
    [SerializeField] Button resetButton;

    [SerializeField] List<Rebinding> rebindings = new List<Rebinding>();

    private void Awake()
    {
        resetButton.onClick.AddListener(resetAllBindings);
    }

    public void OnEnable()
    {
        var rebinds = PlayerPrefs.GetString("rebinds");
        if (!string.IsNullOrEmpty(rebinds))
            actions.LoadBindingOverridesFromJson(rebinds);

        foreach (Rebinding rebinding in rebindings)
            rebinding.ShowText();
    }

    public void OnDisable()
    {
        var rebinds = actions.SaveBindingOverridesAsJson();
        PlayerPrefs.SetString("rebinds", rebinds);
    }

    public void resetAllBindings()
    {
        foreach (InputActionMap map in actions.actionMaps)
            map.RemoveAllBindingOverrides();

        foreach (Rebinding rebinding in rebindings)
            rebinding.ShowText();

        PlayerPrefs.DeleteKey("rebinds");
    }
}