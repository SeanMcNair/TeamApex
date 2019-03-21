using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;

[CustomEditor(typeof(ProjectileBulletData))]
public class ProjectileBulletDataEditor : ProjectileDataEditor
{

    public override void OnEnable()
    {
        base.OnEnable();
        source = (ProjectileBulletData)target;
        source.linkedType = typeof(ProjectileBullet);
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
