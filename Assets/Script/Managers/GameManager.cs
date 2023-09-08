using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
	public GameObject uiManager;
	private LevelGenerator levelGenerator;

    public enum FinishCondition { DefeatAllEnemies, CollectAllItems }

    [System.Serializable]
    public class GameState
    {
        public bool Paused; 
        public bool GameOver; 
        public bool Playing; 
		public bool Continue;
    }

    [System.Serializable]
    public class LevelProgression
    {
        public int currentFloor = 1;
        public int bossFloor = 1;
		[Range(0, 10)]
		public float GROWTH_RATE = 1.5f; 
        public FinishCondition finishCondition = FinishCondition.DefeatAllEnemies;
        public GameObject FinishPrefab; 
		public Vector3 FinishPrefabOffset = new Vector3(0, 0, 0); // Finish prefab offset
   
        [Header("Rewards - Skill")]
        public GameObject skillRewardPrefab; 
        [Range(0, 1)]
        public float skillRewardFrequency;
        public Vector3 SkillPositionOffset;

        [Header("Rewards - Item")]
        public GameObject ItemRewardPrefab; 
        [Range(0, 1)]
        public float ItemRewardFrequency;
        public Vector3 ItemPositionOffset;
    }

    [System.Serializable]
    public class SpecialEvents
    {
        [Header("Special Item")]
        public GameObject forgeItemPrefab;
        [Range(0, 1)]
        public float forgeItemPercentage;
    }

    public GameState gameState;
    public LevelProgression levelProgression;
    public SpecialEvents specialEvents;

	// Use these to activate and deactivate the managers
    public void ActivateUIManager() { ActivateManager(uiManager); }
    public void DeactivateUIManager() { DeactivateManager(uiManager); }
	private bool playerAlive = true;
	private bool allEnemiesDefeated = false;
	
    private HashSet<int> usedRoomSeeds = new HashSet<int>();      // Keeps track of used room seeds
    private HashSet<int> usedDecorationSeeds = new HashSet<int>();  // Keeps track of used decoration seeds
	
	private int nextNumNormalEnemies;
	public int floorNumber = 1;
	private bool playerDiedProcessed = false;
	
	[Header("Enemy progression count")]
	public int nextFloorEnemyCount;  
	public int NumNormalEnemies;
	public int NumSecretEnemies;
	public int NumBossEnemies;
	
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
    }

	// heck game conditions (e.g., win/lose conditions)
	private void CheckGameConditions()
	{
		if(!allEnemiesDefeated)
		{
			Debug.Log("All enemies have not been defeated yet.");
		}
		else
		{
			Debug.Log("All enemies have been defeated.");
		}
	
		if(levelProgression.finishCondition == FinishCondition.DefeatAllEnemies)
		{
			Debug.Log("Finish condition is set to DefeatAllEnemies.");
	
			if (allEnemiesDefeated)
			{
				Debug.Log("Conditions met. All enemies defeated with finish condition set to DefeatAllEnemies.");
				SpawnFinishPrefabs();
				SpawnSkillRewards();
			}
			else
			{
				Debug.Log("Conditions not met. Even though finish condition is set to DefeatAllEnemies, not all enemies have been defeated.");
			}
		}
		else
		{
			Debug.Log("Finish condition is not set to DefeatAllEnemies.");
		}
	
		// If player is alive and their health reaches 0, call PlayerDied
		if (playerAlive && GetPlayerHealth() <= 0)
		{
			playerAlive = false;
			Debug.Log("Player has died. Handling death logic.");
			PlayerDied();
		}
	}
	
	// activate a manager (e.g., UI Manager)
    public void ActivateManager(GameObject manager)
    {
        manager.SetActive(true);
    }

	// deactivate a manager (e.g., UI Manager)
    public void DeactivateManager(GameObject manager)
    {
        manager.SetActive(false);
    }

	// handle winning the game
	public void WinGame()
    {
        ResetGameState();
        gameState.Continue = true;
        Debug.Log("Game won method called");
        SpawnFinishPrefabs();
    }

	// spawn finish prefabs (e.g., exit doors)
	private void SpawnFinishPrefabs()
	{
		GameObject[] finishDoors = GameObject.FindGameObjectsWithTag("Finish");
		if (finishDoors.Length == 0)
		{
			return;
		}
		Debug.Log("Found " + finishDoors.Length + " finish doors.");
		foreach(GameObject door in finishDoors)
		{
			Vector3 spawnPosition = door.transform.position + door.transform.forward + levelProgression.FinishPrefabOffset;
			Quaternion spawnRotation = door.transform.rotation; // Consider the rotation of the door
			Instantiate(levelProgression.FinishPrefab, spawnPosition, spawnRotation);
		}
	}
	
	// spawn skill rewards
	private void SpawnSkillRewards()
	{
		// Check the frequency condition for spawning the skill reward
		if (Random.value <= levelProgression.skillRewardFrequency)
		{
			// Find the player's position
			GameObject player = GameObject.FindGameObjectWithTag("Player");
			if (player != null)
			{
				Vector3 spawnPosition = player.transform.position + levelProgression.SkillPositionOffset;
				Instantiate(levelProgression.skillRewardPrefab, spawnPosition, Quaternion.identity);
			}
			else
			{
				Debug.LogError("Player not found! Cannot spawn skill reward near player.");
			}
		}
	}
	
	// continue the game after winning
	public void ContinueGame()
    {
        SetGameStateToPlaying(); // Just setting the specific state to true
    }
	
	// pause the game
	public void PauseGame()
    {
        SetGameStateToPaused();
        Time.timeScale = 0f;
    }
	
	// handle losing the game
	public void LoseGame()
    {
        ResetGameState();
        gameState.GameOver = true;
    }
	
	// resume the game after pausing
	public void ResumeGame()
    {
        SetGameStateToPlaying();
        Time.timeScale = 1f;
    }
	
	// set to game stat playing
	private void SetGameStateToPlaying()
    {
        ResetGameState();
        gameState.Playing = true;
    }
	
	// set to game stat pause
	private void SetGameStateToPaused()
    {
        ResetGameState();
        gameState.Paused = true;
    }
	
	// reset game states
	private void ResetGameState()
    {
        gameState.Playing = false;
        gameState.Paused = false;
        gameState.GameOver = false;
        gameState.Continue = false;

        playerDiedProcessed = false; // Resetting playerDiedProcessed when the game state resets
    }
	
	// get the player's health (placeholder)
	private float GetPlayerHealth()
	{
		return 100f; // For now, returning a dummy value to avoid errors.
	}
	
	// handle player death
	public void PlayerDied()
    {
        if (playerDiedProcessed) return; // Moved to the top to immediately return if it's already processed

        // Handle game-wide consequences of player death here
        UIManager.Instance.ShowDeadCanvas();
        Debug.Log("Game Over. Player has died.");
        UIManager.SceneCanvasPair levelSceneCanvasPair = UIManager.Instance.sceneCanvasList.Find(pair => pair.sceneName == "Level");
        if (levelSceneCanvasPair != null && levelSceneCanvasPair.sceneCanvases.Count > 0)
        {
            levelSceneCanvasPair.sceneCanvases[0].SetActive(true);
        }

        playerDiedProcessed = true;
		EnemyManager.Instance.StopCheckForPlayer();
		SpawnEffect.Instance.OnPlayerDied();
    }

	// handle all enemies being defeated
	public void AllEnemiesDefeated()
    {
        allEnemiesDefeated = true;
        Debug.Log("All enemies have been defeated!");
		CheckGameConditions();
    }
	
	// handle player colliding with the finish prefab
	public void HandleFinishCollision()
    {
        TransitionToNextFloor();

        if (levelGenerator != null)
        {
            // Set new seeds for the levelGenerator
            levelGenerator.roomSeed = GenerateUniqueSeed(usedRoomSeeds);
            levelGenerator.decorationSeed = GenerateUniqueSeed(usedDecorationSeeds);
        }
		
		int newEnemyCount = CalculateEnemyCountForFloor(levelProgression.currentFloor);
		
        StartCoroutine(ReloadLevel());
    }

	// Generate unique key for Level
    private int GenerateUniqueSeed(HashSet<int> usedSeeds)
    {
        int newSeed;
        do
        {
            newSeed = Random.Range(0, 1001);
        } while (usedSeeds.Contains(newSeed));
        
        usedSeeds.Add(newSeed);
        return newSeed;
    }
	
	// Calculate enemy count for the floor
	public int CalculateEnemyCountForFloor(int floor)
	{
		int baseNumNormalEnemies = EnemyManager.Instance.GetInitialNormalEnemies();
		int nextNumNormalEnemies = Mathf.RoundToInt(baseNumNormalEnemies * Mathf.Pow(levelProgression.GROWTH_RATE, (floor - 1)));
		return nextNumNormalEnemies;
	}

	// Coroutine to reload the level
    private IEnumerator ReloadLevel()
    {
        yield return new WaitForSeconds(1f); // Wait for 1 second before reloading the level
        SceneManager.LoadScene("Level");    // Reload the level
    }
	
	// transition to the next floor/level
	public void TransitionToNextFloor()
	{
		levelProgression.currentFloor++;
		int nextFloorEnemyCount = CalculateEnemyCountForFloor(levelProgression.currentFloor);
		UpdateNormalEnemiesCount(nextFloorEnemyCount);
	}
	
	// update the number of normal enemies for the next floor
	public void UpdateNormalEnemiesCount(int newCount)
	{
		NumNormalEnemies = newCount;
	}
	
	 // prepare for the next floor
	public void PrepareForNextFloor()
	{
		int nextFloor = GameManager.Instance.levelProgression.currentFloor + 1;
		int newEnemyCount = GameManager.Instance.CalculateEnemyCountForFloor(nextFloor);
		
		NumNormalEnemies = newEnemyCount;
		EnemyManager.Instance.initialNormalEnemies = newEnemyCount;  // Access it through EnemyManager instance
		
		GameManager.Instance.nextFloorEnemyCount = newEnemyCount;
		Debug.Log($"Prepared for next floor. Next floor's enemy count: " + GameManager.Instance.nextFloorEnemyCount);
	}
	
	// increase the floor number
    public void IncreaseFloor()
    {
        floorNumber++;
    }
}
