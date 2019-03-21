using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTrigger : MonoBehaviour 
{

    [SerializeField]
    private EnemyMelee enemyMelee;

    void OnTriggerEnter2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            if (enemyMelee)
            {
                enemyMelee.SetPlayer(_col.GetComponent<Player>());
                enemyMelee.SetTriggered(true);
            }
            
        }
    }

    void OnTriggerExit2D(Collider2D _col)
    {
        if (_col.tag == "Player")
        {
            if (enemyMelee)
                enemyMelee.SetTriggered(false);
        }
    }
}
