using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class Enemy : Unit 
{
    private EnemySoundFX soundFX;

    // Use this for initialization
    protected override void Awake () 
	{
        base.Awake();

        //getcomp
        soundFX = GetComponent<EnemySoundFX>();
	}

    public override void AddHp(float _amount)
    {
        base.AddHp(_amount);
    }

    public override void DamageHp(float _damage)
    {
        if (dead)
            return;

        base.DamageHp(_damage);

        //play sound
        if (soundFX)
            soundFX.PlayHurtSound();
    }


    void DestroyEnemy()
    {
        Destroy(this.gameObject);
    }

}
