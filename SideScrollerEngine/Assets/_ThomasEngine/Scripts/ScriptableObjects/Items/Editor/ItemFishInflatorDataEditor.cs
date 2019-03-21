using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemFishInflatorData))]
public class ItemFishInflatorDataEditor : ItemUseableDataEditor
{
    public SerializedProperty drainOnHPHit;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemFishInflatorData)target;
        source.linkedType = typeof(ItemFishInflator);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        drainOnHPHit = sourceRef.FindProperty("drainOnHPHit");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Fish Inflator Properties",boldStyle);
        EditorGUILayout.PropertyField(drainOnHPHit);

    }

}
