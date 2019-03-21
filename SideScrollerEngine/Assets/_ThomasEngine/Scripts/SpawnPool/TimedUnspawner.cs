using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimedUnspawner : MonoBehaviour
{

    [SerializeField] private float time;

    private void OnEnable()
    {
        StartCoroutine(StartTime());
    }

    IEnumerator StartTime()
    {
        yield return new WaitForSeconds(time);
        SpawnPool.Unspawn(gameObject);
    }
}
