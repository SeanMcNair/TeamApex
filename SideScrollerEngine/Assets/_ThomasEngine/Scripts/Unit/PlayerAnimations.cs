using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimations : UnitAnimations
{
    //synced params
    [SerializeField] protected AnimatorParamStateInfo animInputHorizontal;
    public float InputHorizontal { set { if (anim && !forceIdle) anim.SetFloat(animInputHorizontal.stringValue, Mathf.Abs(value)); } }
    [SerializeField] protected AnimatorParamStateInfo animInputVertical;
    public float InputVertical { set { if (anim && !forceIdle) anim.SetFloat(animInputVertical.stringValue, value); } }
    [SerializeField] protected AnimatorParamStateInfo animWallHitLeft;
    public bool WallHitLeft { set { if (anim && !forceIdle) anim.SetBool(animWallHitLeft.stringValue, value); } }
    [SerializeField] protected AnimatorParamStateInfo animWallHitRight;
    public bool WallHitRight { set { if (anim && !forceIdle) anim.SetBool(animWallHitRight.stringValue, value); } }
    [SerializeField] protected AnimatorParamStateInfo animCrouch;
    public bool Crouched { set { if (anim && !forceIdle) anim.SetBool(animCrouch.stringValue, value); } }
    [SerializeField] protected AnimatorParamStateInfo animClimbing;
    public bool Climbing { set { if (anim && !forceIdle) anim.SetBool(animClimbing.stringValue, value); } }
    [SerializeField] protected AnimatorParamStateInfo animRunning;
    public bool Running { set { if (anim && !forceIdle) anim.SetBool(animRunning.stringValue, value); } }
    [SerializeField] protected AnimatorParamStateInfo animBackwards;
    public bool Backwards { set { if (anim && !forceIdle) anim.SetBool(animBackwards.stringValue, value); } }

    //hybrid params
    [SerializeField] protected AnimatorParamStateInfo animDoubleJump;

    public void PlayDoubleJump()
    {
        PlayAnim(animDoubleJump);
    }
}
