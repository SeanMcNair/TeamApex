using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

[CanEditMultipleObjects]
[CustomEditor(typeof(PlayerAnimations))]
public class PlayerAnimationsEditor : UnitAnimationsEditor
{
    private SerializedProperty animInputHorizontal;
    private SerializedProperty animInputVertical;
    private SerializedProperty animDoubleJump;
    private SerializedProperty animCrouch;
    private SerializedProperty animClimbing;
    private SerializedProperty animRunning;
    private SerializedProperty animBackwards;
    private SerializedProperty animWallHitLeft;
    private SerializedProperty animWallHitRight;

    public override void GetProperties()
    {
        base.GetProperties();
        animInputHorizontal = sourceRef.FindProperty("animInputHorizontal");
        animInputVertical = sourceRef.FindProperty("animInputVertical");
        animDoubleJump = sourceRef.FindProperty("animDoubleJump");
        animCrouch = sourceRef.FindProperty("animCrouch");
        animRunning = sourceRef.FindProperty("animRunning");
        animBackwards = sourceRef.FindProperty("animBackwards");
        animClimbing = sourceRef.FindProperty("animClimbing");
        animWallHitLeft = sourceRef.FindProperty("animWallHitLeft");
        animWallHitRight = sourceRef.FindProperty("animWallHitRight");
    }

    public override void DisplaySyncedParameters()
    {
        base.DisplaySyncedParameters();
        animInputHorizontal.IndexStringField(parameters);
        animInputVertical.IndexStringField(parameters);
        animCrouch.IndexStringField(parameters);
        animClimbing.IndexStringField(parameters);
        animRunning.IndexStringField(parameters);
        animBackwards.IndexStringField(parameters);
        animWallHitLeft.IndexStringField(parameters);
        animWallHitRight.IndexStringField(parameters);
    }

    public override void DisplayMovementParameters()
    {
        base.DisplayMovementParameters();
        IndexStringField(animDoubleJump);
    }


}
