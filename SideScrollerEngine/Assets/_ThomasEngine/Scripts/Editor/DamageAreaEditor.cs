using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor (typeof (DamageArea))]
public class DamageAreaEditor : Editor
{
    private DamageArea source;
    private SerializedObject sourceRef;

    private SerializedProperty damageMask;

    private GUIStyle boldStyle = new GUIStyle ();

    private void OnEnable ()
    {
        source = (DamageArea) target;
        sourceRef = serializedObject;

        //set styles
        boldStyle.fontStyle = FontStyle.Bold;

        ResetAreYouSures ();
        GetProperties ();
    }

    public override void OnInspectorGUI ()
    {
        SetProperties ();

        sourceRef.ApplyModifiedProperties ();
    }

    private void OnSceneGUI ()
    {
        if (source.zones.Count < 1)
            return;

        DisplayDamageZonesSceneGUI ();

        SceneView.RepaintAll ();
    }

    void GetProperties ()
    {
        //damage
        damageMask = sourceRef.FindProperty ("damageMask");
    }

    void SetProperties ()
    {
        EditorGUILayout.Space ();
        EditorGUILayout.PropertyField (damageMask);

        ShowAddButton ();
        DisplayDamageZonesInspector ();

    }

    void ShowAddButton ()
    {
        EditorGUILayout.Space ();
        EditorGUILayout.BeginHorizontal ();

        EditorGUILayout.LabelField ("Add Damage Zone");

        if (GUILayout.Button ("+"))
            AddDamageZone ();

        EditorGUILayout.EndHorizontal ();
    }

    void ShowRemoveButton (int _ind)
    {
        EditorGUILayout.BeginHorizontal ();

        if (!source.zones[_ind].areYouSureActive)
        {
            EditorGUILayout.LabelField ("Remove Damage Zone");
            if (GUILayout.Button ("-"))
                source.zones[_ind].areYouSureActive = true;
        }
        else
        {
            EditorGUILayout.LabelField ("Are You Sure?", boldStyle);
            if (GUILayout.Button ("Yes"))
                RemoveDamageZone (_ind);
            if (GUILayout.Button ("No"))
                source.zones[_ind].areYouSureActive = false;
        }

        EditorGUILayout.EndHorizontal ();
    }

    void DisplayDamageZonesInspector ()
    {
        //display zones using the public values
        for (int i = 0; i < source.zones.Count; i++)
        {
            EditorGUILayout.Space ();
            //foldout bool
            source.zones[i].opened = EditorGUILayout.Foldout (source.zones[i].opened, (i + 1) + ". " + source.zones[i].zoneName);
            if (source.zones[i].opened)
            {
                source.zones[i].activeOnStart = EditorGUILayout.Toggle ("Active On Start?", source.zones[i].activeOnStart);
                source.zones[i].zoneName = EditorGUILayout.TextField ("Zone Name", source.zones[i].zoneName);
                source.zones[i].zoneType = (DamageArea.DamageZone.ZoneType) EditorGUILayout.EnumPopup ("Zone Type", source.zones[i].zoneType);

                //switch based on center type
                source.zones[i].centerType = (DamageArea.DamageZone.CenterType) EditorGUILayout.EnumPopup ("Center Type", source.zones[i].centerType);
                if (source.zones[i].centerType == DamageArea.DamageZone.CenterType.Offset)
                    source.zones[i].center = EditorGUILayout.Vector2Field ("Center Offset", source.zones[i].center);
                else if (source.zones[i].centerType == DamageArea.DamageZone.CenterType.Transform)
                    source.zones[i].transPoint = (Transform) EditorGUILayout.ObjectField ("Transform Point", source.zones[i].transPoint, typeof (Transform), true);

                //switch based on zone type enum selection...
                if (source.zones[i].zoneType == DamageArea.DamageZone.ZoneType.Box)
                {
                    source.zones[i].boxSize = EditorGUILayout.Vector2Field ("Box Size", source.zones[i].boxSize);
                    if (source.zones[i].centerType == DamageArea.DamageZone.CenterType.Transform)
                    {
                        source.zones[i].angleType = (DamageArea.DamageZone.AngleType)EditorGUILayout.EnumPopup("Angle Type", source.zones[i].angleType);
                        if (source.zones[i].angleType != DamageArea.DamageZone.AngleType.TransformZ)
                            source.zones[i].angle = EditorGUILayout.FloatField("Angle", source.zones[i].angle);
                        else if (!Application.isPlaying && source.zones[i].transPoint)
                            source.zones[i].angle = source.zones[i].transPoint.eulerAngles.z;
                    }
                    else
                        source.zones[i].angle = EditorGUILayout.FloatField ("Angle", source.zones[i].angle);
                }
                else
                    source.zones[i].radius = EditorGUILayout.FloatField ("Radius", source.zones[i].radius);

                //damage params setup
                source.zones[i].damageAmount = EditorGUILayout.IntField ("Damage Amount", source.zones[i].damageAmount);
                source.zones[i].bounceForce = EditorGUILayout.FloatField ("Bounce Force", source.zones[i].bounceForce);
                source.zones[i].bounceSetup = (DamageArea.DamageZone.BounceDirectionSetup) EditorGUILayout.EnumPopup ("Bounce Direction", source.zones[i].bounceSetup);
                if (source.zones[i].bounceSetup == DamageArea.DamageZone.BounceDirectionSetup.Override)
                    source.zones[i].bounceDir = EditorGUILayout.Vector2Field ("Bounce Direction", source.zones[i].bounceDir);

                //set zonename to custom ONLY if the user has put input...otherwise just the zonetype names
                if (source.zones[i].zoneType != source.zones[i].lastType)
                {
                    if (source.zones[i].zoneName == DamageArea.DamageZone.ZoneType.Box.ToString ())
                        source.zones[i].zoneName = DamageArea.DamageZone.ZoneType.Circle.ToString ();
                    else if (source.zones[i].zoneName == DamageArea.DamageZone.ZoneType.Circle.ToString ())
                        source.zones[i].zoneName = DamageArea.DamageZone.ZoneType.Box.ToString ();

                }
                source.zones[i].lastType = source.zones[i].zoneType;

                //remove button
                ShowRemoveButton (i);
            }

        }
    }

    void DisplayDamageZonesSceneGUI ()
    {
        for (int i = 0; i < source.zones.Count; i++)
        {

            //set color
            if (source.zones[i].active || !Application.isPlaying)
                Handles.color = Color.red;
            else
                Handles.color = Color.blue;

            //drag position handles
            DragPositionHandles (i);

            //draw zonetypes switch
            if (source.zones[i].zoneType == DamageArea.DamageZone.ZoneType.Box)
            {
                //need to create a new handles matrix space just for the box so we can rotate the "angle"...this space is the new Zero position.
                Matrix4x4 boxMatrix = Matrix4x4.TRS (source.zones[i].handlePoint, Quaternion.Euler (0, 0, source.zones[i].angle), Handles.matrix.lossyScale);
                using (new Handles.DrawingScope (boxMatrix))
                {
                    Handles.DrawWireCube (Vector2.zero, source.zones[i].boxSize);
                    source.zones[i].boxSize = Handles.ScaleHandle (source.zones[i].boxSize, Vector2.zero, Quaternion.identity, 1);
                }

            }
            else if (source.zones[i].zoneType == DamageArea.DamageZone.ZoneType.Circle)
                Handles.DrawWireDisc (source.zones[i].handlePoint, Vector3.back, source.zones[i].radius);

            //Draw Labels
            Handles.Label (source.zones[i].handlePoint, source.zones[i].zoneName, boldStyle);
        }

    }

    void DragPositionHandles (int _ind)
    {
        EditorGUI.BeginChangeCheck ();

        //set center
        if (source.zones[_ind].centerType == DamageArea.DamageZone.CenterType.Offset)
            SetHandleStartPosition (_ind, (Vector2) source.transform.position + source.zones[_ind].center);
        else if (source.zones[_ind].centerType == DamageArea.DamageZone.CenterType.Transform)
        {
            if (source.zones[_ind].transPoint)
            {
                SetHandleStartPosition (_ind, source.zones[_ind].transPoint.position);
                //label transform
                Handles.Label (source.zones[_ind].handlePoint + Vector2.down / 4, source.zones[_ind].transPoint.name + " position", boldStyle);
            }

        }

        //set handle position
        source.zones[_ind].handlePoint = Handles.PositionHandle (source.zones[_ind].handlePoint, Quaternion.identity);

        //set position of source trans after dragging
        if (EditorGUI.EndChangeCheck ())
        {
            Undo.RecordObject(source, "Modified " + source + " properties.");
            ApplyEditorValues (_ind);
        }

    }

    void ApplyEditorValues (int _ind)
    {
        if (Application.isPlaying) //only change these settings not in play mode
            return;

        //apply to these values
        source.zones[_ind].center = source.zones[_ind].handlePoint - (Vector2) source.transform.position;
        if (source.zones[_ind].transPoint)
            source.zones[_ind].transPoint.position = source.zones[_ind].handlePoint;

    }

    void SetHandleStartPosition (int _ind, Vector2 _pos)
    {
        //set handle position to source at the start
        if (source.zones[_ind].handlePoint != _pos)
            source.zones[_ind].handlePoint = _pos;
    }

    void ResetAreYouSures ()
    {
        foreach (var zone in source.zones)
        {
            zone.areYouSureActive = false;
        }
    }

    void AddDamageZone ()
    {
        source.zones.Add (new DamageArea.DamageZone ());
    }

    void RemoveDamageZone (int _ind)
    {
        source.zones.RemoveAt (_ind);
    }

}