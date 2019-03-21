using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnerTest : MonoBehaviour
{
    [SerializeField] private Spawner[] spawners;

    [SerializeField] private InputProperty input;

	// Use this for initialization
	void Start ()
    {
        foreach (var spawn in spawners)
        {
            StartCoroutine(spawn.StartSpawning(transform));
        }
	}

    public void Spawn()
    {

    }
}
