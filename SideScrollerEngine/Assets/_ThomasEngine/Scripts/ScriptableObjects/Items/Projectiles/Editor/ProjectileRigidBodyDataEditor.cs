using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ProjectileRigidBodyData))]
public class ProjectileRigidBodyDataEditor : ProjectileDataEditor
{
    public override void OnEnable()
    {
        base.OnEnable();
        source = (ProjectileRigidBodyData)target;
        source.linkedType = typeof(ProjectileRigidBody);
    }

    public override void GetProperties()
    {
        base.GetProperties();
    }

    public override void SetProperties()
    {
        base.SetProperties();
    }

}
