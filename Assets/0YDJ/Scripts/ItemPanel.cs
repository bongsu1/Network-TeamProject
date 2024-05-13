using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemPanel : MonoBehaviour
{
    [SerializeField] Button BrickButton;
    [SerializeField] Button BrigeButton;



    private void Awake()
    {
        BrickButton.onClick.AddListener(Builtting);
        BrigeButton.onClick.AddListener(Builtting);
    }
    
    private void Builtting()
    {

    }
}
