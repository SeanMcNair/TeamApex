using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(Player))]
public class PlayerEditor : UnitEditor
{

    //health
    private SerializedProperty setData;
    private SerializedProperty userMode;
    private SerializedProperty userDataManager;
    private SerializedProperty user;

    public override void GetProperties()
    {
        base.GetProperties();
        setData = sourceRef.FindProperty("setData");
        userMode = sourceRef.FindProperty("userMode");
        userDataManager = sourceRef.FindProperty("userDataManager");
        user = sourceRef.FindProperty("user");
    }

    protected override void DisplayDataProperties()
    {
        EditorGUILayout.PropertyField(setData);
        if (setData.boolValue)
        {
            base.DisplayDataProperties();
        }
        else
        {
            EditorGUILayout.PropertyField(userMode);
            if (userMode.enumValueIndex == 2)
            {
                EditorGUILayout.PropertyField(userDataManager);

                var man = userDataManager.GetRootValue<UserDataManager>();
                if (man != null)
                {
                    user.IndexStringPropertyField(man.GetUserNames());
                }
                else
                    EditorExtensions.LabelFieldCustom("Need User Data Manager!", Color.red, FontStyle.Bold);
            }

        }

    }

}
