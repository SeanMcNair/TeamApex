using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemWeaponMelee : ItemUseable
{
    public new ItemWeaponMeleeData Data { get { return (ItemWeaponMeleeData)data; } }
    [SerializeField] private DetectZone detectZone;

    private Collider2D[] cols;
    private List<Collider2D> damagedCols = new List<Collider2D>();

    private UnitAnimations anim;
    private Coroutine attackCoroutine;

    protected override void OnOwnerFound()
    {
        base.OnOwnerFound();
        anim = curUnitOwner.GetComponent<UnitAnimations>();
    }

    protected override void OnRunItem()
    {
        base.OnRunItem();
        if (attackCoroutine != null)
            StopCoroutine(attackCoroutine);

        attackCoroutine = StartCoroutine(StartAttack());
    }

    IEnumerator StartAttack()
    {
        if (anim)
            anim.PlayMeleeAttack();
        yield return new WaitForSeconds(Data.damageDelay);
        float timer = 0;
        while (timer < Data.activeTime)
        {
            timer += Time.deltaTime;
            if (timer > Data.activeTime)
                timer = Data.activeTime;
            Attack();
            yield return new WaitForEndOfFrame();
        }
        OnStopItem();
        damagedCols.Clear();
    }

    void Attack()
    {
        cols = detectZone.DetectColliders(transform);
        if (cols.Length > 0)
        {
            for (int i = 0; i < Data.unitAmount; i++)
            {
                if (!damagedCols.Contains(cols[i]))
                {
                    Vector2 dir = Data.direction;
                    if (Data.bounceType == ItemWeaponMeleeData.BounceType.ClosetPoints)
                        dir = ((Vector2)cols[i].bounds.center - detectZone.offset).normalized;
                    else if (Data.bounceType == ItemWeaponMeleeData.BounceType.XOnly)
                    {
                        if (curUnitOwner.transform.position.x < cols[i].transform.position.x)
                            dir = Vector2.right;
                        else
                            dir = Vector2.left;
                    }

                    var u = cols[i].GetComponent<Unit>();
                    if (u)
                    {
                        DamageUnit(u, dir);
                        damagedCols.Add(cols[i]);
                    }
                }      
            }
        }
        
    }

    void DamageUnit(Unit _unit, Vector2 _dir)
    {
        _unit.DamageHp(Data.damage);
        var cont = _unit.GetComponent<UnitController>();
        if (cont)
        {
            cont.Bounce(_dir, Data.bounceForce);
        }
            
    }

}
