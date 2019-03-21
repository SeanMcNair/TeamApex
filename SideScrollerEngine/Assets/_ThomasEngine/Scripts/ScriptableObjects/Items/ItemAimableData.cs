using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemAimableData : ItemUseableData
{
    public float aimDistance = 10;
    public float minAimDistance = 0;
    public bool disableAimBehindMuzzle;
    public string muzzlePos;
    public int muzzlePosInd;
    public AimFX aimFX;
}
