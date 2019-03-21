using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeaponRanged : ItemAimable
{
    public new ItemWeaponRangedData Data { get { return (ItemWeaponRangedData)data; } }

    private Coroutine fireCoroutine;

    protected override void Start()
    {
        base.Start();     
    }

    protected override void OnRunItem()
    {
        base.OnRunItem();
        FireWeapon();
    }

    void FireWeapon()
    {
        if (!muzzle)
            return;

        muzzle.LookAt2D(controller.AimPos, true);

        //fx
        if (Data.particleFX)
            Instantiate(Data.particleFX, muzzle.position, muzzle.rotation);

        if (Data.spreadType != ItemWeaponRangedData.SpreadType.Straight)
        {
            float ang = Data.angle / (Data.fireAmount - 1);
            float curRot = Data.angle / 2;
            if (Data.fireAmount > 1)
            {
                for (int i = 0; i < Data.fireAmount; i++)
                {
                    if (Data.spreadType == ItemWeaponRangedData.SpreadType.Random)
                    {
                        ang = Random.Range(ang - Data.randomAmount, ang + Data.randomAmount);
                        curRot = Random.Range(curRot - Data.randomAmount, curRot + Data.randomAmount);
                    }
                    if (i == 0)
                        muzzle.Rotate(0, 0, -curRot);
                    else
                        muzzle.Rotate(0, 0, ang);

                    Shoot();
                }
            }
        }
        else
        {
            Shoot();
        }
        DoLineFX(controller.AimPos);

    }

    void Shoot()
    {
        if (Data.fireType == ItemWeaponRangedData.FireType.Instant)
            ShootInstant();
        else
            ShootProjectile();
    }

    void ShootProjectile()
    {
        var proj = Instantiate(Data.projectile).GetComponent<Projectile>();
        proj.transform.position = muzzle.position;
        var col = Physics2D.OverlapPoint(controller.AimPos, Data.mask);
        Transform tar = null;
        if (col)
        {
            tar = col.transform;
            var unit = col.GetComponent<Unit>();
            if (unit)
            {
                if (unit.AttackTarget)
                    tar = unit.AttackTarget;

            }   
        }
            
        proj.ShootProjectile(Data.projectileSpeed, Data.damage,controller.AimDirection, Data.mask, transform, tar, controller.AimPos);
    }

    void ShootInstant()
    {
        RaycastHit2D hit = Physics2D.Raycast(muzzle.position, muzzle.forward, Data.fireDistance, Data.mask);
        if (hit)
        {
            var unit = hit.collider.GetComponent<Unit>();
            if (unit)
                unit.DamageHp(Data.damage);
        }
    }

}
