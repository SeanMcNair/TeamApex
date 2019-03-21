using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemInteractFX : ItemAimable
{
    public new ItemInteractFXData Data { get { return (ItemInteractFXData)data; } }
 
    protected override void OnRunItem()
    {
        base.OnRunItem();
        var obj = gameObject;
        if (Data.runInteractsOnOwner)
            obj = curUnitOwner.gameObject;
        foreach (var fx in Data.interacts)
        {
            fx.DoFX(obj, obj);
        }
    }

}
