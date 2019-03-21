using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : Linkable
{
    public new ProjectileData Data { get { return (ProjectileData)data; } }
    [SerializeField] protected DetectZone detectZone;
    private float lifeTimer;

    protected float speed;
    public float Speed { get { return speed; } }
    protected int damage;
    public int Damage { get { return damage; } }
    protected LayerMask mask;
    public LayerMask Mask { get { return mask; } }
    protected Transform target;
    public Transform Target { get { return target; } }
    protected Vector2 targetPos;
    public Vector2 TargetPos { get { return targetPos; } }
    protected float velocitySpeed;
    public float VelocitySpeed { get { return velocitySpeed; } }
    protected Vector2 velocityDirection;
    public Vector2 Direction { get { return velocityDirection; } }
    protected Transform sender;
    public Transform Sender { get { return sender; } }
    protected Vector2 lastPos;

    private Collider2D hit;
    private RaycastHit2D rayHit;
    private Vector2 hitPos;
    private Vector2 hitNormal;
    private int curhitAmount;

    protected bool bounced;
    protected bool paused;

    public virtual void Awake()
    {
        lastPos = transform.position;
    }

    private void Start()
    {
        StartCoroutine(StartCalculateSpeed());
    }

    // Update is called once per frame
    public virtual void Update()
    { 
        KillTimer();
    }

    public virtual void FixedUpdate()
    {
        DetectCollision();
    }

    public virtual void ShootProjectile(float _speed, int _damage, Vector2 _direction, LayerMask _mask,Transform _sender = null,Transform _target = null, Vector2 _targetPos = default(Vector2), bool _bounced = false)
    {
        speed = _speed;
        damage = _damage;
        mask = _mask;
        detectZone.detectMask = mask;
        sender = _sender;
        target = _target;
        targetPos = _targetPos;
        velocityDirection = _direction;
        bounced = _bounced;
        transform.rotation = Quaternion.identity;
        transform.LookAt2D(transform.TransformPoint(_direction), false, true);
    }

    void DetectCollision()
    {
        hit = detectZone.DetectCollider(transform);

        if (hit)
        {
            GetHitInfo();
            if (Data.spawnOnImpact.Length > 0)
            {
                DoSpawnOnImpact();
            }

            if (Data.interactFX.Length > 0)
                DoInteractFX(hit.gameObject);

            Unit unit = hit.GetComponent<Unit>();
            if (unit)
            {
                DoDamage(unit);
            }

            curhitAmount++;
            if (curhitAmount >= Data.hitMaxAmount)
                KillProjectile();      
        }

        lastPos = transform.position;
    }

    void GetHitInfo()
    {
        if (detectZone.detectType == DetectZone.DetectAreaType.LineCast)
        {
            hitPos = detectZone.lineHit.point;
            hitNormal = detectZone.lineHit.normal;
        }
        else
        {
            //calculate distance and direction based on detectarea
            float dist = 1;
            if (detectZone.detectType == DetectZone.DetectAreaType.Circle)
                dist = detectZone.radius;
            if (detectZone.detectType == DetectZone.DetectAreaType.Box)
            {
                dist = detectZone.size.x;
                if (detectZone.size.y > dist)
                    dist = detectZone.size.y;
            }   
            //set the direction distance point
            var dirPos = lastPos + (velocityDirection * dist);
            //find closest point on hit collider to the directional distance
            hitPos = hit.bounds.ClosestPoint(dirPos);
            //calculate the direction and distance to that point
            var dir = (hitPos - lastPos).normalized;
            dist = Vector2.Distance(hitPos, lastPos);
            //fire a linecast to that point on the collider
            var hitInfo = Physics2D.Linecast(lastPos, lastPos + (dir * (dist + 0.1f)), detectZone.detectMask);
            //set our hitpos and normal
            hitPos = hitInfo.point;
            hitNormal = hitInfo.normal;
        }
        
    }

    void DoDamage(Unit _unit)
    {
        _unit.DamageHp(damage);
    }

    void DoSpawnOnImpact()
    {
        foreach (var spawn in Data.spawnOnImpact)
        {
            Instantiate(spawn, hitPos, Quaternion.LookRotation(Vector3.forward, hitNormal));
        }
    }

    void DoInteractFX(GameObject _reciever)
    {
        foreach (var interact in Data.interactFX)
        {
            interact.DoFX(this.gameObject, _reciever);
        }
    }

    void KillTimer()
    {
        lifeTimer += Time.deltaTime;
        if (lifeTimer > Data.lifeTime)
        {
            KillProjectile();
        }
    }

    public virtual void PauseProjectile(bool _pause)
    {
        paused = _pause;
    }

    public virtual void KillProjectile()
    {
        Destroy(this.gameObject);
    }

    IEnumerator StartCalculateSpeed()
    {
        var pos = transform.position;
        while (Application.isPlaying)
        {
            // Position at frame start
            pos = transform.position;
            // Wait till it the end of the frame
            yield return new WaitForFixedUpdate();
            // Calculate velocity: Velocity = DeltaPosition / DeltaTime
            velocityDirection = transform.position - pos;
            velocitySpeed = (velocityDirection / Time.fixedDeltaTime).magnitude;
        }
    }
}
