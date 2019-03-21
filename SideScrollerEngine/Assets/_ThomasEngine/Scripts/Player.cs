using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : Unit
{
    public enum UserMode { None, CurUser, Override }
    [SerializeField] private bool setData;
    [SerializeField] private UserMode userMode;
    [SerializeField] private UserDataManager userDataManager;
    [SerializeField] private IndexStringProperty user;

    private int curPoints;
    public int CurPoints { get { return curPoints; } }

    public new PlayerSkinData CurUnitData { get { return (PlayerSkinData)curData; } }
    public new UIPlayer UI { get { return (UIPlayer)ui; } }
    private UserDataManager dataManager;
    private SpawnCheckPointManager spawnManager;
    private PlayerSkinManager skinManager;

    private PlayerSoundFX soundFX;
    private User curUser;

    protected override void GetComponents()
    {
        base.GetComponents();
        //get components
        var gm = GameManager.instance;
        if (gm)
        {
            dataManager = gm.GetUserDataManager();
            spawnManager = gm.GetSpawnManager();
            skinManager = gm.GetSkinManager();
            gm.SpawnedPlayer = this;
        }
    }

    protected override void SetDefaultData()
    {
        if (setData)
        {
            curData = (PlayerSkinData)data;
        }
        else
        {
            if (userMode == UserMode.CurUser)
                curUser = dataManager.GetCurUser();
            else if (userMode == UserMode.Override)
                curUser = dataManager.GetUser(user.indexValue);
            
            curData = skinManager.playerSkins[curUser.playerSkinInd];
            curLives = curUser.lives;
        }
        SetData(curData);

    }

    protected override void SpawnSkin()
    {
        base.SpawnSkin();
        //get sound
        if (curSkin)
            soundFX = curSkin.GetComponent<PlayerSoundFX>();
    }

    public override void SetData(UnitData _skinData)
    {
        base.SetData(_skinData);

        //update data
        if (skinManager)
        {
            if (skinManager.playerSkins.IndexOf((PlayerSkinData)curData) != -1)
                dataManager.SetPlayerSkinData(skinManager.playerSkins.IndexOf((PlayerSkinData)curData));
            else
                Debug.Log("Make sure you add " + _skinData + " to the skin manager: " + skinManager + "!");
        }
    }

    public void AddPoints(int _amount)
    {
        curPoints += _amount;
        if (UI)
            UI.SetPointsValue(curPoints);
    }

    public override void RefreshUIValues()
    {
        base.RefreshUIValues();
        if (UI)
        {
            UI.SetLivesValue(curLives);
            UI.SetPointsValue(curPoints);
        }
    }

    public override void Die(string _reason)
    {
        base.Die();

        //play death sound
        if (soundFX)
            soundFX.PlayDeathSound();
    }

    protected override void ResetUnitPosition(Vector2 _pos)
    {
        var pos = spawnManager.GetCurCheckPointPos();
        if (pos != default(Vector2))
            base.ResetUnitPosition(pos);
        else
            Debug.LogError("Failed to respawn player!");
    }

}
