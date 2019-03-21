using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEditor.Animations;
using System.Linq;
using UnityEditorInternal;
using System.Reflection;
using AnimatorController = UnityEditor.Animations.AnimatorController;

public static class EditorExtensions
{

    public static void ParameterField(this ParameterInfo _param, SerializedProperty _paramValueProperty, Rect _position, string _name)
    {
        var type = _param.ParameterType;
        var serializedObjectValue = _paramValueProperty.FindPropertyRelative("serializedObjectValue");

        if (type == typeof(bool))
        {
            var boolValue = _paramValueProperty.FindPropertyRelative("boolValue");
            boolValue.boolValue = EditorGUI.Toggle(_position, _name, boolValue.boolValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), boolValue.boolValue);
        }
        else if (type == typeof(AnimationCurve))
        {
            var animationCurveValue = _paramValueProperty.FindPropertyRelative("animationCurveValue");
            animationCurveValue.animationCurveValue = EditorGUI.CurveField(_position, _name, animationCurveValue.animationCurveValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), animationCurveValue.animationCurveValue);
        }
        else if (type == typeof(Color))
        {
            var colorValue = _paramValueProperty.FindPropertyRelative("colorValue");
            colorValue.colorValue = EditorGUI.ColorField(_position, _name, colorValue.colorValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), colorValue.colorValue);
        }
        else if (type == typeof(int))
        {
            var intValue = _paramValueProperty.FindPropertyRelative("intValue");
            intValue.intValue = EditorGUI.IntField(_position, _name, intValue.intValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), intValue.intValue);
        }
        else if (type == typeof(float))
        {
            var floatValue = _paramValueProperty.FindPropertyRelative("floatValue");
            floatValue.floatValue = EditorGUI.FloatField(_position, _name, floatValue.floatValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), floatValue.floatValue);

        }
        else if (type == typeof(Vector2))
        {
            var vector2Value = _paramValueProperty.FindPropertyRelative("vector2Value");
            vector2Value.vector2Value = EditorGUI.Vector2Field(_position, _name, vector2Value.vector2Value);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), vector2Value.vector2Value);
        }
        else if (type == typeof(Vector3))
        {
            var vector3Value = _paramValueProperty.FindPropertyRelative("vector3Value");
            vector3Value.vector3Value = EditorGUI.Vector3Field(_position, _name, vector3Value.vector3Value);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), vector3Value.vector3Value);
        }
        else if (type == typeof(Vector4))
        {
            var vector4Value = _paramValueProperty.FindPropertyRelative("vector4Value");
            vector4Value.vector4Value = EditorGUI.Vector4Field(_position, _name, vector4Value.vector4Value);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), vector4Value.vector4Value);
        }
        else if (type == typeof(Quaternion))
        {
            var vector4Value = _paramValueProperty.FindPropertyRelative("vector4Value");
            var quaternionValue = _paramValueProperty.FindPropertyRelative("quaternionValue");
            vector4Value.vector4Value = EditorGUI.Vector4Field(_position, _name, vector4Value.vector4Value);
            quaternionValue.quaternionValue = new Quaternion(vector4Value.vector4Value.x, vector4Value.vector4Value.y, vector4Value.vector4Value.z, vector4Value.vector4Value.w);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), quaternionValue.quaternionValue);
        }
        else if (type == typeof(Rect))
        {
            var rectValue = _paramValueProperty.FindPropertyRelative("rectValue");
            rectValue.rectValue = EditorGUI.RectField(_position, _name, rectValue.rectValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), rectValue.rectValue);
        }
        else if (type == typeof(string))
        {
            var stringValue = _paramValueProperty.FindPropertyRelative("stringValue");
            stringValue.stringValue = EditorGUI.TextField(_position, _name, stringValue.stringValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), stringValue.stringValue);
        }
        else if (type == typeof(LayerMask))
        {
            var layerMaskValue = _paramValueProperty.FindPropertyRelative("layerMaskValue");
            int maskInt = EditorGUI.MaskField(_position, _name, InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layerMaskValue.intValue), InternalEditorUtility.layers);
            layerMaskValue.intValue = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(maskInt);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), layerMaskValue.intValue);
        }
        else if (type == typeof(Bounds))
        {
            var boundsValue = _paramValueProperty.FindPropertyRelative("boundsValue");
            boundsValue.boundsValue = EditorGUI.BoundsField(_position, _name, boundsValue.boundsValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), boundsValue.boundsValue);
        }
        else if (type.IsEnum)
        {
            var boundsValue = _paramValueProperty.FindPropertyRelative("boundsValue");
            boundsValue.boundsValue = EditorGUI.BoundsField(_position, _name, boundsValue.boundsValue);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), boundsValue.boundsValue);
        }
        else
        {
            var objectValue = _paramValueProperty.FindPropertyRelative("objectValue");
            objectValue.objectReferenceValue = EditorGUI.ObjectField(_position, _name, objectValue.objectReferenceValue, type, true);
            serializedObjectValue.stringValue = StringSerializer.Serialize(typeof(object), objectValue.objectReferenceValue);
        }

    }

    public static ReorderableList ReorderableListCustom(this SerializedProperty _listProperty, SerializedObject _sourceRef, System.Type _elementType, string _header = "", int _addFieldAmount = 0)
    {
        var list = new ReorderableList(_sourceRef, _listProperty, true, true, true, true);

        int fieldAmount = 1;

        list.drawHeaderCallback = (Rect position) =>
        {
            EditorGUI.LabelField(position, _header);
        };

        list.drawElementCallback = (Rect position, int index, bool isActive, bool isFocused) =>
        {
            var element = _listProperty.GetArrayElementAtIndex(index);
            var fieldInfos = _elementType.GetFields(BindingFlags.Public | BindingFlags.Instance);
            fieldAmount = fieldInfos.Length + _addFieldAmount;
            EditorGUI.PropertyField(position, element, true);
            list.elementHeight = fieldAmount * (EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing);
        };

        return list;
    }

    public static void ArrayFieldCustom(this SerializedProperty _property, bool _showSize, bool _indent, string _prefixLabel = null)
    {
        _property.isExpanded = EditorGUILayout.Foldout(_property.isExpanded, _property.displayName);
        if (_property.isExpanded)
        {
            if (_indent)
                EditorGUI.indentLevel++;
            if (_showSize)
            {
                _property.arraySize = EditorGUILayout.DelayedIntField("Size", _property.arraySize);
            }

            for (int i = 0; i < _property.arraySize; i++)
            {
                var element = _property.GetArrayElementAtIndex(i);
                var label = new GUIContent
                {
                    text = element.displayName,
                };
                if (_prefixLabel != null)
                    label.text = _prefixLabel + " " + i + ":";
                EditorGUILayout.PropertyField(element, label);
            }
            if (_indent)
                EditorGUI.indentLevel--;
        }

    }

    public static void LabelFieldCustom(string _label, Color _color = default(Color), FontStyle _fontStyle = FontStyle.Normal, int _fontSize = 11)
    {
        GUIStyle style = new GUIStyle
        {
            normal = new GUIStyleState
            {
                textColor = _color,
            },
            fontStyle = _fontStyle,
            fontSize = _fontSize,

        };

        EditorGUILayout.LabelField(_label, style);
    }

    public static void IntFieldClamp(this SerializedProperty _intProperty, int _min, int _max)
    {
        _intProperty.intValue = Mathf.Clamp(_intProperty.intValue, _min, _max);
        EditorGUILayout.PropertyField(_intProperty);
    }

    public static void FloatFieldClamp(this SerializedProperty _floatProperty, float _min, float _max)
    {
        _floatProperty.floatValue = Mathf.Clamp(_floatProperty.floatValue, _min, _max);
        EditorGUILayout.PropertyField(_floatProperty);
    }

    public static void SetIndexStrings(IndexStringProperty _indexStringProperty, string[] _strings)
    {
        _indexStringProperty.stringValues = _strings;
    }

    public static void IndexStringPropertyField(this SerializedProperty _indexStringProperty, string[] _strings)
    {
        var prop = _indexStringProperty.GetRootValue<IndexStringProperty>();
        if (prop != null)
        {
            prop.stringValues = _strings;
            EditorGUILayout.PropertyField(_indexStringProperty);
        }
        else
        {
            Debug.Log(_indexStringProperty.displayName + " must be of type " + typeof(IndexStringProperty));
        }

    }

    public static void ChildNamePopUpParentOverride(this SerializedProperty _childNameProperty, SerializedObject _sourceRef, SerializedProperty _parentProperty)
    {
        var parent = _parentProperty.GetRootValue<GameObject>();
        ChildNamePopUpParentOverride(_childNameProperty, _sourceRef, parent, _parentProperty.displayName);

    }

    public static void ChildNamePopUpParentOverride(this SerializedProperty _childNameProperty, SerializedObject _sourceRef, GameObject _parent, string _parentFieldName)
    {
        var childName = _childNameProperty.GetRootValue<ChildName>();
        childName.overrideParent = true;
        if (childName.parent != _parent)
        {
            childName.overridePropertyName = _parentFieldName;
            childName.parent = _parent;
            _sourceRef.Update();
        }
        EditorGUILayout.PropertyField(_childNameProperty);

    }

    public static void DisplayAllChildrenPopup(string _fieldName, SerializedProperty _goProperty, SerializedProperty _indexProperty, SerializedProperty _stringProperty)
    {

        GameObject go = _goProperty.objectReferenceValue as GameObject;
        if (!go)
        {
            EditorGUILayout.LabelField(_fieldName, _goProperty.displayName + " is empty!");
            return;
        }
        else if (go.transform.childCount < 1)
        {
            EditorGUILayout.LabelField(_fieldName, _goProperty.displayName + " Must Have Children!");
            return;
        }

        //put all child names into array
        Transform[] childs = go.GetComponentsInChildren<Transform>();
        var childNames = new string[childs.Length];
        for (int i = 1; i < childs.Length; i++)
        {
            childNames[i] = childs[i].name;
        }

        //display popup
        _indexProperty.intValue = EditorGUILayout.Popup(_fieldName, _indexProperty.intValue, childNames);
        if (_indexProperty.intValue < childNames.Length)
            _stringProperty.stringValue = childNames[_indexProperty.intValue];
    }

    public static void DisplayAllInputAxisPopup(string _fieldName, SerializedProperty _indexProperty, SerializedProperty _stringProperty)
    {
        //put all input managers axis into an array
        var inputManager = AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/InputManager.asset")[0];
        var obj = new SerializedObject(inputManager);
        var axisArray = obj.FindProperty("m_Axes");
        var inputAxisNames = new string[axisArray.arraySize];
        for (int i = 0; i < inputAxisNames.Length; i++)
        {
            inputAxisNames[i] = axisArray.GetArrayElementAtIndex(i).FindPropertyRelative("m_Name").stringValue;
        }

        //display popup
        _indexProperty.intValue = EditorGUILayout.Popup(_fieldName, _indexProperty.intValue, inputAxisNames);
        SerializedProperty elementHor = axisArray.GetArrayElementAtIndex(_indexProperty.intValue);
        _stringProperty.stringValue = elementHor.FindPropertyRelative("m_Name").stringValue;
    }

    public static void SpritePreviewField(SerializedProperty _sprite, float _width, float _height, bool _stretchFit)
    {
        EditorGUILayout.BeginHorizontal();
        if (_sprite.objectReferenceValue != null)
        {
            var sprite = (Sprite)_sprite.objectReferenceValue;
            if (sprite)
            {
                GUIStyle style = new GUIStyle();
                style.fixedHeight = _height;
                style.fixedWidth = _width;
                style.alignment = TextAnchor.MiddleCenter;
                style.stretchHeight = _stretchFit;
                style.stretchWidth = _stretchFit;
                GUILayout.Box(sprite.texture, style);
            }

        }
        EditorGUILayout.PropertyField(_sprite);
        EditorGUILayout.EndHorizontal();
    }

    public static void DrawHandleTransformPoint(this SerializedProperty _localVector2Property, Object _source, Transform _sourceTrans, bool _lockYPosToSource = false, SerializedProperty _worldVector2Property = null)
    {
        EditorGUI.BeginChangeCheck();

        var localVector2 = _localVector2Property.GetRootValue<Vector2>();
        Vector2 handle = new Vector2();
        if (handle != (Vector2)_sourceTrans.transform.TransformPoint(localVector2))
            handle = Handles.PositionHandle((Vector2)_sourceTrans.transform.TransformPoint(localVector2), Quaternion.identity);
        else
            handle = Handles.PositionHandle(handle, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())//update script values after dragging
        {
            Undo.RecordObject(_source, "Modified " + _source + " properties.");
            var pos = handle;
            if (_lockYPosToSource)
                pos = new Vector2(handle.x, _sourceTrans.transform.position.y);
            _localVector2Property.SetValueOnRoot<Vector2>(_sourceTrans.transform.InverseTransformPoint(pos));
            if (_worldVector2Property != null)
            {
                localVector2 = _localVector2Property.GetRootValue<Vector2>();
                _worldVector2Property.SetValueOnRoot<Vector2>(_sourceTrans.transform.TransformPoint(localVector2));
            }


        }
    }

    public static void DrawHandleWorldPosition(this SerializedProperty _vector2Property, Object _source)
    {
        var vector2Source = _vector2Property.GetRootValue<Vector2>();
        Vector2 handle = new Vector2();
        if (handle != vector2Source)
            handle = Handles.PositionHandle(vector2Source, Quaternion.identity);
        else
            handle = Handles.PositionHandle(handle, Quaternion.identity);

        if (EditorGUI.EndChangeCheck())//update script values after dragging
        {
            Undo.RecordObject(_source, "Modified " + _source + " properties.");

            _vector2Property.SetValueOnRoot<Vector2>(handle);
        }
    }

    public static void DrawDetectZone(Transform _trans, SerializedProperty _detectZone)
    {
        var detectType = _detectZone.FindPropertyRelative("detectType");
        var offset = _detectZone.FindPropertyRelative("offset");
        var size = _detectZone.FindPropertyRelative("size");
        var useTransformZAngle = _detectZone.FindPropertyRelative("useTransformZAngle");
        var angle = _detectZone.FindPropertyRelative("angle");
        var radius = _detectZone.FindPropertyRelative("radius");
        var debugColor = _detectZone.FindPropertyRelative("debugColor");

        //set color
        Handles.color = debugColor.colorValue;
        //set position
        var pos = (Vector2)_trans.position + offset.vector2Value;

        //draw circle
        if (detectType.enumValueIndex == 0)
        {
            Handles.DrawWireDisc(pos, Vector3.back, radius.floatValue);
        }
        //draw box
        else if (detectType.enumValueIndex == 1)
        {
            if (useTransformZAngle.boolValue)
                angle.floatValue = _trans.eulerAngles.z;

            Matrix4x4 boxMatrix = Matrix4x4.TRS(pos, Quaternion.Euler(0, 0, angle.floatValue), Handles.matrix.lossyScale);
            using (new Handles.DrawingScope(boxMatrix))
            {
                Handles.DrawWireCube(Vector2.zero, size.vector2Value);
            }

        }

    }

    public static void PropertyFieldCustom(SerializedProperty _property, string _label, bool _includeChildren = false, Texture _image = null, string _toolTip = null)
    {
        GUIContent content = new GUIContent()
        {
            text = _label,
            image = _image,
            tooltip = _toolTip
        };

        EditorGUILayout.PropertyField(_property, content, _includeChildren);
    }

    public static void PrefabFieldWithComponent(this SerializedProperty _gameobjectProperty, System.Type _componentType)
    {
        _gameobjectProperty.objectReferenceValue =
            EditorGUILayout.ObjectField(_gameobjectProperty.displayName, _gameobjectProperty.objectReferenceValue, typeof(GameObject), false);

        var prefab = _gameobjectProperty.objectReferenceValue;
        if (prefab)
        {
            var obj = prefab as GameObject;
            if (obj)
            {
                if (!obj.GetComponent(_componentType))
                {
                    Debug.Log(obj.name + " does not have component: " + _componentType.Name + ". " + _gameobjectProperty.displayName +
                        " field requires a prefab with a " + _componentType.Name + " component.");
                    _gameobjectProperty.objectReferenceValue = null;
                }
            }

        }
    }

    public static void ScriptableObjectFieldType(SerializedProperty _scriptableProperty, System.Type _type)
    {
        _scriptableProperty.objectReferenceValue =
            EditorGUILayout.ObjectField(_scriptableProperty.displayName, _scriptableProperty.objectReferenceValue, typeof(ScriptableObject), false);

        var obj = _scriptableProperty.objectReferenceValue;
        if (obj)
        {
            var type = obj.GetType();
            if (type != _type)
            {
                Debug.Log(obj.name + " is not of type: " + _type.Name + ". " + _scriptableProperty.displayName +
                    " field requires to be  " + _type.Name);
                _scriptableProperty.objectReferenceValue = null;
            }

        }
    }

    public static void AnimatorStateField(SerializedProperty _property, Animator _anim)
    {
        var states = new AnimatorState[_anim.runtimeAnimatorController.animationClips.Length];
        states = EditorExtensions.GetAnimatorStateNames(_anim);
        if (states.Length > 0)
        {
            var stateNames = new string[states.Length];
            for (int i = 0; i < states.Length; i++)
            {
                stateNames[i] = states[i].name;
            }
            var anim = _property.GetRootValue<AnimatorParamStateInfo>();
            if (anim != null)
            {
                anim.indexValue = EditorGUILayout.Popup(_property.displayName, anim.indexValue, stateNames);
                anim.stringValue = stateNames[anim.indexValue];
            }

        }

    }

    public static AnimatorState[] GetAnimatorStateNames(Animator _animator)
    {
        AnimatorController controller = _animator ? _animator.runtimeAnimatorController as AnimatorController : null;
        return controller == null ? null : controller.layers.SelectMany(l => l.stateMachine.states).Select(s => s.state).ToArray();
    }

    public static AnimatorState[] GetAnimatorStateNames(AnimatorController _animator)
    {
        AnimatorController controller = _animator ? _animator : null;
        return controller == null ? null : controller.layers.SelectMany(l => l.stateMachine.states).Select(s => s.state).ToArray();
    }

    public static int FindStateLayer(AnimatorController _animCont, string _stateName)
    {
        for (int i = 0; i < _animCont.layers.Length; i++)
        {
            foreach (var child in _animCont.layers[i].stateMachine.states)
            {
                if (child.state.name == _stateName)
                    return i;
            }
        }
        Debug.LogError("could not find layer with state name: " + _stateName + " in: " + _animCont.name);
        return 0;
    }

    public static void IndexStringField(this SerializedProperty _indexStringProperty, string[] _stringArray, AnimatorController _animCont = null)
    {
        var indexValue = _indexStringProperty.FindPropertyRelative("indexValue");
        var stringValue = _indexStringProperty.FindPropertyRelative("stringValue");
        var layer = _indexStringProperty.FindPropertyRelative("layer");

        indexValue.intValue = EditorGUILayout.Popup(_indexStringProperty.displayName, indexValue.intValue, _stringArray);
        indexValue.intValue = Mathf.Clamp(indexValue.intValue, 0, _stringArray.Length - 1);
        stringValue.stringValue = _stringArray[indexValue.intValue];
        if (_animCont)
        {
            int lay = FindStateLayer(_animCont, stringValue.stringValue);
            layer.intValue = lay;
        }
    }

    public static void ClampArraySize(this SerializedProperty _targetList, int _size)
    {
        if (_targetList.arraySize == _size)
            return;

        while (_targetList.arraySize < _size)
        {
            _targetList.InsertArrayElementAtIndex(_targetList.arraySize);
        }
        while (_targetList.arraySize > _size)
        {
            _targetList.DeleteArrayElementAtIndex(_targetList.arraySize - 1);
        }
    }
}
