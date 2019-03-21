using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    public static GameManager instance;

    [SerializeField] private UserDataManager userData;
    [SerializeField] private PlayerSkinManager skinManager;
    [SerializeField] private SpawnCheckPointManager spawnManager;
    [SerializeField] private bool initializeSpawnManagerOnStart = true;
    [SerializeField] private SceneTransitionData sceneTransData;
    [SerializeField] private SpawnPoolManager poolManagerData;
    [SerializeField] private float killHeight = -20;
    [SerializeField] private MenuManager menuManager;
    [SerializeField] private MusicManager musicManager;

    private Player spawnedPlayer;
    public Player SpawnedPlayer { get { return spawnedPlayer; } set { spawnedPlayer = value; } }
    private bool won;
    public bool IsWon { get { return won; } }
    private bool gameOver;
    public bool IsGameOver { get { return gameOver; } }
	// Use this for initialization
	void Awake ()
    {
        instance = this;
        if (poolManagerData)
            poolManagerData.Initialize();
	}

    void Start()
    {
        gameOver = false;
        BeginMusic();
        if (userData)
            userData.InitializeData();
        if (spawnManager)
        {
            if (initializeSpawnManagerOnStart)
                spawnManager.Initialize();
        }
            
    }
	
    void BeginMusic()
    {
        if (musicManager)
            //play music
            musicManager.PlayBackgroundMusic();
    }

    public void LevelWin(string _nextScene, float _endTime, bool _freezeGame)
    {
        StartCoroutine(StartLevelWin(_nextScene, _endTime, _freezeGame));
    }

    IEnumerator StartLevelWin(string _nextScene, float _endTime, bool _freezeGame)
    {
        won = true;

        //do UI effects

        //freeze game
        float curScale = Time.timeScale;
        if (_freezeGame)
        {
            Time.timeScale = 0;
        }
            
        //play win music
        if (musicManager)
            musicManager.PlayGameOverWinMusic();

        //wait to load next level
        yield return new WaitForSecondsRealtime(_endTime);

        Time.timeScale = curScale;

        //load level
        sceneTransData.LoadLevelWithLoadingScreen(_nextScene);

    }

    public void GameOverLose()
    {
        gameOver = true;
        userData.SaveCheckPoint(0);
        musicManager.PlayGameOverLoseMusic();
        sceneTransData.LoadGameOverScene();
    }

    public UserDataManager GetUserDataManager()
    {
        return userData;
    }

    public PlayerSkinManager GetSkinManager()
    {
        return skinManager;
    }

    public SpawnCheckPointManager GetSpawnManager()
    {
        return spawnManager;
    }

    public SceneTransitionData GetSceneTransitionData()
    {
        return sceneTransData;
    }

    public MenuManager GetMenuManager()
    {
        return menuManager;
    }

    public MusicManager GetMusicManager()
    {
        return musicManager;
    }

    public float GetKillHeight()
    {
        return killHeight;
    }

}
