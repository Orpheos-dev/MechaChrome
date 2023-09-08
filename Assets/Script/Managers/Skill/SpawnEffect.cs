using System.Collections.Generic;
using UnityEngine;

public class SpawnEffect : MonoBehaviour
{
	public static SpawnEffect Instance;
	
    public GameObject missileBasePrefab; 
    public Transform playerTransform;
	public List<GameObject> missileBases = new List<GameObject>();
	
    public float followSpeed;
    public float safeDistance; // Safe distance from player
	public float yOffset; 

    public bool isActive = false;

	private void Awake()
	{
		if (Instance == null)
		{
			Instance = this;
		}
	}
	
	private void Update()
	{
		// Check and update playerTransform only if it's null and the game is active.
		if (isActive)
		{
			if (playerTransform == null)
			{
				UpdatePlayerTransformReference();
			}
		}
		if (isActive && playerTransform != null)
		{
			for (int i = 0; i < missileBases.Count; i++)
			{
				if (missileBases[i] != null)
				{
					float angle = 360f / missileBases.Count;
					float angleInRadians = angle * Mathf.Deg2Rad * i;
					Vector3 offset = new Vector3(safeDistance * Mathf.Cos(angleInRadians), 0, safeDistance * Mathf.Sin(angleInRadians));
					Vector3 targetPosition = playerTransform.position + offset + new Vector3(0, yOffset, 0);
					float step = followSpeed * Time.deltaTime;
					missileBases[i].transform.position = Vector3.MoveTowards(missileBases[i].transform.position, targetPosition, step);
				}
			}
		}
	}

	// Spawn a missile base around the player.
    public void Spawn()
	{
		if (missileBasePrefab == null)
		{
			return;
		}
		
		UpdatePlayerTransformReference();
	
		if (playerTransform == null)
		{
			return;
		}
	
		Vector3 spawnPosition = playerTransform.position + new Vector3(0, yOffset, 0);

		// Calculate position using polar coordinates
		float angle = 360f / (missileBases.Count + 1); 
	
		// Convert the angle to radians
		float angleInRadians = angle * Mathf.Deg2Rad * missileBases.Count;
	
		// calculate the rotated offset relative to the player
		Vector3 offset = new Vector3(safeDistance * Mathf.Cos(angleInRadians), 0, safeDistance * Mathf.Sin(angleInRadians));
		spawnPosition += offset;
	
		GameObject newMissileBase = Instantiate(missileBasePrefab, spawnPosition, Quaternion.identity);
	
		if (newMissileBase != null)
		{
			DontDestroyOnLoad(newMissileBase);  // Add this line to make the missile base persistent
			missileBases.Add(newMissileBase);
		}
		else
		{
			Debug.LogError("Failed to instantiate Missile Base Prefab.");
		}
	}
	
	// Update the reference to the player's transform.
	public void UpdatePlayerTransformReference()
	{
		GameObject playerObj = GameObject.FindGameObjectWithTag("Player");
		
		if (playerObj != null)
		{
			playerTransform = playerObj.transform;
		}
		else
		{
			Debug.LogError("Player GameObject with tag 'Player' not found in the current scene.");
			playerTransform = null;
		}
	}

	// Get a list of spawned missile bases.
	public List<GameObject> GetMissileBases()
	{
		return missileBases;
	}

	// Called when the player dies to deactivate the effect.
	public void OnPlayerDied()
	{
		isActive = false;
		playerTransform = null;
	}
}
