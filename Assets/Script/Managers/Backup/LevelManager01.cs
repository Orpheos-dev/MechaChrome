//using System.Collections.Generic;
//using System.Linq;
//using UnityEngine;
//
//public class LevelGenerator : MonoBehaviour
//{
//    private TileType[,,] grid;
//    private List<Vector3> doorPositions;
//    private List<Vector3> windowPositions;
//    private HashSet<Vector3> doorTiles;
//	private HashSet<Vector3> windowTiles;
//    private List<Vector3> wallPositions;
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
//        [Range(0, 10)]
//        public float decorationSafeArea;
//        [Range(1, 10)]
//        public float pointSpacing;
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
//    [Header("Area Properties")]
//    public AreaProperties areaProperties;
//
//    [Header("Seeds")]
//    [Range(0, 10)]
//    public int roomSeed;
//    [Range(0, 10)]
//    public int decorationSeed;
//
//    private void Start()
//    {
//        Debug.Log("Grid size (X, Y, Z): (" + gridX + ", " + gridY + ", " + gridZ + ")");
//        wallPositions = new List<Vector3>();
//        System.Random random = new System.Random(roomSeed);
//        grid = new TileType[gridX, gridY, gridZ];
//
//        InitializeDoorAndWindowPositions(random);
//        PlaceFloor(random);
//        PlaceWalls(random);
//        PlaceDoorsAndWindows(doorPositions, windowPositions, areaProperties.numDoors, areaProperties.numWindows, random);
//    }
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
//    {
//        Vector3 position;
//        for (int x = 0; x < gridX; x++)
//        {
//            for (int z = 0; z < gridZ; z++)
//            {
//                int index = random.Next(floor.Count);
//                position = new Vector3(x * tileSize, 0, z * tileSize);
//                Instantiate(floor[index].prefab, position + floor[index].positionOffset, Quaternion.Euler(floor[index].rotationOffset));
//            }
//        }
//    }
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
//						//Debug.Log($"Placing corner wall at position: {position}, x: {x}, z: {z}");
//						List<Quaternion> rotations = new List<Quaternion>();
//						if (x == 0 && z == 0) { rotations.Add(Quaternion.Euler(0, 90, 0)); rotations.Add(Quaternion.Euler(0, 0, 0)); } // Top Left
//						if (x == gridX - 1 && z == 0) { rotations.Add(Quaternion.Euler(0, 0, 0)); rotations.Add(Quaternion.Euler(0, -90, 0)); } // Top Right
//						if (x == 0 && z == gridZ - 1) { rotations.Add(Quaternion.Euler(0, 90, 0)); rotations.Add(Quaternion.Euler(0, 180, 0)); } // Bottom Left
//						if (x == gridX - 1 && z == gridZ - 1) { rotations.Add(Quaternion.Euler(0, 270, 0)); rotations.Add(Quaternion.Euler(0, 180, 0)); } // Bottom Right
//	
//						//Debug.Log($"Placing corner wall at position: {position}, x: {x}, z: {z}, rotations: {string.Join(", ", rotations.Select(r => r.eulerAngles.ToString()))}");
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
//						Quaternion rotation = Quaternion.identity; // Moved inside loop
//						if (x == 0) rotation = Quaternion.Euler(0, 90, 0);
//						else if (x == gridX - 1) rotation = Quaternion.Euler(0, -90, 0);
//						else if (z == 0) rotation = Quaternion.Euler(0, 0, 0);
//						else if (z == gridZ - 1) rotation = Quaternion.Euler(0, 180, 0);
//	
//						rotation = rotation * Quaternion.Euler(wall[index].rotationOffset); // Combine rotations
//						Vector3 finalPosition = position + (rotation * wall[index].positionOffset); // Apply offset based on rotation
//						Instantiate(wall[index].prefab, finalPosition, rotation);
//					}
//				}
//			}
//		}
//	}
//	
//	
//	
//	
//	
//	
//		
//	private void PlaceDoorsAndWindows(List<Vector3> doorPositions, List<Vector3> windowPositions, int numDoors, int numWindows, System.Random random)
//	{
//		// Debug statements to log the information
//		Debug.Log($"Number of doors to place: {numDoors}");
//		Debug.Log($"Number of door positions available: {doorPositions.Count}");
//		Debug.Log($"Number of door prefabs available: {door.Count}");
//		Debug.Log($"Number of windows to place: {numWindows}");
//		Debug.Log($"Number of window positions available: {windowPositions.Count}");
//		Debug.Log($"Number of window prefabs available: {window.Count}");
//	
//		for (int i = 0; i < numDoors; i++)
//		{
//			if (i >= doorPositions.Count)
//			{
//				Debug.LogWarning("Not enough door positions defined.");
//				break;
//			}
//			int index = door.Count > 0 ? random.Next(door.Count) : 0;
//			Vector3 position = doorPositions[i];
//			Quaternion rotation = DetermineRotation(position);
//			Quaternion finalRotation = rotation * Quaternion.Euler(door[index].rotationOffset);
//			Vector3 finalPosition = position + (finalRotation * door[index].positionOffset); // Apply offset based on rotation
//			
//			Debug.Log($"Placing door {i + 1} at position {finalPosition}, rotation {finalRotation.eulerAngles}");
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
//	
//	private Quaternion DetermineRotation(Vector3 position)
//	{
//		int x = (int)(position.x / tileSize);
//		int z = (int)(position.z / tileSize);
//	
//		if (x == 0)
//			return Quaternion.Euler(0, 90, 0);
//		else if (x == gridX - 1)
//			return Quaternion.Euler(0, -90, 0);
//		else if (z == 0)
//			return Quaternion.Euler(0, 0, 0);
//		else if (z == gridZ - 1)
//			return Quaternion.Euler(0, 180, 0);
//		else
//			return Quaternion.identity; // Interior position, should not occur for doors and windows
//	}
//	
//}
//
//