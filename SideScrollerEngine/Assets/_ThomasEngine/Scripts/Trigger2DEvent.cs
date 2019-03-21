using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Internal;

[RequireComponent(typeof(Collider2D))]
public class Trigger2DEvent : MonoBehaviour
{
    [System.Serializable]
    public class TriggerEventOption
    {
        public bool useUnityEvent;
        public UnityEvent triggerEvent;
        public InteractFX[] triggerInteracts;

        public void DoEvent(GameObject _sender, GameObject _receiver)
        {
            if (useUnityEvent)
                triggerEvent.Invoke();

            for (int i = 0; i < triggerInteracts.Length; i++)
            {
                triggerInteracts[i].DoFX(_sender, _receiver);
            }
        }
    }

    [SerializeField] private bool buttonDownToTrigger;
    [SerializeField] private InputProperty button;

    [SerializeField] private float delayTriggerTime;

    [SerializeField] private bool useRepeatDelay;
    [SerializeField] private float repeatDelay = 0.1f;
    private float repeatTimer;

    public int mask = 0;
    public enum TriggerType { Trigger2DEnter, Trigger2DExit, Trigger2DStay }

    [SerializeField] private string triggerTag = "Untagged";

    //events
    [SerializeField] private TriggerEventOption trigger2DEnterEvents;
    [SerializeField] private TriggerEventOption trigger2DExitEvents;
    [SerializeField] private TriggerEventOption trigger2DStayEvents;


    private bool startTrigger;

    private bool triggered;
    public bool IsTriggered { get { return triggered; } }

    private Coroutine triggeredCoroutine;

    private void Start()
    {
        StartCoroutine(StartWait());
    }

    IEnumerator StartWait()
    {
        EnableColliders(false);
        yield return new WaitForSeconds(delayTriggerTime);
        EnableColliders(true);
        startTrigger = true;
    }

    void EnableColliders(bool _enable)
    {
        foreach (var col in GetComponents<Collider2D>())
        {
            col.enabled = _enable;
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (!startTrigger)
            return;


        if (mask != (mask | (1 << 0)))
            return;

        if (col.tag == triggerTag)
        {
            trigger2DEnterEvents.DoEvent(gameObject, col.gameObject);
            if (triggeredCoroutine != null)
                StopCoroutine(triggeredCoroutine);
            triggeredCoroutine = StartCoroutine(StartTriggerSwitch());
        }
    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (!startTrigger)
            return;

        if (mask != (mask | (1 << 1)))
            return;


        if (col.tag == triggerTag)
        {
            trigger2DExitEvents.DoEvent(gameObject, col.gameObject);
            if (triggeredCoroutine != null)
                StopCoroutine(triggeredCoroutine);
            triggeredCoroutine = StartCoroutine(StartTriggerSwitch());

        }
    }

    private void OnTriggerStay2D(Collider2D col)
    {
        if (!startTrigger)
            return;

        if (mask != (mask | (1 << 2)))
            return;

        if (col.tag == triggerTag)
        {
            if (buttonDownToTrigger)
            {
                if (Input.GetButtonDown(button.stringValue))
                {
                    trigger2DStayEvents.DoEvent(gameObject, col.gameObject);
                    if (triggeredCoroutine != null)
                        StopCoroutine(triggeredCoroutine);
                    triggeredCoroutine = StartCoroutine(StartTriggerSwitch());
                }
                if (useRepeatDelay && Input.GetButton(button.stringValue))
                {
                    repeatTimer += Time.deltaTime;
                    if (repeatTimer > repeatDelay)
                    {
                        trigger2DStayEvents.DoEvent(gameObject, col.gameObject);
                        if (triggeredCoroutine != null)
                            StopCoroutine(triggeredCoroutine);
                        triggeredCoroutine = StartCoroutine(StartTriggerSwitch());
                        repeatTimer = 0;
                    }
                }

            }
            else
            {
                trigger2DStayEvents.DoEvent(gameObject, col.gameObject);
                if (triggeredCoroutine != null)
                    StopCoroutine(triggeredCoroutine);
                triggeredCoroutine = StartCoroutine(StartTriggerSwitch());
            }

        }

    }

    IEnumerator StartTriggerSwitch()
    {
        triggered = true;
        yield return new WaitForEndOfFrame();
        triggered = false;
    }

}
