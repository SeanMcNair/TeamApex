using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Linkable : MonoBehaviour
{
    [SerializeField] protected LinkableData data;
    public virtual LinkableData Data { get { return data; } set { data = value; } }
}
