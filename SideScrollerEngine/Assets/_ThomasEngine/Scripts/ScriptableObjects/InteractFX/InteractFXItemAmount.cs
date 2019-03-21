using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemAmount", menuName = "Data/Interacts/ItemAmount", order = 1)]
public class InteractFXItemAmount : InteractFX
{
    [SerializeField] private bool needPrerequisiteItem;
    [SerializeField] private ItemData prerequisiteItem;
    [SerializeField] private float amountDelta;

    private UnitEquip equip;

    public override void DoFX(GameObject _sender = null, GameObject _receiver = null)
    {
        equip = _receiver.GetComponent<UnitEquip>();
        if (needPrerequisiteItem)
        {
            if (equip.CurItem.Data != prerequisiteItem)
            {
                Debug.Log("Invalid skin prerequisite!");
                return;
            }
                
        }
        equip.CurItem.AddAmount(amountDelta);
    }
}
