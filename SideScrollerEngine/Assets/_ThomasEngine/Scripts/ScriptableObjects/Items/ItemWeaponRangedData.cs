using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "WeaponRanged", menuName = "Data/Items/Weapons/Ranged", order = 1)]
public class ItemWeaponRangedData : ItemAimableData
{
    public enum SpreadType { Straight, Angle, Random }
    public enum FireType { Instant, Projectile }
    public int damage;
    public bool instantFirstShot = true;
    public float fireDelay;
    public FireType fireType;
    public float fireDistance;
    public GameObject projectile;
    public float projectileSpeed;
    public SpreadType spreadType;
    public int fireAmount;
    public float angle;
    public float randomAmount;
    public bool aimTransformAtTarget;
    public ChildName aimTrans;
    public LayerMask mask;
}
