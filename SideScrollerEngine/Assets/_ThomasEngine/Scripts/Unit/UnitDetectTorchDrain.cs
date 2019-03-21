using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(ItemTorch))]
public class UnitDetectTorchDrain : DetectZoneTrigger
{
    [SerializeField] private bool lightDrain = true;
    [SerializeField] private bool hpDrain;
    private ItemTorch torch;

    private void Start()
    {
        torch = GetComponent<ItemTorch>();
    }
    protected override void OnEnter(Collider2D _col)
    {
        base.OnEnter(_col);
        if (lightDrain)
            torch.EnableLightDrain(false);
        if (hpDrain)
            torch.EnableHealthDrain(false);
    }

    protected override void OnExit(Collider2D _col)
    {
        base.OnExit(_col);
        if (lightDrain)
            torch.EnableLightDrain(true);
        if (hpDrain)
            torch.EnableHealthDrain(true);
    }

}
