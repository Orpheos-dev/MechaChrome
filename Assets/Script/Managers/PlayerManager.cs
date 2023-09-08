using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
	public EnemyManager enemyManager;
    public CameraManager cameraManager;

    [System.Serializable]
    public class PlayerStats
    {
        [Header("Stats")]
        public GameObject playerPrefab;
        public Vector3 positionOffset;
        public int maxHealth = 100;

        [Header("Projectiles")]
        public GameObject projectilePrefab;
        public float projectileTrajectory = 1f; // This will be used as a pitch adjustment
        public float projectileSpeed = 1f;
        public float projectileFireRate = 1f;
		public Vector3 projectileOffset = new Vector3(0f, 0f, 1f);
		public bool isHoming = false; 
        public int projectileDamage = 5;

        [Header("Movement")]
        public float inputDeadzone = 0.1f;
        public float smoothTime = 0.1f;
        public float moveSpeed = 3f;
    }

    [System.Serializable]
    public class PlayerSkills
    {
        [Header("Skills")]
        public GameObject skillManagerPrefab;
        public bool skillAllowed;
    }

    [System.Serializable]
    public class PlayerItems
    {
        [Header("Items")]
        public GameObject itemManagerPrefab;
        public bool itemAllowed;
    }

    [Header("Player Settings")]
    public PlayerStats playerStats;

    [Header("Player Skills")]
    public PlayerSkills playerSkills;

    [Header("Player Items")]
    public PlayerItems playerItems;

    public GameObject currentPlayer; // Storing the reference to the spawned player.
    private float lastFireTime = -Mathf.Infinity;
	private float homingDelay = 1.0f;
	private int currentHealth;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
			DontDestroyOnLoad(gameObject);
            Debug.Log("PlayerManager instantiated.");
        }
        else
        {
            Debug.Log("Destroying extra PlayerManager instance");
            Destroy(gameObject);
        }
    }

	// Initialize the player and related components
    public void InitializePlayer()
    {
        Debug.Log("PlayerManager started");
        if (cameraManager == null)
        {
            cameraManager = FindObjectOfType<CameraManager>();
        }
        StartCoroutine(CheckForEnemiesCoroutine());
        currentHealth = playerStats.maxHealth;
    }

	// Spawn the player at a random position from a list of positions
    public void SpawnPlayerAtPosition(List<Vector3> positions)
    {
        if (positions.Count == 0)
        {
            Debug.LogWarning("No valid positions for player spawn!");
            return;
        }

        Vector3 spawnPosition = positions[Random.Range(0, positions.Count)];
        spawnPosition += playerStats.positionOffset;

        currentPlayer = Instantiate(playerStats.playerPrefab, spawnPosition, Quaternion.identity);
		Debug.Log($"PlayerManager currentPlayer: {currentPlayer}");
		currentPlayer.AddComponent<PlayerCollisionHandler>();
        if (cameraManager != null)
        {
            cameraManager.SetActualPlayerTransform(currentPlayer.transform);
        }
		
        PlayerController playerController = currentPlayer.GetComponent<PlayerController>();
        playerController.speed = playerStats.moveSpeed;
    }

	// Coroutine to check for enemies and fire projectiles
    private IEnumerator CheckForEnemiesCoroutine()
	{
		while (true)
		{
			if (AnyEnemiesPresent() && currentPlayer != null)
			{
				Vector3 fireStartPosition = currentPlayer.transform.position + currentPlayer.transform.forward + playerStats.projectileOffset;
				Vector3 fireDirection = Vector3.forward;
	
				FireProjectile(fireStartPosition, fireDirection);
			}
			yield return new WaitForSeconds(playerStats.projectileFireRate);
		}
	}
	
	// Check if any enemies are present in the scene
    private bool AnyEnemiesPresent()
	{
		bool enemyPresent = GameObject.FindGameObjectWithTag("Enemy") != null;
		return enemyPresent;
	}

	// Fire a projectile from the player
    public void FireProjectile(Vector3 startPosition, Vector3 direction)
    {
        if (Time.time - lastFireTime >= playerStats.projectileFireRate)
        {
            GameObject proj = Instantiate(playerStats.projectilePrefab, startPosition + playerStats.projectileOffset, Quaternion.identity);
            Rigidbody rb = proj.GetComponent<Rigidbody>();

            ProjectileCollisionDelegator collisionDelegator = proj.GetComponent<ProjectileCollisionDelegator>();
            if (!collisionDelegator)
            {
                collisionDelegator = proj.AddComponent<ProjectileCollisionDelegator>();
            }
            collisionDelegator.onTriggerEnter += HandleProjectileCollision;

            if (playerStats.isHoming)  // Homing logic
            {
                GameObject target = FindClosestEnemy();
                if (target != null)  // Only adjust direction if a target is found
                {
                    StartCoroutine(HomingProjectile(rb, target));
                }
            }
            else // If not homing, just follow the direction (which is forward)
            {
                if (playerStats.projectileTrajectory != 0)
                {
                    direction = Quaternion.Euler(-playerStats.projectileTrajectory, 0, 0) * Vector3.up;
                }
                rb.velocity = direction * playerStats.projectileSpeed;
            }

            lastFireTime = Time.time;
        }
        else
        {
            Debug.Log("Projectile fire rate not met. Waiting...");
        }
    }

	// Handle projectile collision with enemies
    private void HandleProjectileCollision(Collider collider, GameObject projectile)
	{
		if (collider.CompareTag("Enemy"))
		{
			if (enemyManager != null)
			{
				enemyManager.TakeDamage(collider.gameObject, playerStats.projectileDamage);
			}
			else
			{
				Debug.LogError("EnemyManager reference not set in PlayerManager.");
			}
			Destroy(projectile);  // Destroy the projectile
		}       
	}
	
	// Handle collision with progress and skill reward objects
	public void HandleProgressCollision()
	{
		GameManager.Instance.HandleFinishCollision();
	}
	public void HandleSkillRewardCollision()
    {
		SkillManager.Instance.PopulateSkillOptions();
		UIManager.Instance.ToggleCanvas(3);
    }
	
	// Inner class for handling player collisions
	public class PlayerCollisionHandler : MonoBehaviour
	{
		private void OnTriggerEnter(Collider other)
		{
			if (other.CompareTag("Progress"))
			{
				PlayerManager.Instance.HandleProgressCollision();
			}
			if (other.CompareTag("RewardSkill"))
			{
				PlayerManager.Instance.HandleSkillRewardCollision();
			}
		}
	}
	
	// Inner class for delegating projectile collisions
	public class ProjectileCollisionDelegator : MonoBehaviour
	{
		public UnityAction<Collider, GameObject> onTriggerEnter;
	
		private void OnTriggerEnter(Collider other)
		{
			onTriggerEnter?.Invoke(other, this.gameObject);
		}
	}
	
	// Coroutine for homing projectiles
	private IEnumerator HomingProjectile(Rigidbody rb, GameObject target)
	{
		if (!rb || !target)
		{
			yield break;
		}
	
		Vector3 midpoint = CalculateMidPoint(rb.position, target.transform.position);
	
		// Move towards the midpoint
		while (Vector3.Distance((rb ? rb.position : Vector3.zero), midpoint) > 0.5f && rb && target) // Check rb before accessing rb.position
		{
			if (!rb || !target)
			{
				yield break;
			}
		
			Vector3 directionToMidpoint = (midpoint - (rb ? rb.position : Vector3.zero)).normalized; // Recalculate inside the loop
			rb.velocity = directionToMidpoint * playerStats.projectileSpeed;
			yield return new WaitForFixedUpdate();
		}
		
		// Homing phase once you've reached the midpoint
		while (target && rb)
		{
			if (!rb || !target)
			{
				yield break;
			}
	
			Vector3 homingDirection = (target.transform.position - rb.position).normalized;
			rb.velocity = homingDirection * playerStats.projectileSpeed;
			yield return new WaitForFixedUpdate();
		}
	}
	
	// Calculate the midpoint for homing projectiles
	Vector3 CalculateMidPoint(Vector3 start, Vector3 target)
	{
		// Find the midpoint without any trajectory adjustments
		Vector3 basicMidpoint = (start + target) / 2f;
	
		// Adjust for trajectory (upwards or downwards)
		basicMidpoint.y += playerStats.projectileTrajectory;
	
		return basicMidpoint;
	}
	
	// Find the closest enemy to the player
    private GameObject FindClosestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return null;  // No enemies
    
        GameObject closest = null;
        float closestDistance = Mathf.Infinity;
    
        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(currentPlayer.transform.position, enemy.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closest = enemy;
            }
        }
        return closest;
    }
	
	// Handle player taking damage
	public void TakeDamage(int damageAmount)
	{
		currentHealth -= damageAmount;
		currentHealth = Mathf.Clamp(currentHealth, 0, playerStats.maxHealth);
		
		Debug.Log("Player took damage: " + damageAmount + ". Current Health: " + currentHealth);
	
		UpdateHealthBar();
		CheckDeath();
	}
	
	// Update the player's health bar (implementation needed)
	private void UpdateHealthBar()
	{
		// Code to update the health bar
	}
	
	// Check if the player has died
	private void CheckDeath()
	{
		if (currentHealth <= 0)
		{
			Debug.Log("Player has died.");
			if (currentPlayer != null)
			{
				Destroy(currentPlayer);  // Destroying the player GameObject
			}
			GameManager.Instance.PlayerDied();
		}
	}
}
