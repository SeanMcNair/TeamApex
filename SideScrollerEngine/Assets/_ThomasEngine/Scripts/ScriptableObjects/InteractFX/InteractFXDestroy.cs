using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Destroy", menuName = "Data/Interacts/Destroy", order = 1)]
public class InteractFXDestroy : InteractFX
{
    public enum DestroyType { Sender, Receiver, ClosestTag, FindName }

    [SerializeField] private DestroyType destroyType;
    [SerializeField] private bool destroyRoot;
    [SerializeField] private float delay;
    [SerializeField] private TagProperty closestTag;
    [SerializeField] private string nameToFind;

    public override void DoFX(GameObject _sender = null, GameObject _receiver = null)
    {
        if (destroyType == DestroyType.Sender)
            KillObject(_sender);
        else if (destroyType == DestroyType.Receiver)
            KillObject(_receiver);
        else if (destroyType == DestroyType.ClosestTag)
        {
            var obj = Utils.FindClosestByTag(_sender.transform, closestTag.stringValue);
            KillObject(obj.gameObject);
        }
        else if (destroyType == DestroyType.FindName)
        {
            var obj = GameObject.Find(nameToFind);
            KillObject(obj);
        }
    }

    void KillObject(GameObject _go)
    {
        if (!_go)
            return;
        if (destroyRoot)
            Destroy(_go.transform.root.gameObject, delay);
        else
            Destroy(_go, delay);
    }


}
