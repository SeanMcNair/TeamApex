using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ItemWeaponMelee))]
public class ItemWeaponMeleeEditor : ItemEditor
{

    protected SerializedProperty detectZone;


    public override void GetProperties()
    {
        base.GetProperties();
        detectZone = sourceRef.FindProperty("detectZone");
    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.PropertyField(detectZone);

    }

    private void OnSceneGUI()
    {
        var zone = detectZone.GetRootValue<DetectZone>();
        zone.DrawDetectZone(source, sourceRef, source.transform);
    }

}
