using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemButtonPress : ItemUseable
{
    public new ItemButtonPressData Data { get { return (ItemButtonPressData)data; } }

    private void Update()
    {
        GetInputs();
    }

    void GetInputs()
    {
        if (!Data.setButton)
            return;
        if (Input.GetButtonDown(Data.button.stringValue))
            StartCoroutine(Toggle());
        
    }

    IEnumerator Toggle()
    {
        base.UseItem();
        yield return new WaitForEndOfFrame();
        base.StopUseItem();
    }
}
