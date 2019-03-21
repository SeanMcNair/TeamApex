using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MenuManager : MonoBehaviour
{

    [SerializeField]
    private bool freezeOnPause = false;

    private UIPlayer playerUI;

    private bool paused;

    void Start()
    {
        //unfreeze the game
        Time.timeScale = 1;
    }

    // Update is called once per frame
    void Update()
    {
        CheckGameOver();
        GetInputs();
    }

    void GetInputs()
    {

        if (Input.GetButtonDown("Cancel"))
        {
            if (!paused)
            {
                PauseGame(true);
                paused = true;
            }
            else
            {
                PauseGame(false);
                paused = false;
            }
        }
    }

    void CheckGameOver()
    {
        var gm = GameManager.instance;
        if (gm)
        {
            if (gm.IsGameOver)
            {
                if (!playerUI)
                {
                    playerUI = GameManager.instance.SpawnedPlayer.UI;
                    if (playerUI)
                        playerUI.SetSystemCursor(true, false);
                }
            }
        }
    }

    void PauseGame(bool _pause)
    {
        if (!playerUI)
        {
            playerUI = GameManager.instance.SpawnedPlayer.UI;
        }

        if (_pause)
        {
            playerUI.PauseMenuSetActive(true);

            if (freezeOnPause)
                Time.timeScale = 0;
        }
            
        else
        {
            playerUI.PauseMenuSetActive(false);

            if (freezeOnPause)
                Time.timeScale = 1;
        }
           
    }

    public bool isPaused()
    {
        return paused;
    }
}
