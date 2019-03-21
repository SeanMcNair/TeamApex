using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerBullet : MonoBehaviour
{

    [SerializeField]
    private LayerMask hitMask; 

    private float speed = 0;
    private int damage = 0;

    private Vector3 prevPos;

    [SerializeField]
    private float lifeTime = 3;
    private float lifeTimer;

    [SerializeField]
    private GameObject hitFXPrefab;

    void Start()
    {
        lifeTimer = 0;
        prevPos = transform.position;
    }

    void Update()
    {
        KillTimer();
    }

    void FixedUpdate()
    {
        MoveBullet();
        CheckPassThrough();
    }

    void MoveBullet()
    {
        transform.Translate(Vector3.forward * speed * Time.deltaTime);
    }

    void CheckPassThrough()
    {
        Vector2 movementThisStep = transform.position - prevPos;
        float rayLength = movementThisStep.sqrMagnitude;

        RaycastHit2D hit = Physics2D.Raycast(prevPos, movementThisStep, rayLength, hitMask);
        if (hit)
        {
            if (hit.collider.gameObject.layer == LayerMask.NameToLayer("HitBox"))
            {
                Enemy enemy = hit.collider.transform.root.GetComponentInChildren<Enemy>();
                DamageEnemy(enemy);
            }
            DoHitFX(hit.point, hit.normal);
            KillBullet();   
        }

        prevPos = transform.position;
    }

    void KillTimer()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > lifeTime)
        {
            KillBullet();
        }
    }

    void DamageEnemy(Enemy _enemy)
    {
        _enemy.DamageHp(damage);
    }

    public void SetParams(int _damage, float _speed)
    {
        damage = _damage;
        speed = _speed;
    }

    void DoHitFX(Vector3 _hitPos, Vector3 _normal)
    {
        Instantiate(hitFXPrefab, _hitPos, Quaternion.LookRotation(_normal));
    }

    void KillBullet()
    {
        Destroy(this.gameObject);
    }
	
}
