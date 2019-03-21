using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMoveFlee : MonoBehaviour
{
    [SerializeField]
    private GameObject enemyGO;
    [SerializeField]
    private float speed = 3;
    [SerializeField]
    private float maxFleeDistance = 10;
    [SerializeField]
    private bool ignoreZTurning = true;
    [SerializeField]
    private bool ignoreTargetYPos = true;

    private bool stoppedFleeing;
    public bool HasStoppedFleeing() { return stoppedFleeing; }

    private Enemy en;

    private void Start()
    {
        en = enemyGO.GetComponent<Enemy>();
    }

    public void FleeFromTarget (GameObject _target)
    {
        StopAllCoroutines ();
        StartCoroutine (StartFleeFromTarget (_target));
    }

    IEnumerator StartFleeFromTarget (GameObject _target)
    {
        stoppedFleeing = false;
        float distance = 0;
        while (distance < maxFleeDistance)
        {
            if (!en.IsStunned)
            {
                distance = Vector2.Distance(_target.transform.position, enemyGO.transform.position);

                if (ignoreTargetYPos)
                    enemyGO.transform.Translate(enemyGO.transform.right * speed * Time.deltaTime, Space.World);
                else
                {
                    Vector2 dir = (enemyGO.transform.position - _target.transform.position).normalized;//
                    enemyGO.transform.Translate(dir * speed * Time.deltaTime, Space.World);
                }

                if (ignoreZTurning)
                    enemyGO.transform.right = -(new Vector3(_target.transform.position.x, enemyGO.transform.position.y) - enemyGO.transform.position);
                else
                    enemyGO.transform.right = -(_target.transform.position - enemyGO.transform.position);
            }
            
            yield return new WaitForFixedUpdate ();
        }
        if (distance > maxFleeDistance)
            stoppedFleeing = true;

    }

    public void StopFleeing()
    {
        StopAllCoroutines();
    }
}