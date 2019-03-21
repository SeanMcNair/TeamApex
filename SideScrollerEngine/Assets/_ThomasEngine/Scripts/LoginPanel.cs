using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoginPanel : MonoBehaviour
{

    [SerializeField] private Text playerName;
    [SerializeField] private Text coins;
    [SerializeField] private Text lives;
    [SerializeField] private Text levelUnlocked;
    [SerializeField] private GameObject SetPlayerNameWindow;
    [SerializeField] private GameObject curPlayerWindow;
    [SerializeField] private GameObject newPlayerWindow;

    private LoginManager loginManager;
    private UserDataManager dataManager;
    private int slotInd;
    private string curPlayerName;

    private void Start()
    {
        dataManager = GameManager.instance.GetUserDataManager();
        dataManager.SetUserName("");
        SetPlayerNameWindow.SetActive(false);
    }

    public void SetTextData(string _name, int _coins, int _lives, int _levelUnlocked)
    {
        playerName.text = _name;
        if (coins)
            coins.text = _coins.ToString();
        if (lives)
            lives.text = _lives.ToString();
        levelUnlocked.text = _levelUnlocked.ToString();
    }

    public void LoadNewPlayerWindow(bool _load)
    {
        newPlayerWindow.SetActive(_load);
        curPlayerWindow.SetActive(!_load);
    }

    public void StartNewGameAndPlayer()
    { 
        SetPlayerNameWindow.SetActive(true);
        newPlayerWindow.SetActive(false);
        curPlayerWindow.SetActive(false);
    }

    public void SetPlayerName(string _name)
    {
        curPlayerName = _name;
    }

    public void ConfirmPlayerName()
    {
        SetPlayerNameWindow.SetActive(false);
        dataManager.SetUserName(curPlayerName);
        dataManager.CreateUser(slotInd);
        loginManager.RefreshPlayerList();
    }

    public void ErasePlayer()
    {
        dataManager.RemoveUser(slotInd);
        loginManager.RefreshPlayerList();
    }

    public void SetPlayerListParentActive(bool _active)
    {
        loginManager.GetLoginPanelParent().SetActive(_active);
    }

    public void SetLoginManager(LoginManager _manager)
    {
        loginManager = _manager;
    }

    public void SetSlotInd(int _ind)
    {
        slotInd = _ind;
    }

    public int GetSlotInd()
    {
        return slotInd;
    }
}
