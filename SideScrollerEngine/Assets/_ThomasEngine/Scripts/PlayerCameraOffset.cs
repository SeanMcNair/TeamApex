using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class PlayerCameraOffset : MonoBehaviour 
{

    public enum OffsetType { OffsetCamera, ChangePositionLockPlayer }

    [SerializeField] private OffsetType offsetType;
    [SerializeField] private PlayerCamera playerCamera;
    [SerializeField] private string triggerTag = "Player";
    [SerializeField] private float lockDelay = 0.2f;
    [SerializeField] private Transform newCamPos;
    [SerializeField] private Vector3 newOffset = new Vector3(0, 2, -10);
    [SerializeField] private Vector3 newRot = Vector3.zero;
    [SerializeField] private bool overrideRotLock;
    [SerializeField] private float offsetTime = 1;
    [SerializeField] private float lockTime = 5;
    [SerializeField] private bool resetOffsetOnExit = true;
    [SerializeField] private float resetTime = 1;
    [SerializeField] private bool smooth;
    [SerializeField] private GameObject playerPreview;


    private void Start()
    {
        if (!playerCamera)
        {
            playerCamera = Camera.main.GetComponent<PlayerCamera>();
        }
    }

    private void OnTriggerEnter2D(Collider2D col)
    {
        if (col.tag != triggerTag)
            return;

        if (!playerCamera)
        {
            Debug.LogError("No PlayerCamera assigned on " + gameObject.name + "! Please assign a PlayerCamera object");
            return;
        }

        if (offsetType == OffsetType.ChangePositionLockPlayer)
            StartCoroutine(StartChangePositionLockPlayer(col.GetComponent<PlayerController>(), lockDelay, lockTime));
        else
            OffsetCamera();

    }

    private void OnTriggerExit2D(Collider2D col)
    {
        if (col.tag != triggerTag)
            return;

        if (!playerCamera)
        {
            Debug.LogError("No PlayerCamera assigned on " + gameObject.name + "! Please assign a PlayerCamera object");
            return;
        }

        if (resetOffsetOnExit)
        {
            ResetCamera();
        }
            
    }

    void OffsetCamera()
    {
        if (playerCamera.GetCurOffset == newOffset)
            return;

        playerCamera.OffsetCamera(newOffset, newRot, offsetTime, smooth, overrideRotLock);
    }

    void SetCamPosition()
    {
        playerCamera.SetCameraPosition(newCamPos.position, newRot, offsetTime, smooth, overrideRotLock);
    }

    void ResetCamera()
    {
        if (resetTime > 0)
        {
            playerCamera.ResetCamera(resetTime, smooth);
        }
        else
            playerCamera.ResetCamera();
    }

    IEnumerator StartChangePositionLockPlayer(PlayerController _con, float _delay, float _time)
    {
        yield return new WaitForSeconds(_delay);
        _con.DisableMovement(true);
        SetCamPosition();
        yield return new WaitForSeconds(_time);
        ResetCamera();
        yield return new WaitForSeconds(resetTime + 0.3f);
        _con.DisableMovement(false);
        
    }

}
