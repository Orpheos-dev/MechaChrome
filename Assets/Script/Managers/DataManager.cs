using UnityEngine;
using UnityEngine.SceneManagement;

public class DataManager : MonoBehaviour
{
    public static DataManager Instance;
    private LevelGenerator levelManager;
    private PlayerManager playerManager;
    private EnemyManager enemyManager;
	private CameraManager cameraManager;
	
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }

        levelManager = LevelGenerator.Instance;
		enemyManager = EnemyManager.Instance;
        playerManager = PlayerManager.Instance;
		Debug.Log("levelManager: " + levelManager);
		Debug.Log("enemyManager: " + enemyManager);
		Debug.Log("playerManager: " + playerManager);	
		Debug.Log("cameraManager: " + playerManager);	
    }
	
	// Subscribe to sceneLoaded event
	private void OnEnable()
	{
		SceneManager.sceneLoaded += OnSceneLoaded;
	}
	
	// Unsubscribe from sceneLoaded event
	private void OnDisable()
	{
		SceneManager.sceneLoaded -= OnSceneLoaded;
	}
	
	// Start the game if the loaded scene is "Level"
	private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
	{
		if (scene.name == "Level")
		{
			StartGame();
		}
	}
	
	// Initialize level, enemies, and player for a new game
	public void StartGame()
	{
		Debug.Log("StartGame called");
		InitializeLevel();
		InitializeEnemies(0);
		InitializePlayer();
	
		// Set the camera mode (ensure CameraMode is defined in CameraManager)
		CameraManager.Instance.CurrentMode = CameraManager.CameraMode.FollowPlayer;
		// Declare and set the player's transform
		Transform _actualPlayerTransform = PlayerManager.Instance.currentPlayer.transform;
	}
	
	// Initialize the level using LevelGenerator
    public void InitializeLevel()
    {
        levelManager.InitializeLevel();
    }
	
	// Initialize enemies using EnemyManager
	public void InitializeEnemies(int enemyCount)
    {
        enemyManager.InitializeEnemies(enemyCount);
    }

	// Initialize the player using PlayerManager
    public void InitializePlayer()
    {
        playerManager.InitializePlayer();
    }
}
