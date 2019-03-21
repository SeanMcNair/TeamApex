using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public static class Utils
{

    public static bool IsPrefab(this GameObject _go)
    {
        return _go.scene.rootCount == 0;
    }

    public static Transform FindDeepChild(this Transform _parent, string _childName)
    {
        Transform[] children = _parent.GetComponentsInChildren<Transform>();
        foreach (var child in children)
        {
            if (child.name == _childName)
                return child;
        }
        Debug.Log("No match for name: " + _childName + " inside " + _parent.name);
        return null;  
    }

    public static string[] GetDeepChildNames(this Transform _parent)
    {
        var children = _parent.GetComponentsInChildren<Transform>();
        var names = new string[children.Length];
        for (int i = 0; i < children.Length; i++)
        {
            names[i] = children[i].name;
        }
        return names;
    }

    public static Material[] GetDeepChildMaterials(this Transform _parent)
    {
        var rends = _parent.GetComponentsInChildren<Renderer>();
        var startMats = new Material[rends.Length];
        for (int i = 0; i < rends.Length; i++)
        {
            startMats[i] = rends[i].material;
        }
        return startMats;
    }

    public static void SetAllChildMaterials(this Transform _parent, Material _mat)
    {
        var rends = _parent.GetComponentsInChildren<Renderer>();
        for (int i = 0; i < rends.Length; i++)
        {
            rends[i].material = _mat;
        }
    }

    public static bool IsClassOrSubClass(Type _type, Type _baseClass)
    {
        return _type.IsSubclassOf(_baseClass) || _type.IsAssignableFrom(_baseClass);
    }

    public static IEnumerator StartBoolTimer(BoolWrapper _bool, float _time = default(float), int _frames = default(int) )
    {
        _bool.Value = true;
        if (_time > 0)
            yield return new WaitForSeconds(_time);
        else if (_frames > 0)
        {
            int count = 0;
            while (count < _frames)
            {
                count++;
                yield return new WaitForEndOfFrame();
            }
        }
        _bool.Value = false;
    }

    public static void LookAt2D(this Transform _transform, Vector2 _targetPos, bool _ignoreZRotation = false, bool _consistentUpDirection = false)
    {
        var transPos = (Vector2)_transform.position;
        var dir = _targetPos - transPos;
        if (_ignoreZRotation)
            dir = new Vector2(_targetPos.x, _transform.position.y) - transPos;
        _transform.right = dir;
        if (_consistentUpDirection && Vector2.Dot(_transform.up, Vector2.down) > 0)
            _transform.Rotate(180, 0, 0);
    }

    public static void LookAway2D(this Transform _transform, Vector2 _targetPos, bool _ignoreZRotation = false, bool _consistentUpDirection = false)
    {
        var transPos = (Vector2)_transform.position;
        var dir = transPos - _targetPos;
        if (_ignoreZRotation)
            dir = new Vector2(transPos.x, _targetPos.y) - _targetPos;
        _transform.right = dir;
        if (_consistentUpDirection && Vector2.Dot(_transform.up, Vector2.down) > 0)
            _transform.Rotate(180, 0, 0);
    }

    public static IEnumerator ChangeFloatValueBySpeed(FloatWrapper _curValue, float _targetValue, float _speed)
    {
        var startValue = _curValue.Value;
        var diff = Mathf.Abs(_targetValue - _curValue.Value);
        var time = diff / _speed;
        float timer = 0;
        float perc = 0;
        while (timer < time)
        {
            timer += Time.deltaTime;
            if (timer > time)
                timer = time;
            perc = timer / time;
            _curValue.Value = Mathf.Lerp(startValue, _targetValue, perc);
            yield return new WaitForEndOfFrame();

        }
    }

    public static Transform FindClosestByTag(this Transform _pos, string _tag)
    {
        var objs = GameObject.FindGameObjectsWithTag(_tag);
        if (!(objs.Length > 0))
            return null;
        Transform closest = null;
        float distance = Mathf.Infinity;
        for (int i = 0; i < objs.Length; i++)
        {
            var dist = Vector2.Distance(_pos.position, objs[i].transform.position);
            if (dist < distance)
            {
                distance = dist;
                closest = objs[i].transform;
            }
                
        }
        return closest;
    }

    public static void LogComponentNullError(System.Type _type, GameObject _obj)
    {
        Debug.LogError("No " + _type.Name + " component found on " + _obj);
    }
}
