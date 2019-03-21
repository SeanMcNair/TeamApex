using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour
{ 
    [SerializeField] private Vector3 offset = new Vector3(0,1,-15);
    [SerializeField] private bool localXOffset;
    [SerializeField] private float XSensitivity = 10;
    [SerializeField] private float YSensitivity = 10;
    [SerializeField] private float ZSensitivity = 10;

    [SerializeField] private bool lockRotToPlayer;
    public bool IsLockRotToPlayer { get { return lockRotToPlayer; } set { lockRotToPlayer = value; } }
    private bool lockOverride;
    public bool IsLockOverride { get { return lockOverride; } set { lockOverride = value; } }
    [SerializeField] private Vector3 rotation = Vector3.zero;
    [SerializeField] private float rotSensitivity = 10;

    private Vector3 curOffset;
    public Vector3 GetCurOffset { get { return curOffset; } }
    private Vector3 lastOffset;
    private bool settingPosition;
    private bool offsettingCamera;
    private Coroutine offsetCoroutine;
    private Coroutine setPositionCoroutine;

    private Transform player;
    private Player pl;

    private void Start()
    {
        StartCoroutine(StartFindPlayerAndSetPosition());
    }

    void FixedUpdate()
    {
        if (!player)
            return;

        CheckOffset();
        FollowPlayer();
    }

    IEnumerator StartFindPlayerAndSetPosition()
    {
        settingPosition = true;
        while (!player)
        {

            GameObject playerGO = GameObject.FindGameObjectWithTag("Player");
            if (playerGO)
            {
                player = playerGO.transform;
                pl = player.GetComponent<Player>();
            }
            yield return new WaitForEndOfFrame();

        }
        settingPosition = false;
        InitializeCamera();
    }

    void InitializeCamera()
    {
        curOffset = offset;
        transform.position = player.position + curOffset;
    }

    void CheckOffset()
    {
        if (lastOffset != offset)
        {
            curOffset = offset;
            lastOffset = offset;
        }
    }

    void FollowPlayer()
    {
        if (settingPosition || pl.IsDead)
            return;

        var xOffset = player.position.x + curOffset.x;
        if (localXOffset && !offsettingCamera && !lockOverride)
        {
            var dir = Vector3.right;
            if (lockRotToPlayer)
                dir = Vector3.left;
            xOffset = player.TransformPoint(dir * curOffset.x).x;
        } 
        var xLerp = Mathf.Lerp(transform.position.x, xOffset, XSensitivity * Time.deltaTime);
        var yLerp = Mathf.Lerp(transform.position.y, player.position.y + curOffset.y, YSensitivity * Time.deltaTime);
        var zLerp = Mathf.Lerp(transform.position.z, player.position.z + curOffset.z, ZSensitivity * Time.deltaTime);

        transform.position = new Vector3(xLerp, yLerp, zLerp);
        var targetRot = Quaternion.Euler(rotation);
        if (lockRotToPlayer)
        {
            var dir = player.position - transform.position;
            targetRot = Quaternion.LookRotation(dir);
        }
        if (curOffset == offset || !lockOverride)
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRot, rotSensitivity * Time.deltaTime);
        }
            
    }

    public void OffsetCamera(Vector3 _offset, Vector3 _eulerAngles, float _time, bool _smooth, bool _overrideLock)
    {
        StopMovementCoroutines();
        offsetCoroutine = StartCoroutine(StartOffset(_offset, _eulerAngles, _time, _smooth, _overrideLock));
    }

    public void SetCameraPosition(Vector3 _position,Vector3 _eulerAngles, float _time, bool _smooth, bool _overrideLock)
    {
        StopMovementCoroutines();
        setPositionCoroutine = StartCoroutine(StartSetCamPosition(_position,_eulerAngles, _time, _smooth, _overrideLock));
    }

    void StopMovementCoroutines()
    {
        if (offsetCoroutine != null)
            StopCoroutine(offsetCoroutine);
        if (setPositionCoroutine != null)
            StopCoroutine(setPositionCoroutine);
    }

    public void ResetCamera()
    {
        offsettingCamera = false;
        settingPosition = false;
        StopMovementCoroutines();
        curOffset = offset;
    }

    public void ResetCamera(float _time, bool _smooth)
    {
        offsettingCamera = false;
        settingPosition = false;
        StopMovementCoroutines();
        OffsetCamera(offset, rotation, _time, _smooth, false);
    }

    IEnumerator StartOffset(Vector3 _offset, Vector3 _eulerAngles, float _time, bool _smooth, bool _overrideLock)
    {
        if (_overrideLock)
            lockOverride = true;
        offsettingCamera = true;
        Vector3 startOffset = offset;
        if (player)
            startOffset = transform.position - player.position;
        var startRot = transform.rotation;
        float timer = 0;
        float perc = 0;
        while (perc < 1 && offsettingCamera)
        {
            timer += Time.deltaTime;
            if (timer > _time)
            {
                timer = _time;
            }
            perc = timer / _time;

            if (_smooth)
                perc = perc * perc * (3 - 2 * perc);

            curOffset = Vector3.Lerp(startOffset, _offset, perc);
            if (!lockRotToPlayer || lockOverride)
                transform.rotation = Quaternion.Slerp(startRot, Quaternion.Euler(_eulerAngles), perc);
            yield return new WaitForFixedUpdate();
        }
        if (!_overrideLock)
            lockOverride = false;
        offsettingCamera = false;
    }

    IEnumerator StartSetCamPosition(Vector3 _pos, Vector3 _eulerAngles, float _time, bool _smooth, bool _overrideLock)
    {
        if (_overrideLock)
            lockOverride = true;
        settingPosition = true;
        Vector3 curPos = transform.position;
        var startRot = transform.rotation;
        float timer = 0;
        float perc = 0;
        while (perc < 1 && settingPosition)
        {
            timer += Time.deltaTime;
            if (timer > _time)
            {
                timer = _time;
            }
            perc = timer / _time;
            if (_smooth)
                perc = perc * perc * (3f - 2f * perc);

            transform.position = Vector3.Lerp(curPos, _pos, perc);
            if (!lockRotToPlayer || lockOverride)
                transform.rotation = Quaternion.Slerp(startRot, Quaternion.Euler(_eulerAngles), perc);
            yield return new WaitForFixedUpdate();
        }
        if (!_overrideLock)
            lockOverride = false;
    }

    public void SetZSensitivity(float _sens)
    {
        ZSensitivity = _sens;
    }

    public void SetXSensitivity(float _sens)
    {
        XSensitivity = _sens;
    }

    public void SetYSensitivity(float _sens)
    {
        YSensitivity = _sens;
    }
}
