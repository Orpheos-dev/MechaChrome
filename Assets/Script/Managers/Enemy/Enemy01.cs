//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class Enemy : MonoBehaviour
//{
//	// Add a public field for the LevelManager
//    public LevelManager levelManager;
//	// Enemy variables
//    public int maxHp = 50;
//    private int currentHp;
//    public int defense = 0;
//    public int damage = 5;
//    public float projectileSpeed = 1f;
//    public float projectileFireRate = 1f;
//    public GameObject projectilePrefab;
//    public GameObject healthBarPrefab; // The health bar prefab
//
//    private GameObject player; // Reference to the player
//    private float lastFireTime;
//    private GameObject healthBar; // Instance of the health bar
//    private Transform foregroundTransform; // The foreground part of the health bar
//    private float originalScale; // The original scale of the foreground
//
//
//	public void SetLevelManager(LevelManager manager)
//	{
//		levelManager = manager;
//	}
//	
//    public int CurrentHp
//    {
//        get { return currentHp; }
//    }
//    public int MaxHp
//    {
//        get { return maxHp; }
//    }
//
//    private void Start()
//    {
//        currentHp = maxHp;
//        lastFireTime = Time.time;
//        player = GameObject.FindGameObjectWithTag("Player"); // Assuming your player object is tagged as "Player"
//
//        // Instantiate the health bar and position it 30 units above the enemy
//        healthBar = Instantiate(healthBarPrefab, transform.position + new Vector3(0, 30, 0), Quaternion.identity, transform);
//        // Scale down the health bar by half
//        healthBar.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
//        foregroundTransform = healthBar.transform.Find("Foreground");
//        originalScale = foregroundTransform.localScale.x;
//    }
//
//    private void Update()
//    {
//        if (Time.time > lastFireTime + projectileFireRate)
//        {
//            FireProjectile();
//            lastFireTime = Time.time;
//        }
//
//        // Update the health bar
//        UpdateHealthBar();
//    }
//
//
//    private void FireProjectile()
//    {
//        if (player != null)
//        {
//            // Instantiate the projectile and fire it towards the player
//            GameObject projectile = Instantiate(projectilePrefab, transform.position, Quaternion.identity);
//            Vector3 direction = (player.transform.position - transform.position).normalized;
//            projectile.GetComponent<Rigidbody>().velocity = direction * projectileSpeed;
//
//            // Destroy the projectile after some time (optional)
//            Destroy(projectile, 5f);
//        }
//    }
//
//    private void UpdateHealthBar()
//    {
//        // Point the health bar towards the camera
//        healthBar.transform.LookAt(Camera.main.transform.position, Vector3.up);
//
//        // Calculate the health percentage
//        float healthPercent = (float)CurrentHp / MaxHp;
//
//        // Update the health bar scale
//        Vector3 scale = foregroundTransform.localScale;
//        scale.x = healthPercent * originalScale;
//        foregroundTransform.localScale = scale;
//    }
//
//    public void TakeDamage(int damage)
//    {
//        currentHp -= Mathf.Max(damage - defense, 0); // Damage is reduced by defense, but never less than 0
//
//        if (currentHp <= 0)
//        {
//            Die();
//        }
//    }
//
//	private void Die()
//	{
//		// Handle enemy death logic, e.g., play an animation, spawn particles, etc.
//	
//		if (levelManager != null)
//		{
//			// Remove this enemy from the currentEnemies list in the LevelManager
//			levelManager.RemoveEnemy(gameObject);
//		}
//	
//		Destroy(gameObject); // Destroy the enemy
//		Destroy(healthBar); // Destroy the health bar
//	}
//	
//}
//