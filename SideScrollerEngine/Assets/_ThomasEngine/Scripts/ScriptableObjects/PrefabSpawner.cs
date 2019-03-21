using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

[CreateAssetMenu(fileName = "Prefab Spawner", menuName = "Utilities/Prefab Spawner", order = 1)]
public class PrefabSpawner : ScriptableObject
{
    [SerializeField] private GameObject[] prefabsToSpawn;

    public GameObject spawnedPrefab;

    //the prefabs are spawned through the editor script

}
