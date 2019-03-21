using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class InputProperty
{
    public string stringValue;
    public int indexValue;

    private bool InputDown { get { return Input.GetButtonDown(stringValue) || Input.GetAxisRaw(stringValue) != 0; } }
    private bool lastDownCheck = false;
    private bool lastUpCheck = true;

    public bool GetInput()
    {
        return InputDown;
    }

    public bool GetInputDown()
    {
        if (InputDown)
        {
            if (InputDown != lastDownCheck)
            {
                lastDownCheck = InputDown;
                return true;
            }

        }
        lastDownCheck = InputDown;
        return false;
    }

    public bool GetInputUp()
    {
        if (!InputDown)
        {
            if (!InputDown != lastUpCheck)
            {
                lastUpCheck = !InputDown;
                return true;
            }

        }
        lastUpCheck = !InputDown;
        return false;
    }
}