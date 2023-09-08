using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelGenerator : MonoBehaviour
{
	public static LevelGenerator Instance;
	public CameraManager cameraManager;
	
    private TileType[,,] grid;
    private List<Vector3> doorPositions;
    private List<Vector3> windowPositions;
    private HashSet<Vector3> doorTiles;
	private HashSet<Vector3> windowTiles;
    private List<Vector3> wallPositions;
	private List<Vector3> floorPositions;

    [System.Serializable]
    public class TileType
    {
        [Range(1, 10)]
        public int numToUse;
        public GameObject prefab;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;
        public bool allowDecoration;
		[Range(0, 1)]
		public float tileProbability; 
		
    }

    [System.Serializable]
    public class Decoration
    {
        public GameObject prefab;
		public Vector3 decoPositionOffset;
        public Vector3 decoRotationOffset;
		[Range(0, 1)]
		public float DecoProbability; 
    }

    [System.Serializable]
    public class AreaProperties
    {
        [Range(0, 10)]
        public int numWindows;
        [Range(0, 10)]
        public int numDoors;
		[Range(0, 1)]
		public float floorDecorationProbability; 
        [Range(0, 10)]
        public float decorationSafeArea;
        [Range(1, 10)]
        public float pointSpacing;

    }
	
	[System.Serializable]
    public class SpawnPoints
    {	
		public EnemyManager enemyManager;
        public bool allowEnemies;
		public PlayerManager playerManager;
        public bool allowPlayer;
    }

    [Header("Room Size")]
    [Range(1, 100)]
    public int gridX;
    [Range(1, 100)]
    public int gridY;
    [Range(1, 100)]
    public int gridZ;
    public int tileSize;

    [Header("Tiles")]
    public List<TileType> floor;
    public List<TileType> wall;
    public List<TileType> door;
    public List<TileType> window;

    [Header("Decorations")]
    public List<Decoration> wallDecorations;
    public List<Decoration> floorDecorations;

    [Header("Area Properties & spawn")]
    public AreaProperties areaProperties;
	public SpawnPoints spawnProperties;

    [Header("Seeds")]
    [Range(0, 1000)]
    public int roomSeed;
    [Range(0, 1000)]
    public int decorationSeed;
	
	private HashSet<Vector3> floorDecorationPositions = new HashSet<Vector3>();
	
	private bool isInitialized = false;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
			DontDestroyOnLoad(gameObject);
			Debug.Log("LevelGenerator instantiated.");
		}
		else
		{
			Debug.Log("Destroying extra LevelGenerator instance");
			Debug.Log("Attempted to instantiate another LevelGenerator. Destroying the game object.");
			Destroy(gameObject);
		}
	}
	
	// Initializes the level by generating tiles, decorations, and spawning entities.
	public void InitializeLevel()
	{
		roomSeed = Random.Range(0, 1000);
		decorationSeed = Random.Range(0, 1000);
	
		Debug.Log("LevelGenerator started");
		wallPositions = new List<Vector3>();
		System.Random random = new System.Random(roomSeed);
		System.Random decorationRandom = new System.Random(decorationSeed);
		grid = new TileType[gridX, gridY, gridZ];
	
		InitializeDoorAndWindowPositions(random);
		PlaceFloor(random);
		PlaceWalls(random);
		PlaceDoorsAndWindows(doorPositions, windowPositions, areaProperties.numDoors, areaProperties.numWindows, random);
		PlaceWallDecorations(random); 
		PlaceFloorDecorations(random);
		
		// Get the possible enemy positions
		List<Vector3> possibleEnemyPositions = GetPossibleEnemyPositions();
		List<Vector3> filteredEnemyPositions = GetFilteredEnemyPositions(possibleEnemyPositions, floorDecorationPositions);
		
		// If enemies are allowed to spawn, call the spawn method
		if (spawnProperties.allowEnemies && spawnProperties.enemyManager != null)
		{
			int newNumNormalEnemies = GameManager.Instance.NumNormalEnemies;
			int newNumSecretEnemies = GameManager.Instance.NumSecretEnemies;
			int newNumBossEnemies = GameManager.Instance.NumBossEnemies;
	
			spawnProperties.enemyManager.SpawnEnemiesAtPositions(filteredEnemyPositions, newNumNormalEnemies, newNumSecretEnemies, newNumBossEnemies);
		}
		else
		{
			if (spawnProperties.allowEnemies && spawnProperties.enemyManager == null)
			{
				//Debug.LogWarning("Enemy spawning is enabled, but the EnemyManager reference is not assigned!");
			}
		}
		
		// If player is allowed to spawn, call the spawn method
		if (spawnProperties.allowPlayer && spawnProperties.playerManager != null)
		{
			List<Vector3> possiblePlayerPositions = GetPossiblePlayerPositions();
			List<Vector3> filteredPlayerPositions = GetFilteredPlayerPositions(possiblePlayerPositions);
			spawnProperties.playerManager.SpawnPlayerAtPosition(filteredPlayerPositions); // Changed here
			// Initialize Camera
			if (cameraManager != null)
			{
				cameraManager.InitializeCamera();
			}
		}
		else
		{
			if (spawnProperties.allowPlayer && spawnProperties.playerManager == null) // Changed here
			{
				//Debug.LogWarning("Player spawning is enabled, but the PlayerManager reference is not assigned!");
			}
		}
		
		if (!isInitialized)
		{
			isInitialized = true;
		}
	}
	
	// Disables the initialized flag when the object is disabled.
	private void OnDisable()
	{
		isInitialized = false;
	}
	
	// Initializes positions for doors and windows based on the grid.
	private void InitializeDoorAndWindowPositions(System.Random random)
	{
		doorPositions = new List<Vector3>();
		windowPositions = new List<Vector3>();
		doorTiles = new HashSet<Vector3>();
		windowTiles = new HashSet<Vector3>();
	
		List<Vector3> validDoorAndWindowPositions = new List<Vector3>();
		for (int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				if ((x == 0 || x == gridX - 1 || z == 0 || z == gridZ - 1) // Exclude interior positions
					&& !(x == 0 && z == 0)
					&& !(x == gridX - 1 && z == 0)
					&& !(x == 0 && z == gridZ - 1)
					&& !(x == gridX - 1 && z == gridZ - 1)) // Exclude exact corners
				{
					validDoorAndWindowPositions.Add(new Vector3(x * tileSize, 0, z * tileSize));
				}
			}
		}
		
		for (int i = 0; i < areaProperties.numDoors; i++)
		{
			if (validDoorAndWindowPositions.Count == 0) break; // No more valid positions
			int randomIndex = random.Next(validDoorAndWindowPositions.Count);
			Vector3 doorPosition = validDoorAndWindowPositions[randomIndex];
			doorPositions.Add(doorPosition);
			doorTiles.Add(doorPosition); // Ensure this position is skipped when placing walls
			validDoorAndWindowPositions.RemoveAt(randomIndex); // Remove chosen position from valid positions
		}
	
		for (int i = 0; i < areaProperties.numWindows; i++)
		{
			if (validDoorAndWindowPositions.Count == 0) break; // No more valid positions
			int randomIndex = random.Next(validDoorAndWindowPositions.Count);
			Vector3 windowPosition = validDoorAndWindowPositions[randomIndex];
			windowPositions.Add(windowPosition);
			windowTiles.Add(windowPosition); // Ensure this position is skipped when placing walls
			validDoorAndWindowPositions.RemoveAt(randomIndex); // Remove chosen position from valid positions
		}
	}
	
	// Places floor tiles on the grid based on weighted probabilities.
	private void PlaceFloor(System.Random random)
	{
		Vector3 position;
		floorPositions = new List<Vector3>();
	
		// Pre-compute the cumulative probabilities for weighted random selection
		List<float> cumulativeProbabilities = new List<float>();
		float cumulative = 0;
		foreach (var tile in floor)
		{
			cumulative += tile.tileProbability;
			cumulativeProbabilities.Add(cumulative);
		}
	
		for (int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				int index = GetRandomTileIndex(random, cumulativeProbabilities);
				position = new Vector3(x * tileSize, 0, z * tileSize);
				Instantiate(floor[index].prefab, position + floor[index].positionOffset, Quaternion.Euler(floor[index].rotationOffset));
	
				// Update the grid with the correct TileType for each floor position
				grid[x, 0, z] = floor[index];
	
				// Add the floor position to the floorPositions list for floor decorations
				floorPositions.Add(position);
			}
		}
	}
	
	// Helper method to get a random tile index based on weighted probabilities.
	private int GetRandomTileIndex(System.Random random, List<float> cumulativeProbabilities)
	{
		float randomValue = (float)random.NextDouble() * cumulativeProbabilities.Last();
		for (int i = 0; i < cumulativeProbabilities.Count; i++)
		{
			if (randomValue <= cumulativeProbabilities[i])
				return i;
		}
		return cumulativeProbabilities.Count - 1; // Shouldn't be reached unless there's a rounding error
	}
	
	// Places wall tiles on the grid, avoiding door and window positions.
    private void PlaceWalls(System.Random random)
	{
		// Check if there are any walls in the collection
		if (wall == null || wall.Count == 0)
		{
			// No walls to place, so just return.
			return;
		}
	
		// Pre-compute the cumulative probabilities for weighted random selection for walls
		List<float> wallCumulativeProbabilities = new List<float>();
		float wallCumulative = 0;
		foreach (var tile in wall)
		{
			wallCumulative += tile.tileProbability;
			wallCumulativeProbabilities.Add(wallCumulative);
		}
	
		for (int y = 0; y < gridY; y++)
		{
			for (int x = 0; x < gridX; x++)
			{
				for (int z = 0; z < gridZ; z++)
				{
					Vector3 position = new Vector3(x * tileSize, y * tileSize, z * tileSize);
	
					if (doorTiles.Contains(position) || windowTiles.Contains(position)) continue;
	
					bool isCorner = (x == 0 && z == 0) || (x == gridX - 1 && z == 0) || (x == 0 && z == gridZ - 1) || (x == gridX - 1 && z == gridZ - 1);
					bool isEdge = x == 0 || x == gridX - 1 || z == 0 || z == gridZ - 1;
					int index = GetRandomTileIndex(random, wallCumulativeProbabilities);  // Use probability-based index
	
	
					if (isCorner)
					{
						List<Quaternion> rotations = new List<Quaternion>();
						if (x == 0 && z == 0) { rotations.Add(Quaternion.Euler(0, 90, 0)); rotations.Add(Quaternion.Euler(0, 0, 0)); }
						if (x == gridX - 1 && z == 0) { rotations.Add(Quaternion.Euler(0, 0, 0)); rotations.Add(Quaternion.Euler(0, -90, 0)); }
						if (x == 0 && z == gridZ - 1) { rotations.Add(Quaternion.Euler(0, 90, 0)); rotations.Add(Quaternion.Euler(0, 180, 0)); }
						if (x == gridX - 1 && z == gridZ - 1) { rotations.Add(Quaternion.Euler(0, 270, 0)); rotations.Add(Quaternion.Euler(0, 180, 0)); }
	
						foreach (var rot in rotations)
						{
							Quaternion finalRotation = rot * Quaternion.Euler(wall[index].rotationOffset);
							Vector3 finalPosition = position + (finalRotation * wall[index].positionOffset); // Apply offset based on rotation
							Instantiate(wall[index].prefab, finalPosition, finalRotation);
						}
					}
					else if (isEdge)
					{
						Quaternion rotation = Quaternion.identity;
						if (x == 0) rotation = Quaternion.Euler(0, 90, 0);
						else if (x == gridX - 1) rotation = Quaternion.Euler(0, -90, 0);
						else if (z == 0) rotation = Quaternion.Euler(0, 0, 0);
						else if (z == gridZ - 1) rotation = Quaternion.Euler(0, 180, 0);
	
						rotation = rotation * Quaternion.Euler(wall[index].rotationOffset); // Combine rotations
						Vector3 finalPosition = position + (rotation * wall[index].positionOffset); // Apply offset based on rotation
						Instantiate(wall[index].prefab, finalPosition, rotation);
	
						if (wall[index].allowDecoration)
						{
							wallPositions.Add(finalPosition); // Add final position to the wallPositions list
						}
					}
				}
			}
		}
	}
	
	// Places doors and windows at pre-determined positions.
	private void PlaceDoorsAndWindows(List<Vector3> doorPositions, List<Vector3> windowPositions, int numDoors, int numWindows, System.Random random)
	{
		// Pre-compute the cumulative probabilities for doors
		List<float> doorCumulativeProbabilities = new List<float>();
		float doorCumulative = 0;
		foreach (var tile in door)
		{
			doorCumulative += tile.tileProbability;
			doorCumulativeProbabilities.Add(doorCumulative);
		}
	
		// Pre-compute the cumulative probabilities for windows
		List<float> windowCumulativeProbabilities = new List<float>();
		float windowCumulative = 0;
		foreach (var tile in window)
		{
			windowCumulative += tile.tileProbability;
			windowCumulativeProbabilities.Add(windowCumulative);
		}
	
		// Assuming doorPositions and windowPositions are already populated based on your game's logic.
		for (int i = 0; i < numDoors; i++)
		{
			if (doorPositions.Count > 0)
			{
				Vector3 doorPosition = doorPositions[random.Next(doorPositions.Count)];
				int doorIndex = GetRandomTileIndex(random, doorCumulativeProbabilities);
				
				// Use DetermineRotation for calculating base door rotation.
				Quaternion baseRotation = DetermineRotation(doorPosition, true);
				Quaternion offsetRotation = Quaternion.Euler(door[doorIndex].rotationOffset);
				Quaternion finalDoorRotation = baseRotation * offsetRotation;
	
				Vector3 finalDoorPosition = doorPosition + door[doorIndex].positionOffset;
				Instantiate(door[doorIndex].prefab, finalDoorPosition, finalDoorRotation);
	
				doorTiles.Add(doorPosition);
				doorPositions.Remove(doorPosition);
			}
		}
	
		for (int i = 0; i < numWindows; i++)
		{
			if (windowPositions.Count > 0)
			{
				Vector3 windowPosition = windowPositions[random.Next(windowPositions.Count)];
				int windowIndex = GetRandomTileIndex(random, windowCumulativeProbabilities);
	
				// Use DetermineRotation for calculating base window rotation.
				Quaternion baseRotation = DetermineRotation(windowPosition, true);
				Quaternion offsetRotation = Quaternion.Euler(window[windowIndex].rotationOffset);
				Quaternion finalWindowRotation = baseRotation * offsetRotation;
	
				Vector3 finalWindowPosition = windowPosition + window[windowIndex].positionOffset;
				Instantiate(window[windowIndex].prefab, finalWindowPosition, finalWindowRotation);
	
				windowTiles.Add(windowPosition);
				windowPositions.Remove(windowPosition);
			}
		}
	}
	
	
	// Method for the decorations //
	// Places wall decorations at valid positions.
	private void PlaceWallDecorations(System.Random decorationRandom)
	{
		if (wallDecorations == null || wallDecorations.Count == 0)
		{
			Debug.Log("No wall decorations to place");
			return;
		}
	
		float yOffset = 1f; 
		int skipInterval = 3;
		int counter = 0;
	
		// Create cumulative probabilities list for wall decorations
		List<float> wallCumulativeProbabilities = new List<float>();
		float cumulative = 0;
		foreach (var decoration in wallDecorations)
		{
			cumulative += decoration.DecoProbability;
			wallCumulativeProbabilities.Add(cumulative);
		}
	
		foreach (var wallPosition in wallPositions)
		{
			counter++;
	
			if (counter % skipInterval != 0)
			{
				continue;
			}
	
			if (doorTiles.Contains(wallPosition) || windowTiles.Contains(wallPosition))
			{
				continue;
			}
	
			if (wallPositions.Any(pos => pos != wallPosition && Vector3.Distance(pos, wallPosition) < areaProperties.decorationSafeArea))
			{
				continue;
			}
	
			// Select a decoration based on its probability
			int index = GetRandomTileIndex(decorationRandom, wallCumulativeProbabilities);
			Decoration decoration = wallDecorations[index];
	
			// Adjust position with the decoration's offset values
			Vector3 finalPosition = wallPosition + decoration.decoPositionOffset;
			finalPosition.y += yOffset; 
	
			// Calculate the rotation and apply the decoration's rotation offset
			Quaternion baseRotation = DetermineRotation(wallPosition);
			Quaternion offsetRotation = Quaternion.Euler(decoration.decoRotationOffset);
			Quaternion finalRotation = baseRotation * offsetRotation;
	
			// Instantiate the decoration
			Instantiate(decoration.prefab, finalPosition, finalRotation);
		}
	}
	
	// Places floor decorations at valid positions.
	private void PlaceFloorDecorations(System.Random decorationRandom)
	{
		if (floorDecorations == null || floorDecorations.Count == 0)
		{
			//Debug.Log("No floor decorations to place");
			return;
		}
	
		// Calculate Cumulative Probabilities for floor decorations
		List<float> decorationCumulativeProbabilities = new List<float>();
		float decorationCumulative = 0;
		foreach (var decoration in floorDecorations)
		{
			decorationCumulative += decoration.DecoProbability;
			decorationCumulativeProbabilities.Add(decorationCumulative);
		}
	
		float reducedDecorationProbability = areaProperties.floorDecorationProbability * 0.1f;
	
		foreach (var floorPosition in floorPositions)
		{
			if (wallPositions.Any(pos => pos != floorPosition && Vector3.Distance(pos, floorPosition) < areaProperties.decorationSafeArea))
			{
				continue;
			}
	
			TileType floorTile = GetFloorTileAtPosition(floorPosition);
	
			if (floorTile != null && floorTile.allowDecoration)
			{
				bool isNearWall = floorPosition.x <= tileSize || floorPosition.x >= (gridX - 1) * tileSize ||
								floorPosition.z <= tileSize || floorPosition.z >= (gridZ - 1) * tileSize;
	
				float randomValue = (float)decorationRandom.NextDouble();
	
				float finalDecorationProbability = isNearWall ? reducedDecorationProbability : areaProperties.floorDecorationProbability;
	
				if (randomValue > finalDecorationProbability)
				{
					continue;
				}
	
				// Select Decoration Using Weighted Probability
				int decoIndex = GetRandomTileIndex(decorationRandom, decorationCumulativeProbabilities);
				Decoration decoration = floorDecorations[decoIndex];
	
				// Adjust position with the decoration's offset values
				Vector3 finalPosition = floorPosition + decoration.decoPositionOffset;
	
				// Calculate the rotation, apply the prefab's rotation, and then the decoration's rotation offset
				Quaternion baseRotation = DetermineRotation(floorPosition, false);
				Quaternion prefabRotation = Quaternion.Euler(decoration.prefab.transform.rotation.eulerAngles);
				Quaternion offsetRotation = Quaternion.Euler(decoration.decoRotationOffset);
				Quaternion finalRotation = baseRotation * prefabRotation * offsetRotation;
	
				Instantiate(decoration.prefab, finalPosition, finalRotation);
				floorDecorationPositions.Add(finalPosition);
			}
		}
	}
	
	// Retrieves the TileType of the floor at a given position.
	private TileType GetFloorTileAtPosition(Vector3 position)
	{
		int x = (int)(position.x / tileSize);
		int z = (int)(position.z / tileSize);
	
		// Check if the position is within the grid bounds
		if (x >= 0 && x < gridX && z >= 0 && z < gridZ)
		{
			// Return the corresponding floor tile type at this position
			return grid[x, 0, z];
		}
	
		return null;
	}
	
	// Determines the rotation for a tile or decoration based on its grid position.
	private Quaternion DetermineRotation(Vector3 position, bool isWallDecoration = true)
	{
		int x = (int)(position.x / tileSize);
		int z = (int)(position.z / tileSize);
	
		if (isWallDecoration)
		{
			if (x == 0)
				return Quaternion.Euler(0, 90, 0);
			else if (x == gridX - 1)
				return Quaternion.Euler(0, -90, 0);
			else if (z == 0)
				return Quaternion.Euler(0, 0, 0);
			else if (z == gridZ - 1)
				return Quaternion.Euler(0, 180, 0);
		}
		else
		{
			// For floor decorations, rotate in increments of 90 degrees
			int randomSteps = UnityEngine.Random.Range(0, 4); // 0, 1, 2, or 3 steps
			return Quaternion.Euler(0, randomSteps * 90, 0);
		}
	
		return Quaternion.identity; // Interior position, should not occur for doors and windows
	}
	
	
	// Methods for the enemy //
	// Retrieves a list of possible positions for enemy spawning.
    private List<Vector3> GetPossibleEnemyPositions()
	{
		List<Vector3> possiblePositions = new List<Vector3>();
		float checkRadius = 0.5f; // Radius to check for existing objects
		LayerMask layerMask = ~0; // Modify this to include only the layers you want to check
	
		for (int x = 0; x < gridX; x++)
		{
			for (int z = 0; z < gridZ; z++)
			{
				Vector3 position = new Vector3(x * tileSize, 0, z * tileSize); // Modify the Y value as needed
	
				// Check if the position is empty (floor tile)
				if (grid[x, 0, z] != null)
				{
					// Check for colliders at this position
					Collider[] colliders = Physics.OverlapSphere(position, checkRadius, layerMask);
	
					// Only add the position if no colliders were found
					if (colliders.Length == 0)
					{
						possiblePositions.Add(position);
					}
				}
			}
		}
	
		return possiblePositions;
	}
	
	// Retrieves a list of filtered positions for the enemy
	private List<Vector3> GetFilteredEnemyPositions(List<Vector3> possiblePositions, HashSet<Vector3> floorDecorationPositions)
	{
		int buffer = 1; // You can adjust this to change how close to the edge enemies can spawn
		List<Vector3> filteredPositions = new List<Vector3>();
	
		foreach (Vector3 position in possiblePositions)
		{
			int x = (int)(position.x / tileSize);
			int z = (int)(position.z / tileSize);
	
			// Exclude positions near the edges of the grid
			if (x <= buffer || x >= gridX - buffer ||
				z <= buffer || z >= gridZ - buffer)
			{
				continue;
			}
	
			// Exclude positions that have floor decorations
			if (floorDecorationPositions.Contains(position))
			{
				continue;
			}
	
			filteredPositions.Add(position);
		}
	
		return filteredPositions;
	}
	
	
	// Methods for the Player //
	// Retrieves a list of possible positions for player spawning.
	private List<Vector3> GetPossiblePlayerPositions()
	{
		List<Vector3> possiblePositions = new List<Vector3>();
		float checkRadius = 0.5f; // Radius to check for existing objects
		LayerMask layerMask = ~0; // Modify this to include only the layers you want to check
	
		// Loop through the outer grid (you can adjust the buffer size)
		int buffer = 2;
		for (int x = buffer; x < gridX - buffer; x++)
		{
			for (int z = buffer; z < gridZ - buffer; z++)
			{
				// Only consider positions on the outer grid
				if (x > buffer && x < gridX - buffer - 1 && z > buffer && z < gridZ - buffer - 1) continue;
	
				Vector3 position = new Vector3(x * tileSize, 0, z * tileSize); // Modify the Y value as needed
	
				// Check if the position is empty (floor tile)
				if (grid[x, 0, z] != null)
				{
					// Check for colliders at this position
					Collider[] colliders = Physics.OverlapSphere(position, checkRadius, layerMask);
	
					// Only add the position if no colliders were found
					if (colliders.Length == 0)
					{
						possiblePositions.Add(position);
					}
				}
			}
		}
	
		return possiblePositions;
	}
	
	// Retrieves a list of filtered positions for the player
	private List<Vector3> GetFilteredPlayerPositions(List<Vector3> possiblePositions)
	{
		List<Vector3> filteredPositions = new List<Vector3>();
	
		// Calculate the number of positions to keep
		int positionsToKeep = (int)(possiblePositions.Count * 0.3);
	
		// Add the first N positions to the filtered list
		for (int i = 0; i < positionsToKeep; i++)
		{
			filteredPositions.Add(possiblePositions[i]);
			//Debug.Log("Filtered position added: " + possiblePositions[i]); // Log the coordinates of the filtered position
		}
		return filteredPositions;
	}
	
	
	
	
	
	
	
	
}

