using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InputFieldTabManager 
{
    private List<TMP_InputField> list;
    private int curPos;

    public InputFieldTabManager()
    {
        list = new List<TMP_InputField>();
    }

    public void Add(TMP_InputField inputField)
    {
        list.Add(inputField);
    }

    public void SetFocus(int idx = 0)
    {
        

        if (idx >= 0 && idx < list.Count)
        {
            list[idx].ActivateInputField(); //Select
        }
        else
        {
            list[0].ActivateInputField();
        }
      
    }

    private int GetCurerntPos()
    {
        for (int i = 0; i < list.Count; ++i)
        {
            if (list[i].isFocused == true) 
            {
                curPos = i;
                break;
            }
        }
        return curPos;
    }

    private void MoveNext()
    {
        GetCurerntPos();
        if (curPos < list.Count - 1)
        {
            ++curPos;
            list[curPos].ActivateInputField();
        }
    }

    private void MovePrev()
    {
        GetCurerntPos();
        if (curPos > 0)
        {
            --curPos;
            list[curPos].ActivateInputField();
        }
    }

    public void CheckFocus()
    {
        if (Input.GetKeyDown(KeyCode.Tab) && !Input.GetKey(KeyCode.LeftShift))
        {
            MoveNext();
        }
        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKeyDown(KeyCode.Tab))
        {
            MovePrev();
        }
    }
}