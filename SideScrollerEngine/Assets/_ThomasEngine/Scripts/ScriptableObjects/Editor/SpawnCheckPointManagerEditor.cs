using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Reflection;

[CanEditMultipleObjects]
[CustomEditor(typeof(SpawnCheckPointManager))]
public class SpawnCheckPointManagerEditor : Editor
{

    private SpawnCheckPointManager source;
    private SerializedObject sourceRef;

    private SerializedProperty overrideCheckPoint;
    private SerializedProperty checkPoint;
    private SerializedProperty detectMask;
    private SerializedProperty checkPoints;
    //player stuff
    private SerializedProperty playerSpawn;
    //saving stuff
    private SerializedProperty progressOnly;
    private SerializedProperty saveProgressToDisc;
    private SerializedProperty resetProgressOnQuit;
    //level finish
    private SerializedProperty sceneUnlocked;
    private SerializedProperty nextSceneToPlay;
    private SerializedProperty freezeGame;
    private SerializedProperty freezePlayer;
    private SerializedProperty endTime;

    private DetectZone[] points;

    private float furthest;
    private Vector2 furthestPoint1;
    private Vector2 furthestPoint2;

    private void OnEnable()
    {
        source = (SpawnCheckPointManager)target;
        sourceRef = serializedObject;

        SceneView.onSceneGUIDelegate += OnSceneGUI;

        GetProperties();
        GetFurthestCheckPointDistance();
        PositionCamera();
    }

    private void OnDisable()
    {
        SceneView.onSceneGUIDelegate -= OnSceneGUI;
    }

    public override void OnInspectorGUI()
    {
        SetProperties();
        SetCheckPointNames();

        sourceRef.ApplyModifiedProperties();
    }

    void GetProperties()
    {
        overrideCheckPoint = sourceRef.FindProperty("overrideCheckPoint");
        checkPoint = sourceRef.FindProperty("checkPoint");
        detectMask = sourceRef.FindProperty("detectMask");
        checkPoints = sourceRef.FindProperty("checkPoints");

        //playerstuff
        playerSpawn = sourceRef.FindProperty("playerSpawn");

        //saving
        progressOnly = sourceRef.FindProperty("progressOnly");
        saveProgressToDisc = sourceRef.FindProperty("saveProgressToDisc");
        resetProgressOnQuit = sourceRef.FindProperty("resetProgressOnQuit");

        //level finish
        sceneUnlocked = sourceRef.FindProperty("sceneUnlocked");
        nextSceneToPlay = sourceRef.FindProperty("nextSceneToPlay");
        freezeGame = sourceRef.FindProperty("freezeGame");
        freezePlayer = sourceRef.FindProperty("freezePlayer");
        endTime = sourceRef.FindProperty("endTime");
    }

    void SetProperties()
    {
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.LabelField("CheckPoint Options");
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.PropertyField(overrideCheckPoint);
        if (overrideCheckPoint.boolValue)
            EditorGUILayout.PropertyField(checkPoint);
        EditorGUILayout.PropertyField(detectMask);
        var mask = detectMask.GetRootValue<LayerMask>();
        checkPoints.arraySize = Mathf.Clamp(checkPoints.arraySize, 2, int.MaxValue);
        for (int i = 0; i < checkPoints.arraySize; i++)
        {
            var point = checkPoints.GetArrayElementAtIndex(i);
            var overrideDetectMask = point.FindPropertyRelative("overrideDetectMask");
            var overridePositionType = point.FindPropertyRelative("overridePositionType");
            var overrideDetectType = point.FindPropertyRelative("overrideDetectType");
            
            overrideDetectMask.boolValue = true;
            overridePositionType.boolValue = true;
            overrideDetectType.boolValue = true;
        }
        points = checkPoints.GetRootValue<DetectZone[]>();
        if (points != null)
        {
            foreach (var point in points)
            {
                point.detectMask = mask;
                point.positionType = DetectZone.PositionType.World;
                point.detectType = DetectZone.DetectAreaType.Box;
            }
        }
        EditorGUILayout.PropertyField(checkPoints, true);
        
        //playerstuff
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.LabelField("Player Options");
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.PropertyField(playerSpawn);
        //saving
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.LabelField("Data Options");
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.PropertyField(progressOnly);
        EditorGUILayout.PropertyField(saveProgressToDisc);
        EditorGUILayout.PropertyField(resetProgressOnQuit);
        //level finish
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.LabelField("On Level Finish");
        EditorGUILayout.LabelField("------------------------");
        EditorGUILayout.PropertyField(sceneUnlocked);
        EditorGUILayout.PropertyField(nextSceneToPlay);
        EditorGUILayout.PropertyField(freezeGame);
        EditorGUILayout.PropertyField(freezePlayer);
        EditorGUILayout.PropertyField(endTime);

    }

    void SetCheckPointNames()
    {
        for (int i = 0; i < checkPoints.arraySize; i++)
        {
            var prop = checkPoints.GetArrayElementAtIndex(i);
            var pointName = prop.FindPropertyRelative("zoneName");
            pointName.stringValue = "Checkpoint " + i;
        }
    }

    private void OnSceneGUI(SceneView sceneView)
    {
        DrawCheckPoints();
        SceneView.RepaintAll();
    }

    void DrawCheckPoints()
    {
        if (points == null)
            return;
        if (points.Length < 1)
            return;
        for (int i = 0; i < points.Length; i++)
        {
            var zone = points[i];
            var pointName = zone.zoneName;

            zone.DrawDetectZone(source, sourceRef);

            //drawlabel
            var style = new GUIStyle
            {
                fontStyle = FontStyle.Bold,
                alignment = TextAnchor.LowerCenter,
                normal = new GUIStyleState
                {
                    textColor = Color.black,
                }
            };
            Handles.Label(zone.worldPos + zone.offset, pointName, style);
        }

    }

    void PositionCamera()
    {
        if (furthest < 10)
            return;

        var sceneview = SceneView.lastActiveSceneView;
        if (sceneview != null)
        {
            sceneview.orthographic = true;
            sceneview.size = furthest;
            sceneview.rotation = Quaternion.LookRotation(Vector3.zero);
            var center = Vector2.Lerp(furthestPoint1, furthestPoint2, 0.5f);
            sceneview.pivot = center;
        }

    }

    private void GetFurthestCheckPointDistance()
    {
        furthest = 0;
        var checks = checkPoints.GetRootValue<DetectZone[]>();
        if (checks != null)
        {
            if (checks.Length > 0)
            {
                foreach (var check in checks)
                {
                    foreach (var otherCheck in checks)
                    {
                        var dist = Vector2.Distance(check.worldPos, otherCheck.worldPos);
                        if (dist > furthest)
                        {
                            furthest = dist;
                            furthestPoint1 = check.worldPos;
                            furthestPoint2 = otherCheck.worldPos;
                        }

                    }
                }
            }

        }

    }



}
