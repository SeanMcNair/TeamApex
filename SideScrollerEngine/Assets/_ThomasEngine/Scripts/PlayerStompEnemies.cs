using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent (typeof (Rigidbody2D))]
[RequireComponent (typeof (PlayerController))]
[RequireComponent (typeof (Player))]
public class PlayerStompEnemies : MonoBehaviour
{

    [SerializeField]
    private int damage = 1;

    //instant kill
    [SerializeField]
    private bool instantKill;
    [SerializeField]
    private string instantKillTag;

    //detection
    [SerializeField]
    private LayerMask stompMask;
    [SerializeField]
    private Vector2 detectBoxSize = Vector2.one;
    [SerializeField]
    private Vector2 detectBoxCenter = Vector2.zero;

    //bouncing
    [SerializeField]
    private float bounceForce = 10;

    private Rigidbody2D rb;
    private PlayerController con;
    private Player pl;
    private Enemy curEnemy;

    private Collider2D[] cols;
    private List<Collider2D> damCols = new List<Collider2D> ();
    private List<Collider2D> damaged = new List<Collider2D> ();
    private Collider2D[] lastCols;

    // Use this for initialization
    void Start ()
    {
        rb = GetComponent<Rigidbody2D> ();
        con = GetComponent<PlayerController> ();
        pl = GetComponent<Player> ();

    }

    // Update is called once per frame
    void FixedUpdate ()
    {
        DoStompDetection ();
    }

    void DoStompDetection ()
    {
        if (rb.velocity.y >= 0 || pl.IsPhysicsIgnored) //only do damage if moving downward
            return;

        Vector2 pos = transform.position;
        cols = Physics2D.OverlapBoxAll (pos + detectBoxCenter, detectBoxSize, 0, stompMask);
        //We do some loop checks to see if the colliders are triggers or not. We only apply damage to non-triggers.
        if (cols.Length > 0)
        {
            //only continue if the collision list changed
            if (cols == lastCols)
                return;

            //create non-trigger array
            damCols = new List<Collider2D> ();
            for (int i = 0; i < cols.Length; i++)
            {
                if (!cols[i].isTrigger)
                    damCols.Add (cols[i]);
            }

            if (damCols.Count > 0)
            {
                //damage the non-trigger enemy
                for (int i = 0; i < damCols.Count; i++)
                {
                    //add damaged enemy to list so we only damage once 
                    if (!damaged.Contains (damCols[i]))
                    {
                        curEnemy = damCols[i].GetComponent<Enemy> ();
                        if (curEnemy)
                        {
                            damaged.Add (damCols[i]);
                            DoDamage (curEnemy);
                            Bounce ();
                        }
                    }
                }
            }
            else if (damaged.Count > 0)
                damaged.Clear ();
                
            lastCols = cols;
        }
        else if (lastCols != null || damaged.Count > 0)
        {
            lastCols = null;
            damaged.Clear ();
        }

    }

    void DoDamage (Enemy _enemy)
    {
        if (instantKill && instantKillTag == _enemy.tag)
            _enemy.DamageHp (_enemy.CurHP);
        else
            _enemy.DamageHp (damage);
    }

    void Bounce ()
    {
        con.Bounce (Vector2.up, bounceForce);
    }
    
}