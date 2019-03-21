using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIPlayer : UIUnit
{
    [System.Serializable]
    public class CursorSettings
    {
        public bool visible = true;
        public CursorLockMode lockMode = CursorLockMode.None;

        public void ActivateSettings()
        {
            Cursor.visible = visible;
            Cursor.lockState = lockMode;
        }
    }

    public static UIPlayer instance;

    [SerializeField] protected UIValue lives;
    [SerializeField] protected UIValue points;
    [SerializeField] protected GameObject pauseMenu;
    [SerializeField] protected QuickMenuUI quickMenu;
    public QuickMenuUI QuickMenu { get { return quickMenu; } }
    [SerializeField] private CursorSettings startCursorSettings;
    [SerializeField] private CursorSettings pauseCursorSettings;

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        startCursorSettings.ActivateSettings();
    }

    public void SetSystemCursor(bool _visible, bool _locked)
    {
        Cursor.visible = _visible;
        if (_locked)
            Cursor.lockState = CursorLockMode.Locked;
        else
            Cursor.lockState = CursorLockMode.None;
    }

    public void SetLivesValue(float _value)
    {
        lives.SetCurValue(_value);
    }

    public void SetPointsValue(float _value)
    {
        points.SetCurValue(_value);
    }

    public void PauseMenuSetActive(bool _active)
    {
        if (_active)
            pauseCursorSettings.ActivateSettings();
        else
            startCursorSettings.ActivateSettings();
        pauseMenu.SetActive(_active);
    }
}
