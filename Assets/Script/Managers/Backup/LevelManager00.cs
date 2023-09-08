//using System.Collections.Generic;
//using UnityEngine;
//
//public class LevelManager : MonoBehaviour
//{
//	private GameManager gameManager; // reference to the GameManager
//    // Array to hold block prefabs
//    public GameObject[] blockPrefabs;
//	// Array to hold enemy prefabs
//	public GameObject[] enemyPrefabs;
//	// Variable to hold the finish object prefab
//    public GameObject finishPrefab;
//	// Variable to hold the player
//	public GameObject player;
//	// Variable to hold the current skill box object
//	public GameObject SkillBoxPrefab;
//
//    // Lists to store current blocks and enemies
//    private List<GameObject> currentBlocks = new List<GameObject>();
//    private List<GameObject> currentEnemies = new List<GameObject>();
//	// Variable to track the current exit object
//    private GameObject currentFinish;
//	private GameObject currentSkillBox;
//	private bool skillBoxSpawned = false; // Track if the skill box has been spawned
//
//    // Variable to track the current floor number
//    private int currentFloorNumber = 1;
//	// The size of the grid
//	private int gridSize = 4;
//	// The distance between blocks
//	private float blockSpacing = 200.0f;
//	// center of the array
//	private Vector3 levelCenter = new Vector3(360, 45, 580);
//	// Add a reference to store the initial position
//    private Vector3 initialPlayerPosition;
//	
//	
//	private void Awake()
//    {
//		// add any logic needed at start
//    }
//	
//    void Start()
//	{
//		// Store the initial player position
//		initialPlayerPosition = player.transform.position;
//		
//		GenerateLevel();
//	}
//	
//	// Method to place a block at a given position, connecting to a given connector
//	private void PlaceBlock(Vector3 position, Transform connector)
//	{
//		// Use the FindConnectableBlock method to select a block
//		GameObject blockToPlace = FindConnectableBlock(connector);
//		
//		if (blockToPlace == null)
//		{
//			// If no connectable block is found, fall back to placing a random block
//			int randomIndex = Random.Range(0, blockPrefabs.Length);
//			blockToPlace = blockPrefabs[randomIndex];
//		}
//	
//		// Find the connector on the new block that will be connected
//		Transform blockConnector = blockToPlace.GetComponent<Block>().connectors[0];
//	
//		// Calculate the rotation needed for the new block's connector to align with the given connector
//		Quaternion rotation = Quaternion.FromToRotation(blockConnector.forward, -connector.forward);
//	
//		// Instantiate the block and add it to our currentBlocks list
//		GameObject blockInstance = Instantiate(blockToPlace, position, rotation);
//		//Debug.Log($"Placed a block at {position}, using a block prefab of type {blockToPlace.name}");
//		currentBlocks.Add(blockInstance);
//	}
//	
//	// A method to find a block that can connect to a given connector
//	private GameObject FindConnectableBlock(Transform connector)
//	{
//		List<GameObject> connectableBlocks = new List<GameObject>();
//	
//		// Loop through all block prefabs
//		foreach (GameObject blockPrefab in blockPrefabs)
//		{
//			Block block = blockPrefab.GetComponent<Block>();
//	
//			// Loop through all connectors of this block
//			foreach (Transform blockConnector in block.connectors)
//			{
//				// Check if this block's connector can connect to the given connector
//				// Here we just check if they are facing opposite directions, 
//				// you may need more complex logic depending on your game's design
//				if (Vector3.Dot(connector.forward, blockConnector.forward) < -0.9f)
//				{
//					connectableBlocks.Add(blockPrefab);
//					break;
//				}
//			}
//		}
//	
//		// Return a random block from the connectable blocks
//		if (connectableBlocks.Count > 0)
//		{
//			int randomIndex = Random.Range(0, connectableBlocks.Count);
//			//Debug.Log($"Selected a connectable block of type {connectableBlocks[randomIndex].name} for a connector at {connector.position}");
//			return connectableBlocks[randomIndex];
//		}
//	
//		// If no connectable blocks are found, return null
//		Debug.Log("No connectable block found");
//		return null;
//	}
//	
//	// Method to place an enemy at a given position
//	private void PlaceEnemy(Vector3 position)
//	{
//		// Select a random enemy from the prefabs
//		int randomIndex = Random.Range(0, enemyPrefabs.Length);
//		GameObject enemyToPlace = enemyPrefabs[randomIndex];
//		
//		// Instantiate the enemy and add it to our currentEnemies list
//		GameObject enemyInstance = Instantiate(enemyToPlace, position, Quaternion.identity);
//		
//		// Assign this LevelManager instance to the Enemy
//		Enemy enemyScript = enemyInstance.GetComponent<Enemy>();
//		if (enemyScript != null)
//		{
//			enemyScript.SetLevelManager(this);
//		}
//	
//		currentEnemies.Add(enemyInstance);
//	}
//	
//	private void GenerateLevel()
//	{
//		//Debug.Log("In LevelManager.GenerateLevel");
//		//Debug.Log("blockPrefabs is " + (blockPrefabs == null ? "null" : "not null"));
//		//Debug.Log("enemyPrefabs is " + (enemyPrefabs == null ? "null" : "not null"));
//		//Debug.Log("finishPrefab is " + (finishPrefab == null ? "null" : "not null"));
//		//Debug.Log("player is " + (player == null ? "null" : "not null"));
//		
//		// Calculate an offset to center the grid
//		float offset = -(gridSize * blockSpacing) / 2.0f;
//	
//		// Loop over the grid to place blocks
//		for (int x = 0; x < gridSize; x++)
//		{
//			for (int y = 0; y < gridSize; y++)
//			{
//				// Calculate the position for this block, using the offset
//				Vector3 position = new Vector3(levelCenter.x + offset + x * blockSpacing, levelCenter.y, levelCenter.z + offset + y * blockSpacing);
//				
//				// If this is the first block, place it randomly
//				if (x == 0 && y == 0)
//				{
//					// Select a random block from the prefabs
//					int randomIndex = Random.Range(0, blockPrefabs.Length);
//					GameObject blockToPlace = blockPrefabs[randomIndex];
//					
//					// Instantiate the block and add it to our currentBlocks list
//					GameObject blockInstance = Instantiate(blockToPlace, position, Quaternion.identity);
//					currentBlocks.Add(blockInstance);
//				}
//				else
//				{
//					// For subsequent blocks, try to connect them to a connector on the previously placed block
//					Block previousBlock = currentBlocks[currentBlocks.Count - 1].GetComponent<Block>();
//					if (previousBlock == null)
//					{
//						//Debug.LogError("previousBlock is null");
//					}
//					else if (previousBlock.connectors == null)
//					{
//						//Debug.LogError("previousBlock.connectors is null");
//					}
//					
//					// Choose a connector on the previous block
//					Transform connector = previousBlock.connectors[Random.Range(0, previousBlock.connectors.Count)];
//	
//					// Place a block at this position, connecting to the chosen connector
//					PlaceBlock(position, connector);
//				}
//			}
//		}
//	
//		// Determine the number of enemies to place based on the current floor number
//		int numEnemies = 2 * currentFloorNumber;
//	
//		// Loop to place enemies
//		for (int i = 0; i < numEnemies; i++)
//		{
//			// Pick a random block
//			GameObject randomBlock = currentBlocks[Random.Range(0, currentBlocks.Count)];
//			
//			// Calculate random offset
//			Vector3 enemyOffset = new Vector3(blockSpacing / 2.0f, 0, blockSpacing / 2.0f);
//			enemyOffset = new Vector3(enemyOffset.x * (Random.Range(0, 2) == 0 ? 1 : -1), 0, enemyOffset.z * (Random.Range(0, 2) == 0 ? 1 : -1));
//
//			// Calculate the position for this enemy, using the offset
//			Vector3 enemyPosition = randomBlock.transform.position + enemyOffset;
//
//			// Place an enemy at this position
//			PlaceEnemy(enemyPosition);
//		}
//	}
//
//	public void RemoveEnemy(GameObject enemy)
//	{
//		if (currentEnemies.Contains(enemy))
//		{
//			currentEnemies.Remove(enemy);
//		}
//	}
//
//   void Update()
//    {
//        // Check if all enemies have been defeated
//        if (currentEnemies.Count == 0)
//        {
//            // If there is no current skill box and it hasn't been spawned before, spawn it
//            if (currentSkillBox == null && !skillBoxSpawned)
//            {
//                SpawnSkillBox();
//                skillBoxSpawned = true; // Update the skill box spawn status
//            }
//
//            // If there is no current exit object, spawn one
//            if (currentFinish == null)
//            {
//                SpawnFinish();
//            }
//        }
//    }
//	
//    // Method to spawn the finish object
//	private void SpawnFinish()
//    {
//        // Choose a position for the finish object
//        Vector3 finishPosition = new Vector3(340, 45, 950);  // Replace with the position you want
//
//        // Instantiate the finish object
//        currentFinish = Instantiate(finishPrefab, finishPosition, Quaternion.identity);
//        currentFinish.tag = "Finish";
//    }
//	
//	// Method to spawn the skill box object
//	private void SpawnSkillBox()
//	{
//		// Choose a position for the skill box object
//		Vector3 skillBoxPosition = new Vector3(180, 50, 876);
//	
//		// If there is a current skill box, destroy it
//		if(currentSkillBox != null) {
//			Debug.Log("Destroying current skill box.");
//			Destroy(currentSkillBox);
//		} else {
//			Debug.Log("currentSkillBox is null.");
//		}
//	
//		// Instantiate the skill box object using the SkillBoxPrefab
//		currentSkillBox = Instantiate(SkillBoxPrefab, skillBoxPosition, Quaternion.identity);
//		currentSkillBox.tag = "Reward";
//	}
//	
//	
//	// Method to clear the level
//    private void ClearLevel()
//    {
//        // Loop through and destroy each block in the level
//        foreach (GameObject block in currentBlocks)
//        {
//            Destroy(block);
//        }
//
//        // Clear the list of blocks
//        currentBlocks.Clear();
//
//        // Loop through and destroy each enemy in the level
//        foreach (GameObject enemy in currentEnemies)
//        {
//            Destroy(enemy);
//        }
//
//        // Clear the list of enemies
//        currentEnemies.Clear();
//    }
//
//	private void ResetPlayerPosition()
//	{
//		// Get the character controller component
//		CharacterController characterController = player.GetComponent<CharacterController>();
//	
//		// If there is a character controller, disable it temporarily
//		if (characterController != null)
//		{
//			characterController.enabled = false;
//		}
//	
//		// Set the player's position to the respawn position
//		player.transform.position = initialPlayerPosition;
//	
//		// If there was a character controller, re-enable it
//		if (characterController != null)
//		{
//			characterController.enabled = true;
//		}
//	}
//	
//    // Method to progress to the next floor
//	public void NextFloor()
//	{
//		// Clear the current level
//		ClearLevel();
//		// Increase the floor number
//		currentFloorNumber++;
//		// Generate the next level
//		GenerateLevel();
//		
//		// Destroy the current exit object
//		if (currentFinish != null)
//		{
//			Destroy(currentFinish);
//			currentFinish = null;
//		}
//	
//		// Reset skill box spawn status
//		skillBoxSpawned = false;
//		// Reset player's position
//		ResetPlayerPosition();
//		// Add some XP to the player's total
//        GameManager.Instance.playerXP += 10;
//		// Add some currency to the player's total
//		GameManager.Instance.playerCurrency += 50;
//	}
//	
//}
//