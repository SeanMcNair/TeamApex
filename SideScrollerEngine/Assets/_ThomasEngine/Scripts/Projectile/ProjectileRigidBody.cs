using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileRigidBody : Projectile
{

    public new ProjectileRigidBodyData Data { get { return (ProjectileRigidBodyData)data; } }
    protected Rigidbody2D rb;

    public override void Awake()
    {
        base.Awake();
        rb = GetComponent<Rigidbody2D>();
    }

    public override void ShootProjectile(float _speed, int _damage, Vector2 _direction, LayerMask _mask, Transform _sender = null, Transform _target = null, Vector2 _targetPos = default(Vector2), bool _bounced = false)
    {
        base.ShootProjectile(_speed, _damage,_direction, _mask, _sender, _target);
        LaunchProjectile();
    }

    public override void PauseProjectile(bool _pause)
    {
        base.PauseProjectile(_pause);
        rb.Sleep();
    }

    void LaunchProjectile()
    {
        rb.Sleep();
        rb.AddForce(velocityDirection * speed, ForceMode2D.Impulse);
    }
}
