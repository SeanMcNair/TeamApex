using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BoolWrapper
{
    public bool Value { get; set; }
    public BoolWrapper(bool value) { Value = value; }
}

[System.Serializable]
public class FloatWrapper
{
    public float Value { get; set; }
    public FloatWrapper(float value) { Value = value; }
}

[System.Serializable]
public class IntWrapper
{
    public int Value { get; set; }
    public IntWrapper(int value) { Value = value; }
}

[System.Serializable]
public class RectWrapper
{
    public Rect Value { get; set; }
    public RectWrapper(Rect value) { Value = value; }
}

[System.Serializable]
public class TagProperty
{
    public string stringValue;
}

[System.Serializable]
public class LayerProperty
{
    public int indexValue;
    public string stringValue;
    public LayerMask maskValue;
}

[System.Serializable]
public class ChildName
{
    public bool overrideParent;
    public string overridePropertyName;
    public GameObject parent;
    public string stringValue;
    public int indexValue;
    public Transform transformValue { get { return parent.transform.FindDeepChild(stringValue); } }
}

[System.Serializable]
public class AnimatorStateProperty
{
    public string stateToPlay;
    public float crossfadeTime;
    public string exitState;
}

[System.Serializable]
public class AnimatorParamStateInfo
{
    public int indexValue;
    public string stringValue;
    public int layer;
}

[System.Serializable]
public class IndexStringProperty
{
    public int indexValue;
    public string stringValue;
    public string[] stringValues;
}

[System.Serializable]
public class Spawner
{
    public Spawnable spawn;
    public float repeatTime;

    public IEnumerator StartSpawning(Transform _trans)
    {
        while (true)
        {
            yield return new WaitForSeconds(repeatTime);
            SpawnPool.Spawn(spawn.poolIndex, _trans.position, Quaternion.identity);
        }
    }
}

[System.Serializable]
public class TestProperty
{
    public MethodProperty method;
}



