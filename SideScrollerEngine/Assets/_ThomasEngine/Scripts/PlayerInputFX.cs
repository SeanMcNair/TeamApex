using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputFX : MonoBehaviour
{
    [System.Serializable]
    public class InputFXOption
    {
        public InputProperty inputButton;
        public InteractFX[] interactFXes;
    }

    [SerializeField] private InputFXOption[] inputOptions;

    private void Update()
    {
        GetInputs();
    }

    private void GetInputs()
    {
        foreach (var option in inputOptions)
        {
            if (Input.GetButtonDown(option.inputButton.stringValue))
            {
                foreach (var fx in option.interactFXes)
                {
                    fx.DoFX(gameObject, gameObject);
                }
            }
        }
    }
}
