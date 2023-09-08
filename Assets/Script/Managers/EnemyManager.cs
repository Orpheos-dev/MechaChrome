using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class EnemyManager : MonoBehaviour
{
    public static EnemyManager Instance;

    [System.Serializable]
    public class EnemyData
    {
        [Header("Stats")]
        public GameObject enemyPrefab;
        public GameObject healthBarPrefab;
        public Vector3 positionOffset;
        public int maxHp = 50;
        public float rotationSpeed = 1f; 

        [Header("Projectiles")]
        public GameObject projectilePrefab;
        public float projectileTrajectory = 1f;
        public float projectileSpeed = 1f;
        public float projectileFireRate = 1f;
        public Vector3 projectileOffset = new Vector3(0f, 0f, 1f);
        public bool isHoming = false;
        public int projectileDamage = 5;
    }
	
	[System.Serializable]
	public class EnemyInfo
	{
		public EnemyData enemyData;
		public float lastFireTime = -Mathf.Infinity;
	}
	
    [Header("Enemy Types")]
    public List<EnemyData> normalEnemies = new List<EnemyData>();
    public List<EnemyData> secretEnemies = new List<EnemyData>();
    public List<EnemyData> bossEnemies = new List<EnemyData>();

    [Header("Enemy properties")]
    public int NumNormalEnemies = 2;
    public int NumSecretEnemies = 0;
    public int NumBossEnemies = 0;

    private List<GameObject> currentEnemies = new List<GameObject>();
    private GameObject player;
    private float lastFireTime = -Mathf.Infinity;
	
	private Coroutine checkForPlayerRoutine; 

	private Dictionary<GameObject, EnemyInfo> enemyToDataMap = new Dictionary<GameObject, EnemyInfo>();
	private int currentHealth;
	private Dictionary<GameObject, int> enemyCurrentHealth = new Dictionary<GameObject, int>();
	
	public int initialNormalEnemies; 
	public int initialSecretEnemies; //Not being used at for now
	public int initialBossEnemies; //Not being used at for now
	
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
			DontDestroyOnLoad(gameObject);
            Debug.Log("EnemyManager instantiated.");
        }
        else
        {
            Debug.Log("Destroying extra EnemyManager instance");
            Destroy(gameObject);
        }
    }
	
	// Initialize enemies
	public void InitializeEnemies(int someParameter)
    {
        Debug.Log("Enemies Initialized");
		player = GameObject.FindGameObjectWithTag("Player");
	    StartCoroutine(CheckForPlayerCoroutine());
    }
	
	// Spawns enemies at given positions based on the type and count
    public void SpawnEnemiesAtPositions(List<Vector3> enemySpawnPositions, int newNumNormalEnemies, int newNumSecretEnemies, int newNumBossEnemies)
	{
		NumNormalEnemies = newNumNormalEnemies;
		NumSecretEnemies = newNumSecretEnemies;
		NumBossEnemies = newNumBossEnemies;
	
		SpawnEnemiesOfType(normalEnemies, NumNormalEnemies, new List<Vector3>(enemySpawnPositions));
		SpawnEnemiesOfType(secretEnemies, NumSecretEnemies, new List<Vector3>(enemySpawnPositions));
		SpawnEnemiesOfType(bossEnemies, NumBossEnemies, new List<Vector3>(enemySpawnPositions));
	}
	
	// Helper function to spawn enemies of a specific type
    private void SpawnEnemiesOfType(List<EnemyData> enemyList, int numToSpawn, List<Vector3> spawnPositions)
	{
		if (numToSpawn > spawnPositions.Count)
		{
			Debug.LogWarning("Trying to spawn more enemies than there are spawn positions!");
			numToSpawn = spawnPositions.Count;
		}
	
		for (int i = 0; i < numToSpawn; i++)
		{
			EnemyData enemyData = enemyList[Random.Range(0, enemyList.Count)];
			int randomIndex = Random.Range(0, spawnPositions.Count);
			Vector3 spawnPosition = spawnPositions[randomIndex];
			spawnPositions.RemoveAt(randomIndex);
			spawnPosition += enemyData.positionOffset;
			
			GameObject enemy = Instantiate(enemyData.enemyPrefab, spawnPosition, Quaternion.identity);
			currentEnemies.Add(enemy);
			EnemyInfo enemyInfo = new EnemyInfo { enemyData = enemyData };
			enemyToDataMap[enemy] = enemyInfo;
			enemyCurrentHealth[enemy] = enemyData.maxHp;
		}
	}

	// Coroutine that periodically checks for player and fires projectiles
    private IEnumerator CheckForPlayerCoroutine()
	{
		while (true)
		{
			if (player == null)
			{
				yield break;  // If the player is null, we exit the coroutine.
			}
	
			foreach (GameObject enemy in currentEnemies)
			{
				// It's also good practice to check if the enemy is still valid.
				if(enemy != null)
				{
					EnemyInfo enemyInfo = enemyToDataMap[enemy];
					Vector3 fireStartPosition = enemy.transform.position + enemy.transform.forward + enemyInfo.enemyData.projectileOffset;
					Vector3 fireDirection = (player.transform.position - enemy.transform.position).normalized;
					FireProjectile(fireStartPosition, fireDirection, enemyInfo);
				}
			}
			yield return new WaitForSeconds(1.0f); // Assuming each enemy has the same fire rate, adjust as needed.
		}
	}
	
	// Function to stop the CheckForPlayer coroutine
	public void StopCheckForPlayer()
	{
		if (checkForPlayerRoutine != null)
		{
			StopCoroutine(checkForPlayerRoutine);
			checkForPlayerRoutine = null;  // Null it out after stopping to ensure it's not reused
		}
	}
	
	// Fires a projectile from a given position in a given direction
    public void FireProjectile(Vector3 startPosition, Vector3 direction, EnemyInfo enemyInfo)
	{
		if (Time.time - enemyInfo.lastFireTime >= enemyInfo.enemyData.projectileFireRate)
		{
			GameObject proj = Instantiate(enemyInfo.enemyData.projectilePrefab, startPosition, Quaternion.identity);
			if (!proj)
			{
				Debug.LogError("Failed to instantiate projectile.");
				return;
			}
	
			Rigidbody rb = proj.GetComponent<Rigidbody>();
			if (!rb)
			{
				Debug.LogError("Projectile does not have a Rigidbody.");
				return;
			}
	
			rb.useGravity = false; // Modify the Rigidbody properties.
	
			// Ensure the projectile's Collider is set as a trigger.
			Collider collider = proj.GetComponent<Collider>();
			if (collider)
			{
				collider.isTrigger = true;
			}
			else
			{
				Debug.LogError("Projectile does not have a Collider.");
				return;
			}
	
			// Add the OnTriggerEnter event to the projectile.
			ProjectileCollisionDelegator collisionDelegator = proj.GetComponent<ProjectileCollisionDelegator>();
			if (!collisionDelegator)
			{
				collisionDelegator = proj.AddComponent<ProjectileCollisionDelegator>();
			}
	
			collisionDelegator.onTriggerEnter = (coll, gameObject) => HandleProjectileCollision(coll, proj);

			if (enemyInfo.enemyData.isHoming && player != null)  // Homing logic
			{
				StartCoroutine(HomingProjectile(rb, player, enemyInfo.enemyData));
			}
			else
			{
				if (enemyInfo.enemyData.projectileTrajectory != 0)
				{
					direction = Quaternion.Euler(-enemyInfo.enemyData.projectileTrajectory, 0, 0) * direction;
				}
				rb.velocity = direction * enemyInfo.enemyData.projectileSpeed;
			}
	
			// Depending on your requirements, you might adjust this mapping.
			enemyToDataMap[proj] = enemyInfo;
			enemyInfo.lastFireTime = Time.time;
		}
	}
	
	// Handles what happens when a projectile collides with something
	public void HandleProjectileCollision(Collider collider, GameObject projectile)
	{
		Debug.Log("Collision detected with: " + collider.gameObject.name);
		if (collider.CompareTag("Player"))
		{
			EnemyData enemyData = enemyToDataMap[projectile].enemyData;
	
			if (enemyData != null)
			{
				Debug.Log("Projectile collided with Player. Damage Amount: " + enemyData.projectileDamage);
				PlayerManager.Instance.TakeDamage(enemyData.projectileDamage);
			}
			else
			{
				Debug.Log("Projectile collided with Player but enemyData is null!");
			}
	
			// Optionally, destroy the projectile upon collision
			Destroy(projectile);
		}
	}
	
	// Class to delegate projectile collision
	public class ProjectileCollisionDelegator : MonoBehaviour
	{
		public UnityAction<Collider, GameObject> onTriggerEnter;
	
		private void OnTriggerEnter(Collider other)
		{
			onTriggerEnter?.Invoke(other, this.gameObject);
		}
	}
	
	// Coroutine for projectiles that home in on the player
    private IEnumerator HomingProjectile(Rigidbody rb, GameObject target, EnemyData enemyData)
	{
		Vector3 midpoint = CalculateMidPoint(rb.position, target.transform.position, enemyData.projectileTrajectory);
		Vector3 directionToMidpoint = (midpoint - rb.position).normalized;
	
		while (rb && Vector3.Distance(rb.position, midpoint) > 0.5f)  // Check if rb still exists
		{
			rb.velocity = directionToMidpoint * enemyData.projectileSpeed;
			yield return new WaitForFixedUpdate();
		}
	
		while (target && rb)  // Check if both target and rb still exist
		{
			Vector3 homingDirection = (target.transform.position - rb.position).normalized;
			rb.velocity = homingDirection * enemyData.projectileSpeed;
			yield return new WaitForFixedUpdate();
		}
	}
	
	// Calculates a midpoint for the homing projectile's path
    Vector3 CalculateMidPoint(Vector3 start, Vector3 target, float trajectory)
    {
        Vector3 basicMidpoint = (start + target) / 2f;
        basicMidpoint.y += trajectory;
        return basicMidpoint;
    }
	
	// Returns the current number of active enemies
	public int GetCurrentEnemiesCount()
	{
		return currentEnemies.Count;
	}
	
	// Function to get the initial number of normal enemies
	public int GetInitialNormalEnemies()
    {
        return initialNormalEnemies;
    }
	
	// Function to set the number of normal enemies
	public void SetNormalEnemyCount(int count)
	{
		NumNormalEnemies = count;
	}
	
	// Handles damage taken by an enemy
	public void TakeDamage(GameObject damagedEnemy, int damageAmount)
	{
		if (!enemyCurrentHealth.ContainsKey(damagedEnemy))
		{
			return;
		}
	
		enemyCurrentHealth[damagedEnemy] -= damageAmount;
		int remainingHealth = enemyCurrentHealth[damagedEnemy];
	
		Debug.Log(damagedEnemy.name + " took " + damageAmount + " damage. Remaining health: " + remainingHealth);
	
		if (remainingHealth <= 0)
		{
			Die(damagedEnemy);
		}
	}
	
	// Handles the death of an enemy
    private void Die(GameObject deadEnemy)
	{
		Debug.Log("Enemy died.");
		currentEnemies.Remove(deadEnemy);
		enemyToDataMap.Remove(deadEnemy);
		enemyCurrentHealth.Remove(deadEnemy);
		Destroy(deadEnemy);
	
		// Check if all enemies have been defeated
		if(currentEnemies.Count == 0)
		{
			GameManager.Instance.PrepareForNextFloor();
			GameManager.Instance.AllEnemiesDefeated();
		}
	}
	
	// Prepares enemy settings for the next level/floor
	public void PrepareForNextFloor()
	{
		int nextFloor = GameManager.Instance.levelProgression.currentFloor + 1;
	
		// Calculate new enemy count for the next floor
		int newEnemyCount = GameManager.Instance.CalculateEnemyCountForFloor(nextFloor);
		NumNormalEnemies = newEnemyCount;
		initialNormalEnemies = NumNormalEnemies;
	
		GameManager.Instance.nextFloorEnemyCount = newEnemyCount;
		Debug.Log("Prepared for next floor. Next floor's enemy count: " + GameManager.Instance.nextFloorEnemyCount);
	}
}
