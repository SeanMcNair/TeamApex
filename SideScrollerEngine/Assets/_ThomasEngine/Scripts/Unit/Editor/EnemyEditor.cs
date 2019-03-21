using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(Enemy))]
public class EnemyEditor : UnitEditor
{ 

    public override void GetProperties()
    {
        base.GetProperties();
    }

    protected override void SetProperties()
    {
        base.SetProperties();
    }
}
