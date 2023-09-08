//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//
//public class LevelGenerator : MonoBehaviour
//{
//	public static LevelGenerator Instance;
//	
//    private TileType[,,] grid;
//    private List<Vector3> doorPositions;
//    private List<Vector3> windowPositions;
//    private HashSet<Vector3> doorTiles;
//	private HashSet<Vector3> windowTiles;
//    private List<Vector3> wallPositions;
//	private List<Vector3> floorPositions;
//
//    [System.Serializable]
//    public class TileType
//    {
//        [Range(1, 10)]
//        public int numToUse;
//        public GameObject prefab;
//        public Vector3 positionOffset;
//        public Vector3 rotationOffset;
//        public bool allowDecoration;
//    }
//
//    [System.Serializable]
//    public class Decoration
//    {
//        public GameObject prefab;
//    }
//
//    [System.Serializable]
//    public class AreaProperties
//    {
//        [Range(0, 10)]
//        public int numWindows;
//        [Range(0, 10)]
//        public int numDoors;
//        public float wallDecorationOffset;
//        public float floorDecorationOffset;
//		[Range(0, 1)]
//		public float floorDecorationProbability; 
//        [Range(0, 10)]
//        public float decorationSafeArea;
//        [Range(1, 10)]
//        public float pointSpacing;
//
//    }
//	
//	[System.Serializable]
//    public class SpawnPoints
//    {	
//		public EnemyManager enemyManager;
//        public bool allowEnemies;
//		public PlayerManager playerManager;
//        public bool allowPlayer;
//    }
//
//    [Header("Room Size")]
//    [Range(1, 100)]
//    public int gridX;
//    [Range(1, 100)]
//    public int gridY;
//    [Range(1, 100)]
//    public int gridZ;
//    public int tileSize;
//
//    [Header("Tiles")]
//    public List<TileType> floor;
//    public List<TileType> wall;
//    public List<TileType> door;
//    public List<TileType> window;
//
//    [Header("Decorations")]
//    public List<Decoration> wallDecorations;
//    public List<Decoration> floorDecorations;
//
//    [Header("Area Properties & spawn")]
//    public AreaProperties areaProperties;
//	public SpawnPoints spawnProperties;
//
//    [Header("Seeds")]
//    [Range(0, 10)]
//    public int roomSeed;
//    [Range(0, 10)]
//    public int decorationSeed;
//	
//	private HashSet<Vector3> floorDecorationPositions = new HashSet<Vector3>();
//	
//	private bool isInitialized = false;
//	
//	private void OnEnable()
//	{
//		Debug.Log("LevelGenerator started");
//		wallPositions = new List<Vector3>();
//		System.Random random = new System.Random(roomSeed);
//		System.Random decorationRandom = new System.Random(decorationSeed);
//		grid = new TileType[gridX, gridY, gridZ];
//	
//		InitializeDoorAndWindowPositions(random);
//		PlaceFloor(random);
//		PlaceWalls(random);
//		PlaceDoorsAndWindows(doorPositions, windowPositions, areaProperties.numDoors, areaProperties.numWindows, random);
//		PlaceWallDecorations(random); 
//		PlaceFloorDecorations(random);
//		
//		// Get the possible enemy positions
//		List<Vector3> possibleEnemyPositions = GetPossibleEnemyPositions();
//		
//		List<Vector3> filteredEnemyPositions = GetFilteredEnemyPositions(possibleEnemyPositions, floorDecorationPositions);
//		
//		// If enemies are allowed to spawn, call the spawn method
//		if (spawnProperties.allowEnemies && spawnProperties.enemyManager != null)
//		{
//			spawnProperties.enemyManager.SpawnEnemiesAtPositions(filteredEnemyPositions);
//		}
//		else
//		{
//			if (spawnProperties.allowEnemies && spawnProperties.enemyManager == null)
//			{
//				//Debug.LogWarning("Enemy spawning is enabled, but the EnemyManager reference is not assigned!");
//			}
//		}
//		
//		// If player is allowed to spawn, call the spawn method
//		if (spawnProperties.allowPlayer && spawnProperties.playerManager != null)
//		{
//			List<Vector3> possiblePlayerPositions = GetPossiblePlayerPositions();
//			List<Vector3> filteredPlayerPositions = GetFilteredPlayerPositions(possiblePlayerPositions);
//			spawnProperties.playerManager.SpawnPlayerAtPosition(filteredPlayerPositions); // Changed here
//		}
//		else
//		{
//			if (spawnProperties.allowPlayer && spawnProperties.playerManager == null) // Changed here
//			{
//				//Debug.LogWarning("Player spawning is enabled, but the PlayerManager reference is not assigned!");
//			}
//		}
//		
//		if (!isInitialized)
//		{
//			isInitialized = true;
//		}
//	}
//	
//	private void Awake()
//	{
//		if (Instance == null)
//		{
//			Instance = this;
//			Debug.Log("LevelGenerator instantiated.");
//		}
//		else
//		{
//			Debug.Log("Destroying extra LevelGenerator instance");
//			Debug.Log("Attempted to instantiate another LevelGenerator. Destroying the game object.");
//			Destroy(gameObject);
//		}
//	}
//	
//	
//	private void OnDisable()
//	{
//		isInitialized = false;
//	}
//	
//    
//		
//	
//	private void InitializeDoorAndWindowPositions(System.Random random)
//	{
//		doorPositions = new List<Vector3>();
//		windowPositions = new List<Vector3>();
//		doorTiles = new HashSet<Vector3>();
//		windowTiles = new HashSet<Vector3>();
//	
//		List<Vector3> validDoorAndWindowPositions = new List<Vector3>();
//		for (int x = 0; x < gridX; x++)
//		{
//			for (int z = 0; z < gridZ; z++)
//			{
//				if ((x == 0 || x == gridX - 1 || z == 0 || z == gridZ - 1) // Exclude interior positions
//					&& !(x == 0 && z == 0)
//					&& !(x == gridX - 1 && z == 0)
//					&& !(x == 0 && z == gridZ - 1)
//					&& !(x == gridX - 1 && z == gridZ - 1)) // Exclude exact corners
//				{
//					validDoorAndWindowPositions.Add(new Vector3(x * tileSize, 0, z * tileSize));
//				}
//			}
//		}
//		
//		for (int i = 0; i < areaProperties.numDoors; i++)
//		{
//			if (validDoorAndWindowPositions.Count == 0) break; // No more valid positions
//			int randomIndex = random.Next(validDoorAndWindowPositions.Count);
//			Vector3 doorPosition = validDoorAndWindowPositions[randomIndex];
//			doorPositions.Add(doorPosition);
//			doorTiles.Add(doorPosition); // Ensure this position is skipped when placing walls
//			validDoorAndWindowPositions.RemoveAt(randomIndex); // Remove chosen position from valid positions
//		}
//	
//		for (int i = 0; i < areaProperties.numWindows; i++)
//		{
//			if (validDoorAndWindowPositions.Count == 0) break; // No more valid positions
//			int randomIndex = random.Next(validDoorAndWindowPositions.Count);
//			Vector3 windowPosition = validDoorAndWindowPositions[randomIndex];
//			windowPositions.Add(windowPosition);
//			windowTiles.Add(windowPosition); // Ensure this position is skipped when placing walls
//			validDoorAndWindowPositions.RemoveAt(randomIndex); // Remove chosen position from valid positions
//		}
//	}
//	
//	private void PlaceFloor(System.Random random)
//	{
//		Vector3 position;
//		floorPositions = new List<Vector3>(); // Add this line to initialize the floorPositions list
//	
//		for (int x = 0; x < gridX; x++)
//		{
//			for (int z = 0; z < gridZ; z++)
//			{
//				int index = random.Next(floor.Count);
//				position = new Vector3(x * tileSize, 0, z * tileSize);
//				Instantiate(floor[index].prefab, position + floor[index].positionOffset, Quaternion.Euler(floor[index].rotationOffset));
//	
//				// Update the grid with the correct TileType for each floor position
//				grid[x, 0, z] = floor[index];
//	
//				// Add the floor position to the floorPositions list for floor decorations
//				floorPositions.Add(position);
//			}
//		}
//	}
//	
//    private void PlaceWalls(System.Random random)
//	{
//		// Check if there are any walls in the collection
//		if (wall == null || wall.Count == 0)
//		{
//			// No walls to place, so just return.
//			return;
//		}
//	
//		for (int y = 0; y < gridY; y++)
//		{
//			for (int x = 0; x < gridX; x++)
//			{
//				for (int z = 0; z < gridZ; z++)
//				{
//					Vector3 position = new Vector3(x * tileSize, y * tileSize, z * tileSize);
//	
//					if (doorTiles.Contains(position) || windowTiles.Contains(position)) continue;
//	
//					bool isCorner = (x == 0 && z == 0) || (x == gridX - 1 && z == 0) || (x == 0 && z == gridZ - 1) || (x == gridX - 1 && z == gridZ - 1);
//					bool isEdge = x == 0 || x == gridX - 1 || z == 0 || z == gridZ - 1;
//					int index = random.Next(wall.Count);
//	
//					if (isCorner)
//					{
//						List<Quaternion> rotations = new List<Quaternion>();
//						if (x == 0 && z == 0) { rotations.Add(Quaternion.Euler(0, 90, 0)); rotations.Add(Quaternion.Euler(0, 0, 0)); }
//						if (x == gridX - 1 && z == 0) { rotations.Add(Quaternion.Euler(0, 0, 0)); rotations.Add(Quaternion.Euler(0, -90, 0)); }
//						if (x == 0 && z == gridZ - 1) { rotations.Add(Quaternion.Euler(0, 90, 0)); rotations.Add(Quaternion.Euler(0, 180, 0)); }
//						if (x == gridX - 1 && z == gridZ - 1) { rotations.Add(Quaternion.Euler(0, 270, 0)); rotations.Add(Quaternion.Euler(0, 180, 0)); }
//	
//						foreach (var rot in rotations)
//						{
//							Quaternion finalRotation = rot * Quaternion.Euler(wall[index].rotationOffset);
//							Vector3 finalPosition = position + (finalRotation * wall[index].positionOffset); // Apply offset based on rotation
//							Instantiate(wall[index].prefab, finalPosition, finalRotation);
//						}
//					}
//					else if (isEdge)
//					{
//						Quaternion rotation = Quaternion.identity;
//						if (x == 0) rotation = Quaternion.Euler(0, 90, 0);
//						else if (x == gridX - 1) rotation = Quaternion.Euler(0, -90, 0);
//						else if (z == 0) rotation = Quaternion.Euler(0, 0, 0);
//						else if (z == gridZ - 1) rotation = Quaternion.Euler(0, 180, 0);
//	
//						rotation = rotation * Quaternion.Euler(wall[index].rotationOffset); // Combine rotations
//						Vector3 finalPosition = position + (rotation * wall[index].positionOffset); // Apply offset based on rotation
//						Instantiate(wall[index].prefab, finalPosition, rotation);
//	
//						if (wall[index].allowDecoration)
//						{
//							wallPositions.Add(finalPosition); // Add final position to the wallPositions list
//						}
//					}
//				}
//			}
//		}
//	}
//	
//	private void PlaceDoorsAndWindows(List<Vector3> doorPositions, List<Vector3> windowPositions, int numDoors, int numWindows, System.Random random)
//	{
//
//		for (int i = 0; i < numDoors; i++)
//		{
//			if (i >= doorPositions.Count)
//			{
//				//Debug.LogWarning("Not enough door positions defined.");
//				break;
//			}
//			int index = door.Count > 0 ? random.Next(door.Count) : 0;
//			Vector3 position = doorPositions[i];
//			Quaternion rotation = DetermineRotation(position);
//			Quaternion finalRotation = rotation * Quaternion.Euler(door[index].rotationOffset);
//			Vector3 finalPosition = position + (finalRotation * door[index].positionOffset); // Apply offset based on rotation
//			
//			//Debug.Log($"Placing door {i + 1} at position {finalPosition}, rotation {finalRotation.eulerAngles}");
//			Instantiate(door[index].prefab, finalPosition, finalRotation);
//		}
//	
//		for (int i = 0; i < numWindows; i++)
//		{
//			if (i >= windowPositions.Count)
//			{
//				Debug.LogWarning("Not enough window positions defined.");
//				break;
//			}
//			int index = window.Count > 0 ? random.Next(window.Count) : 0;
//			Vector3 position = windowPositions[i];
//			Quaternion rotation = DetermineRotation(position);
//			Quaternion finalRotation = rotation * Quaternion.Euler(window[index].rotationOffset);
//			Vector3 finalPosition = position + (finalRotation * window[index].positionOffset); // Apply offset based on rotation
//			Instantiate(window[index].prefab, finalPosition, finalRotation);
//		}
//	}
//	
//	private void PlaceWallDecorations(System.Random decorationRandom)
//	{
//		if (wallDecorations == null || wallDecorations.Count == 0)
//		{
//			Debug.Log("No wall decorations to place");
//			return;
//		}
//	
//		float yOffset = 1f; // Define the Y offset for the wall decoration
//		int skipInterval = 3; // Define how many positions to skip before placing a decoration
//		int counter = 0; // Counter to keep track of wall positions processed
//	
//		// Set the seed for the decorationRandom instance to ensure the same decorations are chosen for each run
//		decorationRandom = new System.Random(decorationSeed);
//	
//		// Iterate through all wall positions
//		foreach (var wallPosition in wallPositions)
//		{
//			counter++; // Increment the counter for each wall position
//	
//			// Skip the position if it's in the interval
//			if (counter % skipInterval != 0)
//			{
//				continue;
//			}
//	
//			// Check if the position is near a door or window
//			if (doorTiles.Contains(wallPosition) || windowTiles.Contains(wallPosition))
//			{
//				continue;
//			}
//	
//			// Check if the position is within the safe area
//			if (wallPositions.Where(pos => pos != wallPosition).Any(pos => Vector3.Distance(pos, wallPosition) < areaProperties.decorationSafeArea))
//			{
//				continue;
//			}
//	
//			// Select a random decoration using the decorationRandom instance
//			int index = decorationRandom.Next(wallDecorations.Count);
//			//Debug.Log("Selected wall decoration index: " + index); // Debug statement to print selected index
//			Decoration decoration = wallDecorations[index];
//	
//			// Determine the position and rotation
//			Vector3 finalPosition = wallPosition; // Start with the original wall position
//			finalPosition.y += areaProperties.wallDecorationOffset; // Apply the Y offset from the area properties
//			Quaternion rotation = DetermineRotation(wallPosition);
//	
//			// Instantiate the decoration
//			Instantiate(decoration.prefab, finalPosition, rotation);
//		}
//	}
//	
//	private void PlaceFloorDecorations(System.Random decorationRandom)
//	{
//		if (floorDecorations == null || floorDecorations.Count == 0)
//		{
//			//Debug.Log("No floor decorations to place");
//			return;
//		}
//	
//		float reducedDecorationProbability = areaProperties.floorDecorationProbability * 0.1f;
//	
//		// Set the seed for the decorationRandom instance to ensure the same decorations are chosen for each run
//		decorationRandom = new System.Random(decorationSeed);
//	
//		foreach (var floorPosition in floorPositions)
//		{
//			// Check if the position is within the safe area
//			if (wallPositions.Where(pos => pos != floorPosition).Any(pos => Vector3.Distance(pos, floorPosition) < areaProperties.decorationSafeArea))
//			{
//				continue;
//			}
//	
//			// Find the corresponding floor tile at this position
//			TileType floorTile = GetFloorTileAtPosition(floorPosition);
//	
//			// Check if the floor tile allows decoration
//			if (floorTile != null && floorTile.allowDecoration)
//			{
//				// Determine if the floor position is near the wall based on the tileSize
//				bool isNearWall = floorPosition.x <= tileSize || floorPosition.x >= (gridX - 1) * tileSize ||
//								floorPosition.z <= tileSize || floorPosition.z >= (gridZ - 1) * tileSize;
//	
//				// Calculate a random value between 0 and 1 (inclusive)
//				float randomValue = (float)decorationRandom.NextDouble();
//	
//				// Define the final floorDecorationProbability
//				float finalDecorationProbability = isNearWall ? reducedDecorationProbability : areaProperties.floorDecorationProbability;
//	
//				// Randomly decide whether to skip the decoration for this floor position
//				if (randomValue > finalDecorationProbability)
//				{
//					continue;
//				}
//	
//				// Select a random decoration using the decorationRandom instance
//				int index = decorationRandom.Next(floorDecorations.Count);
//				//Debug.Log("Selected floor decoration index: " + index);
//				Decoration decoration = floorDecorations[index];
//	
//				// Determine the position and rotation
//				Vector3 finalPosition = floorPosition;
//				finalPosition.y += areaProperties.floorDecorationOffset; // Apply the Y offset
//	
//				// Apply the rotation offset correctly
//				Quaternion rotation = DetermineRotation(floorPosition,false);
//				Quaternion finalRotation = rotation * Quaternion.Euler(decoration.prefab.transform.rotation.eulerAngles);
//	
//				// Instantiate the decoration
//				Instantiate(decoration.prefab, finalPosition, finalRotation);
//				
//				// Add the floor decoration position to the HashSet
//				floorDecorationPositions.Add(finalPosition);
//			}
//		}
//	}
//
//	private TileType GetFloorTileAtPosition(Vector3 position)
//	{
//		int x = (int)(position.x / tileSize);
//		int z = (int)(position.z / tileSize);
//	
//		// Check if the position is within the grid bounds
//		if (x >= 0 && x < gridX && z >= 0 && z < gridZ)
//		{
//			// Return the corresponding floor tile type at this position
//			return grid[x, 0, z];
//		}
//	
//		return null;
//	}
//	
//	private Quaternion DetermineRotation(Vector3 position, bool isWallDecoration = true)
//	{
//		int x = (int)(position.x / tileSize);
//		int z = (int)(position.z / tileSize);
//	
//		if (isWallDecoration)
//		{
//			if (x == 0)
//				return Quaternion.Euler(0, 90, 0);
//			else if (x == gridX - 1)
//				return Quaternion.Euler(0, -90, 0);
//			else if (z == 0)
//				return Quaternion.Euler(0, 0, 0);
//			else if (z == gridZ - 1)
//				return Quaternion.Euler(0, 180, 0);
//		}
//		else
//		{
//			// For floor decorations, rotate in increments of 90 degrees
//			int randomSteps = UnityEngine.Random.Range(0, 4); // 0, 1, 2, or 3 steps
//			return Quaternion.Euler(0, randomSteps * 90, 0);
//		}
//	
//		return Quaternion.identity; // Interior position, should not occur for doors and windows
//	}
//	
//	// Method to find all possible enemy positions on the floor tiles
//    private List<Vector3> GetPossibleEnemyPositions()
//	{
//		List<Vector3> possiblePositions = new List<Vector3>();
//		float checkRadius = 0.5f; // Radius to check for existing objects
//		LayerMask layerMask = ~0; // Modify this to include only the layers you want to check
//	
//		for (int x = 0; x < gridX; x++)
//		{
//			for (int z = 0; z < gridZ; z++)
//			{
//				Vector3 position = new Vector3(x * tileSize, 0, z * tileSize); // Modify the Y value as needed
//	
//				// Check if the position is empty (floor tile)
//				if (grid[x, 0, z] != null)
//				{
//					// Check for colliders at this position
//					Collider[] colliders = Physics.OverlapSphere(position, checkRadius, layerMask);
//	
//					// Only add the position if no colliders were found
//					if (colliders.Length == 0)
//					{
//						possiblePositions.Add(position);
//					}
//				}
//			}
//		}
//	
//		return possiblePositions;
//	}
//	
//	private List<Vector3> GetFilteredEnemyPositions(List<Vector3> possiblePositions, HashSet<Vector3> floorDecorationPositions)
//	{
//		int buffer = 1; // You can adjust this to change how close to the edge enemies can spawn
//		List<Vector3> filteredPositions = new List<Vector3>();
//	
//		foreach (Vector3 position in possiblePositions)
//		{
//			int x = (int)(position.x / tileSize);
//			int z = (int)(position.z / tileSize);
//	
//			// Exclude positions near the edges of the grid
//			if (x <= buffer || x >= gridX - buffer ||
//				z <= buffer || z >= gridZ - buffer)
//			{
//				continue;
//			}
//	
//			// Exclude positions that have floor decorations
//			if (floorDecorationPositions.Contains(position))
//			{
//				continue;
//			}
//	
//			filteredPositions.Add(position);
//		}
//	
//		return filteredPositions;
//	}
//	
//	private List<Vector3> GetPossiblePlayerPositions()
//	{
//		List<Vector3> possiblePositions = new List<Vector3>();
//		float checkRadius = 0.5f; // Radius to check for existing objects
//		LayerMask layerMask = ~0; // Modify this to include only the layers you want to check
//	
//		// Loop through the outer grid (you can adjust the buffer size)
//		int buffer = 2;
//		for (int x = buffer; x < gridX - buffer; x++)
//		{
//			for (int z = buffer; z < gridZ - buffer; z++)
//			{
//				// Only consider positions on the outer grid
//				if (x > buffer && x < gridX - buffer - 1 && z > buffer && z < gridZ - buffer - 1) continue;
//	
//				Vector3 position = new Vector3(x * tileSize, 0, z * tileSize); // Modify the Y value as needed
//	
//				// Check if the position is empty (floor tile)
//				if (grid[x, 0, z] != null)
//				{
//					// Check for colliders at this position
//					Collider[] colliders = Physics.OverlapSphere(position, checkRadius, layerMask);
//	
//					// Only add the position if no colliders were found
//					if (colliders.Length == 0)
//					{
//						possiblePositions.Add(position);
//					}
//				}
//			}
//		}
//	
//		return possiblePositions;
//	}
//	
//	
//	private List<Vector3> GetFilteredPlayerPositions(List<Vector3> possiblePositions)
//	{
//		List<Vector3> filteredPositions = new List<Vector3>();
//	
//		// Calculate the number of positions to keep
//		int positionsToKeep = (int)(possiblePositions.Count * 0.3);
//	
//		// Add the first N positions to the filtered list
//		for (int i = 0; i < positionsToKeep; i++)
//		{
//			filteredPositions.Add(possiblePositions[i]);
//			//Debug.Log("Filtered position added: " + possiblePositions[i]); // Log the coordinates of the filtered position
//		}
//		return filteredPositions;
//	}
//	
//	
//	
//	
//	
//	
//	
//	
//}
//
//