using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(ItemAimableData))]
public class ItemAimableDataEditor : ItemUseableDataEditor
{

    private SerializedProperty aimDistance;
    private SerializedProperty minAimDistance;
    private SerializedProperty disableAimBehindMuzzle;
    private SerializedProperty muzzlePos;
    private SerializedProperty muzzlePosInd;
    private SerializedProperty aimFX;

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ItemAimableData)target;
        source.linkedType = typeof(ItemAimable);
    }

    public override void GetProperties()
    {
        base.GetProperties();
        minAimDistance = sourceRef.FindProperty("minAimDistance");
        aimDistance = sourceRef.FindProperty("aimDistance");
        disableAimBehindMuzzle = sourceRef.FindProperty("disableAimBehindMuzzle");
        muzzlePos = sourceRef.FindProperty("muzzlePos");
        muzzlePosInd = sourceRef.FindProperty("muzzlePosInd");
        aimFX = sourceRef.FindProperty("aimFX");

    }

    public override void SetProperties()
    {
        base.SetProperties();
        EditorGUILayout.LabelField("Aim Properties", boldStyle);
        EditorGUILayout.PropertyField(minAimDistance);
        EditorGUILayout.PropertyField(aimDistance);
        EditorGUILayout.PropertyField(disableAimBehindMuzzle);
        EditorGUILayout.PropertyField(aimFX);
        EditorExtensions.DisplayAllChildrenPopup("Muzzle Pos", connectedPrefab, muzzlePosInd, muzzlePos);
    }

}
