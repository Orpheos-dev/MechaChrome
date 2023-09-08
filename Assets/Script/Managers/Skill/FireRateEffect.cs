using UnityEngine;
using System.Collections.Generic;

public class FireRateEffect : MonoBehaviour
{
	private SpawnEffect spawnEffect; 
	private DamageEffect damageEffect; 
	
	public GameObject projectilePrefab;
    public float fireRate; // Fire rate in seconds
    
    private float nextFireTime;
    public bool isActive = false;

    private void Awake()
    {
        spawnEffect = GetComponent<SpawnEffect>();
		damageEffect = GetComponent<DamageEffect>();
    }
	
	private void Update()
    {
        // Only execute if the skill is active
        if (isActive)
        {
            if (Time.time >= nextFireTime)
            {
                if (HasEnemies())
                {
                    // Make all missile bases shoot
                    List<GameObject> missileBases = spawnEffect.GetMissileBases();
                    foreach (var missileBase in missileBases)
                    {
                        ShootProjectile(missileBase.transform);
                    }
                    nextFireTime = Time.time + fireRate;
                }
            }
        }
    }

	// Shoots a projectile from the given spawn point.
    private void ShootProjectile(Transform spawnPoint)
	{
		// Instantiate the projectile prefab at the spawn point
		GameObject projectile = Instantiate(projectilePrefab, spawnPoint.position, spawnPoint.rotation);
		
		// Call the RegisterProjectile method in DamageEffect to attach the delegate
        damageEffect.RegisterProjectile(projectile);
	
		// Set the projectile size
		//projectile.transform.localScale = Vector3.one;  // Reset to original size for now
	
		// Find the nearest enemy
		GameObject nearestEnemy = FindNearestEnemy();
	
		if (nearestEnemy != null)
		{
			// Set the projectile's direction to target the nearest enemy
			Vector3 direction = (nearestEnemy.transform.position - spawnPoint.position).normalized;
			projectile.transform.rotation = Quaternion.LookRotation(direction);
	
			// Assign velocity to the projectile
			Rigidbody projectileRigidbody = projectile.GetComponent<Rigidbody>();
			if (projectileRigidbody != null)
			{
				float speed = 10f;  // You can adjust this value as needed
				projectileRigidbody.velocity = direction * speed;
			}
		}
	}
	
	// Finds the nearest enemy to the player.
    private GameObject FindNearestEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestEnemy = null;
        float nearestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distance = Vector3.Distance(transform.position, enemy.transform.position);
            if (distance < nearestDistance)
            {
                nearestEnemy = enemy;
                nearestDistance = distance;
            }
        }

        return nearestEnemy;
    }

	// Checks if there are any enemies in the scene.
    private bool HasEnemies()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        return enemies.Length > 0;
    }
}
