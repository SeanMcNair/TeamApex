using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

[CustomPropertyDrawer(typeof(UnitAITriggerContainer))]
public class UnitAITriggerContainerPropertyDrawer : PropertyDrawerCustom
{
    private SerializedProperty unitAITriggers;
    private UnitAITriggerContainer source;
    private UnitAI aiSource;
    private UnitEquip equipSource;
    private List<UnitAITrigger> triggerSource = new List<UnitAITrigger>();

    private GUIStyle boldStyle;
    private ReorderableList eventList;
    private float fieldSize;

    private bool initialized;

    protected override void GetProperties(SerializedProperty _property)
    {
        base.GetProperties(_property);

        boldStyle = new GUIStyle
        {
            fontStyle = FontStyle.Bold,
        };

        //get property values
        unitAITriggers = _property.FindPropertyRelative("unitAITriggers");
        source = fieldInfo.GetValue(_property.serializedObject.targetObject) as UnitAITriggerContainer;
        triggerSource = source.unitAITriggers;
        aiSource = _property.serializedObject.targetObject as UnitAI;
        equipSource = aiSource.GetComponent<UnitEquip>();
    }

    protected override void SetFieldAmount()
    {
        if (!initialized)
            Initialize(sourceRef);

        if (eventList.serializedProperty.arraySize > 0)
        {
            fieldAmount = 3;
            for (int i = 0; i < eventList.serializedProperty.arraySize; i++)
            {
                var element = eventList.serializedProperty.GetArrayElementAtIndex(i);
                var currentFieldAmount = element.FindPropertyRelative("currentFieldAmount");
                fieldAmount += currentFieldAmount.intValue;
            }
        }
        else
        {
            fieldAmount = 4;
        }
    }

    protected override void OnGUICustom(Rect position, SerializedProperty property, GUIContent label, bool _prefixLabel = false, int _indentLevel = 0)
    {
        base.OnGUICustom(position, property, GUIContent.none, _prefixLabel, _indentLevel);
    }

    private void Initialize(SerializedProperty property)
    {
        initialized = true;
        fieldSize = EditorGUIUtility.singleLineHeight + EditorGUIUtility.standardVerticalSpacing;
        //set up lists and cache data and whatnot
        eventList = new ReorderableList(property.serializedObject, unitAITriggers, true, true, true, true);
        eventList.drawHeaderCallback = (Rect _position) =>
        {
            EditorGUI.LabelField(_position, "AI Events");
        };

    }

    protected override void DisplayGUIElements(Rect position, SerializedProperty property)
    {
        EditorGUI.BeginChangeCheck();

        eventList.elementHeightCallback = (index) =>
        {
            float height = 0;
            var element = eventList.serializedProperty.GetArrayElementAtIndex(index);
            var currentFieldAmount = element.FindPropertyRelative("currentFieldAmount");
            height += currentFieldAmount.intValue;

            return fieldSize * height;
        };

        eventList.drawElementCallback = (Rect pos, int index, bool isActive, bool isFocused) =>
        {
            var element = eventList.serializedProperty.GetArrayElementAtIndex(index);
            var eventSource = triggerSource[index];

            //trigger
            var triggerType = element.FindPropertyRelative("triggerType");
            var useType = element.FindPropertyRelative("useType");
            var amount = element.FindPropertyRelative("amount");
            var detectZoneInd = element.FindPropertyRelative("detectZoneInd");
            var events = element.FindPropertyRelative("events");


            //valueamount
            var valueAmountManager = element.FindPropertyRelative("valueAmountManager");
            var valueAmountInd = element.FindPropertyRelative("valueAmountInd");
            var valueOption = element.FindPropertyRelative("valueOption");
            var comparedValue = element.FindPropertyRelative("comparedValue");

            //gui stuff
            var currentFieldAmount = element.FindPropertyRelative("currentFieldAmount");

            pos.height = EditorGUIUtility.singleLineHeight;

            currentFieldAmount.intValue = 1;

            EditorGUI.PropertyField(pos, triggerType);

            currentFieldAmount.intValue++;
            pos.y += fieldSize;
            EditorGUI.PropertyField(pos, useType);

            if (useType.enumValueIndex == 1)
            {
                currentFieldAmount.intValue++;
                pos.y += fieldSize;
                EditorGUI.PropertyField(pos, amount);
            }
            if (amount.intValue < 1)
                amount.intValue = 1;

            currentFieldAmount.intValue++;
            pos.y += fieldSize;
            //detect zone
            if (triggerType.enumValueIndex != 3)
            {
                var detectNames = new string[aiSource.detectZones.Length];
                for (int i = 0; i < aiSource.detectZones.Length; i++)
                {
                    detectNames[i] = aiSource.detectZones[i].zoneName;
                }
                detectZoneInd.intValue = EditorGUI.Popup(pos, "Detect Zone To Use", detectZoneInd.intValue, detectNames);
            }
            else
            {
                //value Amount
                EditorGUI.PropertyField(pos, valueAmountManager);
                if (eventSource.valueAmountManager)
                {
                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    var names = eventSource.valueAmountManager.GetValueNames();
                    valueAmountInd.intValue = EditorGUI.Popup(pos, "Value To Use", valueAmountInd.intValue, names);

                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.PropertyField(pos, valueOption);

                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.PropertyField(pos, comparedValue);
                }
                

            }

            //events
            currentFieldAmount.intValue++;
            pos.y += fieldSize;
            events.isExpanded = EditorGUI.Foldout(pos,events.isExpanded, events.displayName);

            //All event elements
            if (events.isExpanded)
            {
                EditorGUI.indentLevel++;
                currentFieldAmount.intValue++;
                pos.y += fieldSize;
                events.arraySize = EditorGUI.DelayedIntField(pos,"Event Amount", events.arraySize);
                for (int i = 0; i < events.arraySize; i++)
                {
                    var eventElement = events.GetArrayElementAtIndex(i);

                    var startType = eventElement.FindPropertyRelative("startType");

                    //event type
                    var eventTypeMask = eventElement.FindPropertyRelative("eventTypeMask");
                    //state type
                    var stateType = eventElement.FindPropertyRelative("stateType");
                    //items
                    var itemEventType = eventElement.FindPropertyRelative("itemEventType");
                    var item = eventElement.FindPropertyRelative("item");
                    //anim
                    var setAnimator = eventElement.FindPropertyRelative("setAnimator");
                    var animator = eventElement.FindPropertyRelative("animator");
                    var stateToPlay = eventElement.FindPropertyRelative("stateToPlay");
                    var crossfadeTime = eventElement.FindPropertyRelative("crossfadeTime");
                    //method
                    var methodToCall = eventElement.FindPropertyRelative("methodToCall");
                    //interacts
                    var interacts = eventElement.FindPropertyRelative("interacts");
                    //common stuff
                    var delay = eventElement.FindPropertyRelative("delay");
                    var repeat = eventElement.FindPropertyRelative("repeat");
                    var repeatTime = eventElement.FindPropertyRelative("repeatTime");
                    var finishType = eventElement.FindPropertyRelative("finishType");
                    var totalTime = eventElement.FindPropertyRelative("totalTime");
                    var finished = eventElement.FindPropertyRelative("finished");

                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.LabelField(pos, "====================");
                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.LabelField(pos, "Event " + (i + 1).ToString(), boldStyle);
                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.LabelField(pos, "====================");

                    //start and trigger
                    if (i > 0)
                    {
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, startType);
                    }
                    
                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    //event Type
                    string[] eventTypes = { "State Event", "Item Event", "Play Animation", "Call Method", "Do Interact FX" };
                    eventTypeMask.intValue = EditorGUI.MaskField(pos, "Event Mask", eventTypeMask.intValue, eventTypes);

                    //state events
                    if (eventTypeMask.intValue == (eventTypeMask.intValue | (1 << 0)))
                    {
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.LabelField(pos, "Unit State", boldStyle);
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, stateType);
                    }

                    //item events
                    if (eventTypeMask.intValue == (eventTypeMask.intValue | (1 << 1)))
                    {
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.LabelField(pos, "Item Options", boldStyle);

                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, itemEventType);

                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        var names = equipSource.ItemManager.GetItemNames();
                        item.intValue = EditorGUI.Popup(pos, "Item", item.intValue, names);
                    }

                    //anim events
                    if (eventTypeMask.intValue == (eventTypeMask.intValue | (1 << 2)))
                    {
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.LabelField(pos, "Animator", boldStyle);

                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, setAnimator);

                        if (setAnimator.boolValue)
                        {
                            currentFieldAmount.intValue++;
                            pos.y += fieldSize;
                            EditorGUI.PropertyField(pos, animator);
                        }
                        else if (animator.objectReferenceValue)
                            animator.objectReferenceValue = null;

                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, stateToPlay);

                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, crossfadeTime);
                    }

                    //method events
                    if (eventTypeMask.intValue == (eventTypeMask.intValue | (1 << 3)))
                    {
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.LabelField(pos, "Call Method", boldStyle);

                        int methodFieldAmount;
                        methodToCall.MethodPropertyField(pos, 1, out pos, out methodFieldAmount);
                        currentFieldAmount.intValue += methodFieldAmount;
                    }

                    if (eventTypeMask.intValue == (eventTypeMask.intValue | (1 << 4)))
                    {

                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.LabelField(pos, "Interact FX", boldStyle);

                        currentFieldAmount.intValue++;
                        //interacts
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, interacts, true);

                        if (interacts.isExpanded)
                        {
                            currentFieldAmount.intValue++;
                            pos.y += fieldSize;
                            foreach (var interact in interacts)
                            {
                                currentFieldAmount.intValue++;
                                pos.y += fieldSize;
                            }

                        }
                    }

                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.LabelField(pos, "Time Options", boldStyle);

                    //delay
                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.PropertyField(pos, delay);

                    //repeat
                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.PropertyField(pos, repeat);

                    //repeat delay
                    if (repeat.boolValue)
                    {
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, repeatTime);
                    }

                    //finishtype
                    currentFieldAmount.intValue++;
                    pos.y += fieldSize;
                    EditorGUI.PropertyField(pos, finishType);

                    //totalTime
                    if (finishType.enumValueIndex == 1)
                    {
                        currentFieldAmount.intValue++;
                        pos.y += fieldSize;
                        EditorGUI.PropertyField(pos, totalTime);
                    }
                }
                EditorGUI.indentLevel--;
            }

        };

        eventList.DoList(position);

        if (EditorGUI.EndChangeCheck())
        {
            Undo.RecordObject(property.serializedObject.targetObject, "List Change");
            property.serializedObject.ApplyModifiedProperties();
        }
    }
}