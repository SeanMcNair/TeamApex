using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ItemAmount", menuName = "Data/Interacts/PickUpAmmo", order = 1)]
public class InteractFXPickUpAmmo : InteractFX
{
    [SerializeField] private bool setAmmo;
    [SerializeField] private ValueAmountData ammoToPickUp;

    private UnitEquip equip;

    public override void DoFX(GameObject _sender = null, GameObject _receiver = null)
    {
        equip = _receiver.GetComponent<UnitEquip>();
        if (!equip)
            return;

        var ammo = ammoToPickUp;
        if (!setAmmo)
        {
            ammo = equip.CurItem.AmmoAmountLocal.data;
        }
        equip.CurItem.PickUpAmmo(ammo);
    }
}
