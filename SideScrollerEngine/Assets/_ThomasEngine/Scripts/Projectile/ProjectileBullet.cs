using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ProjectileBullet : Projectile
{
    public new ProjectileBulletData Data { get { return (ProjectileBulletData)data; } }

    public override void FixedUpdate()
    {
        base.FixedUpdate();
        MoveForward();
    }

    void MoveForward()
    {
        if (!paused)
            transform.Translate(Vector2.right * speed * Time.deltaTime, Space.Self);
    }
}
