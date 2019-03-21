using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(FishController))]
public class FishControllerEditor : PlayerControllerEditor
{
    //Movement Properties
    private SerializedProperty fallSpeed;
    private SerializedProperty inflatedSpeed;
    private SerializedProperty inflatedColliderSize;
    private SerializedProperty animProp;
    private SerializedProperty holdToInflate;
    private SerializedProperty inflateTime;
    private SerializedProperty useAddedForce;
    private SerializedProperty consistentRiseForce;

    public override void GetProperties()
    {
        base.GetProperties();
        GetFishProperties();
    }

    public override void SetProperties()
    {
        DisplayFishProperties();
    }

    void GetFishProperties()
    {
        fallSpeed = sourceRef.FindProperty("fallSpeed");
        inflatedSpeed = sourceRef.FindProperty("inflatedSpeed");
        inflatedColliderSize = sourceRef.FindProperty("inflatedColliderSize");
        animProp = sourceRef.FindProperty("animProp");
        holdToInflate = sourceRef.FindProperty("holdToInflate");
        inflateTime = sourceRef.FindProperty("inflateTime");
        useAddedForce = sourceRef.FindProperty("useAddedForce");
        consistentRiseForce = sourceRef.FindProperty("consistentRiseForce");
    }

    void DisplayFishProperties()
    {
        EditorExtensions.PropertyFieldCustom(jumpButton, "Inflate Button");
        EditorGUILayout.PropertyField(fallSpeed);
        EditorGUILayout.PropertyField(inflatedSpeed);
        EditorGUILayout.PropertyField(inflatedColliderSize);
        EditorGUILayout.LabelField("Anim Prop controls crossfade time for both animation and inflating");
        EditorGUILayout.PropertyField(animProp, true);
        EditorGUILayout.PropertyField(holdToInflate);
        if (!holdToInflate.boolValue)
            EditorGUILayout.PropertyField(inflateTime);
        EditorGUILayout.PropertyField(useAddedForce);
        if (useAddedForce.boolValue)
        {
            EditorGUILayout.PropertyField(consistentRiseForce);
        }

    }

    

}
