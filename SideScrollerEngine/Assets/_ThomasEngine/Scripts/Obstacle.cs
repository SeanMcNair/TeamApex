using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{

    [SerializeField] private string damageTag; 
    [SerializeField] private int damage = 1;
    [SerializeField] private bool trigger = true;
    [SerializeField] private bool hurtOnStay = true;
    [SerializeField] private bool oneHitKill = false;
    [SerializeField] private bool bounceUnit = true;
    [SerializeField] private float bounceForce = 5;

    void Start()
    {
        Collider2D col = GetComponent<Collider2D>();
        col.isTrigger = trigger;
    }

    void OnCollisionStay2D(Collision2D col)
    {

        if (col.gameObject.tag == damageTag)
        {
            if (hurtOnStay)
            {
                Player pl = col.gameObject.GetComponent<Player>();
                AddDamage(pl, col.contacts[0].point);
            }
        }
    }

    void OnCollisionEnter2D(Collision2D col)
    {

        if (col.gameObject.tag == damageTag)
        {
            Player pl = col.gameObject.GetComponent<Player>();
            AddDamage(pl, col.contacts[0].point);
                
        }
    }

    void OnTriggerStay2D(Collider2D col)
    {

        if (col.tag == damageTag)
        {
            if (hurtOnStay)
            {
                Player pl = col.GetComponent<Player>();
                AddDamage(pl, pl.transform.position);
            }
        }
    }

    void OnTriggerEnter2D(Collider2D col)
    {

        if (col.tag == damageTag)
        {
            Player pl = col.GetComponent<Player>();
            AddDamage(pl,pl.transform.position);
        }
    }

    void AddDamage(Unit _unit, Vector2 _contactPoint)
    {
        if (oneHitKill)
            _unit.DamageHp(_unit.CurHP);
        else
        {
            if (bounceUnit)
            {
                var direction = (_contactPoint - (Vector2)transform.position).normalized;
                _unit.DamageHp(damage, bounceForce, direction);
                

            }
            else
            {
                _unit.DamageHp(damage);
            }
            
        }
            
    }


}
