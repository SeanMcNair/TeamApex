using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (EnemyMoveTowards))]
public class EnemyMoveTowardsEditor : UnitMoveTowardsEditor
{
    private SerializedProperty useItem;
    private SerializedProperty repeatIfNotUsing;

    public override void GetProperties ()
    {
        base.GetProperties();
        useItem = sourceRef.FindProperty ("useItem");
        repeatIfNotUsing = sourceRef.FindProperty ("repeatIfNotUsing");
    }

    public override void DisplayArrivalProperties()
    {
        base.DisplayArrivalProperties();
        EditorGUILayout.PropertyField(useItem);
        EditorGUILayout.PropertyField(repeatIfNotUsing);
    }

}