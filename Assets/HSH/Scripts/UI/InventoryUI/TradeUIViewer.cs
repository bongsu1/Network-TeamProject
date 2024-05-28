using UnityEngine;

public class TradeUIViewer : PopUpUI
{
    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (gameObject.activeSelf == true)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
