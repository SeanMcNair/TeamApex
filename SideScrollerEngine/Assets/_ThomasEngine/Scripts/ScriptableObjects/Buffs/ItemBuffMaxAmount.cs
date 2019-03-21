using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemMaxAmount", menuName = "Data/Buffs/ItemMaxAmount", order = 1)]
public class ItemBuffMaxAmount : ItemBuff
{
    public float addToMaxValue = 1;
    public bool addToCurValue = true;

    public override void ActivateBuff(Item _item, bool _activate)
    {
        var useable = (ItemUseable)_item;
        if (useable)
        {
            if (_activate)
                useable.AddToClipSize(addToMaxValue);
            else
                useable.RemoveMaxValue(addToMaxValue);

            if (addToCurValue)
            {
                if (_activate)
                    useable.AddAmount(addToMaxValue);
                else
                    useable.RemoveAmount(addToMaxValue);
            }
        }
        
            

    }

}
