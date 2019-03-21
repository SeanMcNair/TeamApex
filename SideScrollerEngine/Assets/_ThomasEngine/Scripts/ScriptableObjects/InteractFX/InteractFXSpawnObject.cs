using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "SpawnObject", menuName = "Data/Interacts/SpawnObject", order = 1)]
public class InteractFXSpawnObject : InteractFX
{
    public enum SetTransformType { None, Sender, Receiver}
    [SerializeField] private GameObject objectToSpawn;
    [SerializeField] private SetTransformType setParent;
    [SerializeField] private SetTransformType setPosAndRot;

    private Transform spawn;

    public override void DoFX(GameObject _sender, GameObject _receiver)
    {
        spawn = Instantiate(objectToSpawn).transform;
        if (setParent != SetTransformType.None)
        {
            if (setParent == SetTransformType.Receiver)
                spawn.SetParent(_receiver.transform);
            else
                spawn.SetParent(_sender.transform);

        }
        if (setPosAndRot != SetTransformType.None)
        {
            if (setPosAndRot == SetTransformType.Receiver)
                SetPositionAndRotation(_receiver.transform);
            else
                SetPositionAndRotation(_sender.transform);
        }
    }

    void SetPositionAndRotation(Transform _trans)
    {
        spawn.position = _trans.position;
        spawn.rotation = _trans.rotation;
    }
}
