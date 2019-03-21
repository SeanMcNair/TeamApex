using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class InteractFX : ScriptableObject
{
    public abstract void DoFX(GameObject _sender = null, GameObject _receiver = null);
}
