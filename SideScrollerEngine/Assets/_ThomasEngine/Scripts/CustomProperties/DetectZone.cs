using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
#if UNITY_EDITOR
using UnityEditor;
#endif

//This is a custom serilizable detect zone property using Overlap functions.
//There is a custom editor property drawer for this class
[System.Serializable]
public class DetectZone
{
    public bool overrideZoneName;
    public string zoneName;
    public enum PositionType { Offset, Local, World }
    public enum DetectAreaType { Circle, Box, Point, LineCast }
    public bool overrideDetectMask;
    public LayerMask detectMask;
    public bool overrideDetectType;
    public DetectAreaType detectType;
    public bool overridePositionType;
    public PositionType positionType;
    public Transform trans;
    public Vector2 worldPos;
    public Vector2 offset;
    public Vector2 size = Vector2.one;
    public bool useTransformZAngle;
    public float angle = 0;
    public float radius = 1;
    public Color debugColor = Color.cyan;

    //gui stuff
    public Vector2 handlePoint;

    private Collider2D col;
    private Collider2D[] cols;
    public RaycastHit2D lineHit;
    private Vector2 curDirection;
    private Vector2 curDetectPos;
    private Vector2 lastDetectPos;

    private enum CallBackType { OnEnter, OnExit, OnStay }
    private Coroutine detectRoutine;
    private Coroutine triggerRoutine;
    private List<Collider2D> colsList = new List<Collider2D>();
    private List<Collider2D> enteredList = new List<Collider2D>();
    private List<System.Action<Collider2D>> enterCallBacks = new List<System.Action<Collider2D>>();
    private List<System.Action<Collider2D>> exitCallBacks = new List<System.Action<Collider2D>>();
    private List<System.Action<Collider2D>> stayCallBacks = new List<System.Action<Collider2D>>();

    #region DETECTION

    void SetCurDetectPos(Transform _trans = null)
    {
        if (positionType == PositionType.Offset && _trans)
            curDetectPos = _trans.TransformPoint(offset);
        else if (positionType == PositionType.Local && trans)
            curDetectPos = trans.TransformPoint(offset);
        else
            curDetectPos = worldPos + offset;

        if (useTransformZAngle)
        {
            if (positionType == PositionType.Local && trans)
                angle = trans.eulerAngles.z;
            else if (positionType == PositionType.Offset && _trans)
                angle = _trans.eulerAngles.z;
        }
        if (lastDetectPos == Vector2.zero)
            lastDetectPos = curDetectPos;
    }

    void GetDirection()
    {
        curDirection = (curDetectPos - lastDetectPos).normalized;
        lastDetectPos = curDetectPos;
    }

    public Collider2D[] DetectCollidersNonAlloc(Transform _trans = null, int _maxAmount = 1)
    {
        SetCurDetectPos(_trans);
        cols = new Collider2D[_maxAmount];
        if (detectType == DetectAreaType.Circle)
            Physics2D.OverlapCircleNonAlloc(curDetectPos, radius, cols, detectMask);
        else if (detectType == DetectAreaType.Box)
            Physics2D.OverlapBoxNonAlloc(curDetectPos, size, angle, cols, detectMask);
        else if (detectType == DetectAreaType.Point)
            Physics2D.OverlapPointNonAlloc(curDetectPos, cols, detectMask);
        else if(detectType == DetectAreaType.LineCast)
        {
            var hits = new RaycastHit2D[_maxAmount];
            Physics2D.LinecastNonAlloc(curDetectPos, lastDetectPos, hits, detectMask);
            if (hits.Length > 0)
            {
                for (int i = 0; i < hits.Length; i++)
                {
                    cols[i] = hits[i].collider;
                }
            }
            GetDirection();
        }
        return cols;
    }

    public Collider2D[] DetectColliders(Transform _trans = null)
    {
        SetCurDetectPos(_trans);
        if (detectType == DetectAreaType.Circle)
            cols = Physics2D.OverlapCircleAll(curDetectPos, radius, detectMask);
        else if (detectType == DetectAreaType.Box)
            cols = Physics2D.OverlapBoxAll(curDetectPos, size, angle, detectMask);
        else if (detectType == DetectAreaType.Point)
            cols = Physics2D.OverlapPointAll(curDetectPos, detectMask);
        else if (detectType == DetectAreaType.LineCast)
        {
            var hits = Physics2D.LinecastAll(lastDetectPos, curDetectPos, detectMask);
            if (hits.Length > 0)
            {
                var cols = new Collider2D[hits.Length];
                for (int i = 0; i < hits.Length; i++)
                {
                    cols[i] = hits[i].collider;
                }
            }
            GetDirection();
        }
        return cols;
    }

    public Collider2D DetectCollider(Transform _trans = null)
    {
        SetCurDetectPos(_trans);
        return DetectCollider(curDetectPos);
    }

    public Collider2D DetectCollider(Vector2 _pos)
    {
        if (detectType == DetectAreaType.Circle)
            col = Physics2D.OverlapCircle(_pos, radius, detectMask);
        else if (detectType == DetectAreaType.Box)
            col = Physics2D.OverlapBox(_pos, size, angle, detectMask);
        else if (detectType == DetectAreaType.Point)
            col = Physics2D.OverlapPoint(_pos, detectMask);
        else if (detectType == DetectAreaType.LineCast)
        {
            lineHit = Physics2D.Linecast(lastDetectPos, curDetectPos, detectMask);
            if (lineHit)
                col = lineHit.collider;
            GetDirection();
        }
        return col;
    }

    public bool Detected(Transform _trans = null)
    {
        return DetectCollider(_trans);
    }

    #endregion

    #region TRIGGER_CALLBACKS

    public void AddEnterTrigger(MonoBehaviour _sender, System.Action<Collider2D>_callback)
    {
        DoTriggerDetection(_sender);
        enterCallBacks.Add(_callback);
    }

    public void RemoveEnterTrigger(MonoBehaviour _sender, System.Action<Collider2D> _callback)
    {
        enterCallBacks.Remove(_callback);
        CheckCallBacks(_sender);
    }

    public void AddExitTrigger(MonoBehaviour _sender, System.Action<Collider2D> _callback)
    {
        DoTriggerDetection(_sender);
        exitCallBacks.Add(_callback);
    }

    public void RemoveExitTrigger(MonoBehaviour _sender, System.Action<Collider2D> _callback)
    {
        exitCallBacks.Remove(_callback);
        CheckCallBacks(_sender);
    }

    public void AddStayTrigger(MonoBehaviour _sender, System.Action<Collider2D> _callback)
    {
        DoTriggerDetection(_sender);
        stayCallBacks.Add(_callback);
    }

    public void RemoveStayTrigger(MonoBehaviour _sender, System.Action<Collider2D> _callback)
    {
        stayCallBacks.Remove(_callback);
        CheckCallBacks(_sender);
    }

    public void ClearEnterTriggers(MonoBehaviour _sender)
    {
        enterCallBacks.Clear();
        CheckCallBacks(_sender);
    }

    public void ClearExitTriggers(MonoBehaviour _sender)
    {
        exitCallBacks.Clear();
        CheckCallBacks(_sender);
    }

    public void ClearStayTriggers(MonoBehaviour _sender)
    {
        stayCallBacks.Clear();
        CheckCallBacks(_sender);
    }

    public void ClearAllTriggers(MonoBehaviour _sender)
    {
        stayCallBacks.Clear();
        enterCallBacks.Clear();
        exitCallBacks.Clear();
        CheckCallBacks(_sender);
    }

    void DoTriggerDetection(MonoBehaviour _sender)
    {
        if (detectRoutine == null)
            detectRoutine = _sender.StartCoroutine(StartColliderDetection(_sender.transform));
        if (triggerRoutine == null)
            triggerRoutine = _sender.StartCoroutine(StartTriggerDetection());
    }

    void CheckCallBacks(MonoBehaviour _sender)
    {
        if (stayCallBacks.Count < 1 && enterCallBacks.Count < 1 && exitCallBacks.Count < 1)
        {
            if (detectRoutine != null)
                _sender.StopCoroutine(detectRoutine);
            if (triggerRoutine != null)
                _sender.StopCoroutine(triggerRoutine);
                
        }
    }

    IEnumerator StartColliderDetection(Transform _sender)
    {
        while (Application.isPlaying)
        {
            colsList = DetectColliders(_sender).ToList();
            yield return new WaitForFixedUpdate();
        }
    }

    IEnumerator StartTriggerDetection()
    {
        while (Application.isPlaying)
        {
            CheckEnterTrigger();
            CheckExitTrigger();
            yield return new WaitForEndOfFrame();
        }
    }

    void CheckEnterTrigger()
    {
        if (colsList.Count < 1)
            return;

        for (int i = 0; i < colsList.Count; i++)
        {
            //match found in entered?
            if (!enteredList.Contains(cols[i]))
            {
                //new collider entered detection zone
                enteredList.Add(cols[i]);
                //do all enter callbacks
                foreach (var callBack in enterCallBacks)
                    callBack.Invoke(cols[i]);
            }
        }

        
    }

    void CheckExitTrigger()
    {
        if (enteredList.Count < 1)
            return;

        for (int i = 0; i < enteredList.Count; i++)
        {
            //match found?
            if (!cols.Contains(enteredList[i]))//no
            {
                //a collider has exited detection zone..do exit callbacks
                foreach (var callback in exitCallBacks)
                    callback.Invoke(enteredList[i]);
                //remove collider from entered list
                enteredList.Remove(enteredList[i]);
            }
            else//yes
            {
                //the collider is still in detection zone..do callbacks
                foreach (var callback in stayCallBacks)
                    callback.Invoke(enteredList[i]);
            }
        }
    }

    #endregion

    #region EDITOR_FUNCTIONS

#if UNITY_EDITOR
    public void DrawDetectZone(Object _source, SerializedObject _sourceRef, Transform _sourceTrans = null)
    {
        Handles.color = debugColor;
        EditorGUI.BeginChangeCheck();

        var pos = offset;
        if (positionType == PositionType.World)
        {
            if (handlePoint != worldPos)
                handlePoint = worldPos;
            handlePoint = Handles.PositionHandle(handlePoint, Quaternion.identity);

            //position points after dragging
            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(_source, "Modified " + _source + " properties.");
                worldPos = handlePoint;
                _sourceRef.ApplyModifiedProperties();
                _sourceRef.Update();
            }

            //get final position
            pos = worldPos + offset;
        }
        else if (positionType == PositionType.Local && trans)
                pos = trans.TransformPoint(offset);
        else if (positionType == PositionType.Offset && _sourceTrans)
                pos = _sourceTrans.TransformPoint(offset);

        if (useTransformZAngle)
        {
            if (positionType == PositionType.Local && trans)
                angle = trans.eulerAngles.z;
            else if (positionType == PositionType.Offset && _sourceTrans)
                angle = _sourceTrans.eulerAngles.z;
        }
        Matrix4x4 angleMatrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, angle), Handles.matrix.lossyScale);
        using (new Handles.DrawingScope(angleMatrix))
        {
            //draw the objects
            if (detectType == DetectAreaType.Box)
                Handles.DrawWireCube(Vector2.zero, size);
            else if (detectType == DetectAreaType.Circle)
                Handles.DrawWireDisc(Vector2.zero, Vector3.back, radius);
        }

    }
#endif

    #endregion
}


