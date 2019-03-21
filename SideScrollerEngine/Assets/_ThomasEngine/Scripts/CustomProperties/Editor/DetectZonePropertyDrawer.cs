using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer (typeof (DetectZone))]
public class DetectZonePropertyDrawer : PropertyDrawer
{
    private SerializedProperty sourceRef;

    private SerializedProperty overrideZoneName;
    private SerializedProperty zoneName;
    private SerializedProperty overrideDetectMask;
    private SerializedProperty detectMask;
    private SerializedProperty overrideDetectType;
    private SerializedProperty detectType;
    private SerializedProperty overridePositionType;
    private SerializedProperty positionType;
    private SerializedProperty trans;
    private SerializedProperty worldPos;
    private SerializedProperty offset;
    private SerializedProperty size;
    private SerializedProperty useTransformZAngle;
    private SerializedProperty angle;
    private SerializedProperty radius;
    private SerializedProperty debugColor;

    //need to set field amount manually if you add more fields
    private int fieldAmount = 7;
    private float fieldSize = 16;
    private float padding = 2;

    private bool expanded;

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        sourceRef = property;
        GetProperties();
            //set the height of the drawer by the field size and padding
            return (fieldSize * fieldAmount) + (padding * fieldAmount);
    }

    public virtual void GetProperties()
    {
        //get property values
        overrideZoneName = sourceRef.FindPropertyRelative("overrideZoneName");
        zoneName = sourceRef.FindPropertyRelative("zoneName");
        overrideDetectMask = sourceRef.FindPropertyRelative("overrideDetectMask");
        overrideDetectMask = sourceRef.FindPropertyRelative("overrideDetectMask");
        detectMask = sourceRef.FindPropertyRelative("detectMask");
        overrideDetectType = sourceRef.FindPropertyRelative("overrideDetectType");
        detectType = sourceRef.FindPropertyRelative("detectType");
        overridePositionType = sourceRef.FindPropertyRelative("overridePositionType");
        positionType = sourceRef.FindPropertyRelative("positionType");
        trans = sourceRef.FindPropertyRelative("trans");
        worldPos = sourceRef.FindPropertyRelative("worldPos");
        offset = sourceRef.FindPropertyRelative("offset");
        size = sourceRef.FindPropertyRelative("size");
        useTransformZAngle = sourceRef.FindPropertyRelative("useTransformZAngle");
        angle = sourceRef.FindPropertyRelative("angle");
        radius = sourceRef.FindPropertyRelative("radius");
        debugColor = sourceRef.FindPropertyRelative("debugColor");
    }


    // Draw the property inside the given rect
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // Using BeginProperty / EndProperty on the parent property means that
        // prefab override logic works on the entire property.
        EditorGUI.BeginProperty(position, label, property);

        //divide all field heights by the field amount..then minus the padding
        position.height /= fieldAmount; position.height -= padding;

        // Draw Prefix label...this will push all other content to the right
        //position = EditorGUI.PrefixLabel(position, GUIUtility.GetControlID(FocusType.Passive), label);

        // Draw non-indented label instead
        EditorGUI.LabelField(position, property.displayName);

        // Get the start indent level
        var indent = EditorGUI.indentLevel;
        // Set indent amount
        EditorGUI.indentLevel = indent + 1;

        DisplayGUIElements(position, property, label);

        // Set indent back to what it was
        EditorGUI.indentLevel = indent;

        EditorGUI.EndProperty();
    }

    public virtual void DisplayGUIElements(Rect position, SerializedProperty property, GUIContent label)
    {
        fieldAmount = 1;

        if (!overrideZoneName.boolValue)
        {
            fieldAmount++;
            //offset position.y by field size
            position.y += fieldSize + padding;
            //first field
            EditorGUI.PropertyField(position, zoneName);
        }

        if (!overrideDetectMask.boolValue)
        {
            fieldAmount++;
            //offset position.y by field size
            position.y += fieldSize + padding;
            //first field
            EditorGUI.PropertyField(position, detectMask);
        }
        
        if (!overrideDetectType.boolValue)
        {
            //detect type
            fieldAmount++;
            position.y += fieldSize + padding;
            EditorGUI.PropertyField(position, detectType);
        }
        
        if (!overridePositionType.boolValue)
        {
            //position type
            fieldAmount++;
            position.y += fieldSize + padding;
            EditorGUI.PropertyField(position, positionType);
        }
        
        //Local position type
        if (positionType.enumValueIndex == 1)
        {
            fieldAmount++;
            position.y += fieldSize + padding;
            EditorGUI.PropertyField(position, trans);
        }
        //worldPos
        else if (positionType.enumValueIndex == 2)
        {
            fieldAmount++;
            position.y += fieldSize + padding;
            EditorGUI.PropertyField(position, worldPos);
        }

        //offset
        fieldAmount++;
        position.y += fieldSize + padding;
        EditorGUI.PropertyField(position, offset);

        //if box
        if (detectType.enumValueIndex == 1)
        {
            fieldAmount++;
            position.y += fieldSize + padding;
            EditorGUI.PropertyField(position, size);

            if (positionType.enumValueIndex != 2)
            {
                fieldAmount++;
                position.y += fieldSize + padding;
                EditorGUI.PropertyField(position, useTransformZAngle);
            }
            else
                useTransformZAngle.boolValue = false;


            if (!useTransformZAngle.boolValue)
            {
                fieldAmount++;
                position.y += fieldSize + padding;
                EditorGUI.PropertyField(position, angle);
            }
        }
        //circle
        else if (detectType.enumValueIndex == 0)
        {
            fieldAmount++;
            position.y += fieldSize + padding;
            EditorGUI.PropertyField(position, radius);
        }

        if (detectType.enumValueIndex == 0 || detectType.enumValueIndex == 1)
        {
            //color field
            fieldAmount++;
            position.y += fieldSize + padding;
            EditorGUI.PropertyField(position, debugColor);
        }
        
    }

}