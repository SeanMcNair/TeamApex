using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System.IO;

[CreateAssetMenu(fileName = "UserDataManager", menuName = "Data/Managers/UserDataManager", order = 1)]
public class UserDataManager : ScriptableObject
{
    [System.Serializable]
    public class UserContainer
    {
        public int curUserId;
        public List<User> users = new List<User>();
    }

    [SerializeField] private int maxUsers = 4;
    public int MaxUsers { get { return maxUsers; } }
    [SerializeField] private int maxItems = 2;
    [SerializeField] private bool setDefaultItems;
    [SerializeField] private ItemDataManager itemManager;
    [SerializeField] private ItemProperty[] items;
    [SerializeField] private UserContainer userContainer;
    public int CurUserID { get { return Mathf.Clamp(userContainer.curUserId, 0, userContainer.users.Count - 1); } }
    public List<User> Users { get { return userContainer.users; } }
    private string userNameInput;
    public string UserNameInput { get { return userNameInput; } set { userNameInput = value; } }
    private string jsonPath;
    public string JsonPath { get { return jsonPath; } set { jsonPath = value; } }

    #region DATA_WRITING

    public void InitializeData()
    {
        jsonPath = Application.streamingAssetsPath + "/UserManagerData.json";
        GetDataFromFile();
    }

    void GetDataFromFile()
    {
        if (!File.Exists(jsonPath))
            WriteDataToFile();
        else
            ReadDataFromFile();
    }

    void WriteDataToFile()
    {
        StreamWriter writer = new StreamWriter(jsonPath);
        string jsonData = StringSerializer.Serialize(typeof(UserContainer), userContainer);
        writer.Write(jsonData);
        writer.Close();
        Debug.Log("Saved Data To File");
    }

    void ReadDataFromFile()
    {
        //get data from json
        StreamReader reader = new StreamReader(jsonPath);
        string jsonData = reader.ReadToEnd();
        userContainer = StringSerializer.Deserialize(typeof(UserContainer), jsonData) as UserContainer;
        //close reader
        reader.Close();
        Debug.Log("Reading data from file");
    }

    #endregion

    #region USER MANAGEMENT

    public void CreateUser(int _saveSlotInd)
    {
        if (NameExists(userNameInput))
            return;
        if (userContainer.users.Count >= maxUsers)
        {
            Debug.Log("max capacity reached for users! Delete a user!");
            return;
        }

        User user = new User
        {
            userId = userContainer.users.Count,
            playerName = userNameInput,
            saveSlotId = _saveSlotInd,
            levelUnlocked = 1,
            inventoryItems = new ItemProperty[maxItems],

        };
        if (setDefaultItems)
        {
            items.CopyTo(user.inventoryItems,0);
        }
        userContainer.curUserId = user.userId;
        userContainer.users.Add(user);

        WriteDataToFile();
    }

    public void RemoveUser(int _saveSlotInd)
    {
        User userToRemove = new User();
        foreach (var user in userContainer.users)
        {
            if (user.saveSlotId == _saveSlotInd)
                userToRemove = user;
        }
        userContainer.users.Remove(userToRemove);
        WriteDataToFile();
    }

    public void EraseAllUsers()
    {
        userContainer.users.Clear();

        WriteDataToFile();
    }

    public void SetCurUser(int _id)
    {
        userContainer.curUserId = _id;

        WriteDataToFile();
    }

    public User GetCurUser()
    {
        return userContainer.users[CurUserID];
    }

    public User GetUser(int _ind)
    {
        return userContainer.users[_ind];
    }

    public string[] GetUserNames()
    {
        var names = new string[userContainer.users.Count];
        for (int i = 0; i < names.Length; i++)
        {
            names[i] = userContainer.users[i].playerName;
        }
        return names;
    }

    #endregion

    #region USER NAME

    public string GetPlayerName()
    {
        return userContainer.users[userContainer.curUserId].playerName;
    }

    bool NameExists(string _name)
    {
        foreach (var user in userContainer.users)
        {
            if (user.playerName == _name)
            {
                Debug.Log("Name Already Exists!");
                return true;
            }

        }
        return false;
    }

    public void SetUserName(string _name)
    {
        userNameInput = _name;
    }

    #endregion

    #region LEVELS/CHECKPOINTS

    public void SaveLevelName(string _levelName)
    {
        GetCurUser().levelName = _levelName;
        WriteDataToFile();
    }

    public string GetCurLevelName()
    {
        return GetCurUser().levelName;
    }

    public void SaveCheckPoint(int _checkPoint)
    {
        GetCurUser().curCheckPoint = _checkPoint;
        WriteDataToFile();
    }

    public int GetCurCheckPoint()
    {
        return GetCurUser().curCheckPoint;
    }

    public void UnlockLevel(int _level)
    {
        GetCurUser().levelUnlocked = _level;

        WriteDataToFile();
    }

    #endregion

    #region SKIN

    public void SetPlayerSkinData(int _skinInd)
    {
        userContainer.users[userContainer.curUserId].playerSkinInd = _skinInd;

        WriteDataToFile();
    }

    public PlayerSkinData GetSkinData()
    {
        PlayerSkinManager sm = GameManager.instance.GetSkinManager();
        return sm.playerSkins[userContainer.users[userContainer.curUserId].playerSkinInd];
    }

    #endregion

    #region COINS

    //public void Set

    #endregion

    #region INVENTORY

    public void SetInventoryItems(ItemProperty[] _invItems, int _userInd)
    {
        GetUser(_userInd).inventoryItems = new ItemProperty[_invItems.Length];
        for (int i = 0; i < _invItems.Length; i++)
        {
            GetUser(_userInd).inventoryItems[i] = _invItems[i];
        }
        Debug.Log("Saving quick menu items");

        WriteDataToFile();
    }

    public ItemProperty[] GetInventoryItems()
    {
        //need inventory system in the future
        return GetCurUser().inventoryItems;
    }

    public ItemProperty[] GetInventoryItems(int _userInd)
    {
        //need inventory system in the future
        return userContainer.users[_userInd].inventoryItems;
    }

    #endregion
}
