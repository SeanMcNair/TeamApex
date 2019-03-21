using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{

    [SerializeField]
    private int damage = 1;
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float chargeDelay;
    private float chargeTimer;
    [SerializeField]
    private bool delayOnlyOnce = true;
    [SerializeField]
    private bool obstacleOnly;
    [SerializeField]
    private float attackDelay = 1;
    [SerializeField]
    private float attackDistance = 1;
    [SerializeField]
    private bool leap;
    [SerializeField]
    private float leapYForce = 5;
    [SerializeField]
    private float leapXForce = 5;
    [SerializeField]
    private float leapDelay = 1;
    [SerializeField]
    private EnemyController enemyController;
    [SerializeField]
    private float maxAttackAngle = 90;

    private bool attacking;
    private bool triggered;

    private Collider2D plCol;
    private Player player;

    private float distance;

    private float xTimer;
    private float yTimer;

    private bool delayedOnce;
    private bool pausePatrol;
    private bool leaping;

    private EnemyAnimations anim;
    private Enemy enemy;
    private EnemySoundFX soundFX;

    void Start()
    {
        anim = GetComponent<EnemyAnimations>();
        enemy = GetComponent<Enemy>();
        soundFX = GetComponent<EnemySoundFX>();
    }

    void Update()
    {
        if (anim)
            SyncAnimations();
    }

    void SyncAnimations()
    {
        //anim.attack = attacking;
        //anim.powerUp = poweringUp;
    }

    void FixedUpdate()
    {

        if (triggered)
        {
            if (!pausePatrol)
            {
                enemyController.PausePatrol(true);
                pausePatrol = true;
            }

            if (!enemy.IsDead)
            {
                MoveToPlayer();
                AttackPlayer();
            }
        }
        else if (pausePatrol)
        {
            enemyController.PausePatrol(false);
            pausePatrol = false;
        }
    }

    void MoveToPlayer()
    {
        chargeTimer += Time.deltaTime;
        if (chargeTimer > chargeDelay || delayedOnce)
        {
            if (delayOnlyOnce && !delayedOnce)
            {
                delayedOnce = true;
            }

            if (!attacking)
                transform.LookAt(new Vector3(player.transform.position.x, transform.position.y, player.transform.position.z));

            if (!leaping)
                transform.Translate(Vector3.forward * speed * Time.fixedDeltaTime);

        }

    }

    void AttackPlayer()
    {
        distance = Vector2.Distance(player.transform.root.position, transform.position);
        if (distance < attackDistance)
        {
            if (!attacking)
            {
                attacking = true;

                if (!obstacleOnly)
                {
                    if (leap)
                    {
                        StartCoroutine("LeapAttack");
                    }
                    else
                    {
                        StartCoroutine("StartAttack");
                    }
                }
            }

        }
        else if (attacking)
        {
            StopCoroutine("StartAttack");
            StopCoroutine("LeapAttack");
            attacking = false;
            chargeTimer = 0;
        }
    }


    IEnumerator LeapAttack()
    {
        leaping = true;
        yield return new WaitForSeconds(leapDelay);
        Vector2 dir = (player.transform.position - transform.position).normalized;
        Vector2 force = new Vector2(dir.x * leapXForce, leapYForce);
        if (!enemy.IsDead)
        {
            GetComponent<Rigidbody2D>().AddForce(force, ForceMode2D.Impulse);

            //play sound
            if (soundFX)
                soundFX.PlayAttackSound();
        }
            

        attacking = false;
        leaping = false;
    }


    IEnumerator StartAttack()
    {
        float timer = 0;
        while (timer < attackDelay)
        {
            timer += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        //only damage if facing player
        float angleToPlayer = Vector3.Angle(transform.forward, player.transform.position - transform.position);
        if (angleToPlayer < maxAttackAngle)
            DamagePlayer(player);

        //play sound
        if (soundFX)
            soundFX.PlayAttackSound();

        attacking = false;
    }

    void DamagePlayer(Player _player)
    {
        _player.DamageHp(damage);
    }

    public void SetPlayer(Player _player)
    {
        player = _player;
    }

    public void SetTriggered(bool _triggered)
    {
        triggered = _triggered;
    }
}
