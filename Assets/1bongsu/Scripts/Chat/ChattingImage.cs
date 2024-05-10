using System.Collections;
using TMPro;
using UnityEngine;

public class ChattingImage : MonoBehaviour
{
    [SerializeField] TMP_Text[] text;

    private void OnEnable()
    {
        StartCoroutine(ChattingRoutoine()); // 셀애니메이션 or 비디오 대체
    }

    IEnumerator ChattingRoutoine()
    {
        int start = 0;
        while (true)
        {
            SetActiveText(start);
            start++;

            if (start >= text.Length) // text의 갯수를 넘으면 처음부터
                start = 0;

            yield return new WaitForSeconds(0.5f);
        }
    }
    private void SetActiveText(int index)
    {
        for (int i = 0; i < text.Length; i++)
        {
            text[i].gameObject.SetActive(i == index);
        }
    }
}
