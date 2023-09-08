using UnityEngine;
using static PlayerManager;

public class DamageEffect : MonoBehaviour
{
	private EnemyManager enemyManager;
	
    public int projectileDamage; // Set your desired projectile damage value here.
    public bool isActive = false;

    private void Awake()
    {
        // Find the EnemyManager in the scene
        enemyManager = FindObjectOfType<EnemyManager>();
        if (enemyManager == null)
        {
            Debug.LogError("EnemyManager not found in the scene.");
        }
    }

    // called by FireRateEffect when a projectile is instantiated.
    public void RegisterProjectile(GameObject projectile)
    {
        Debug.Log("Projectile registered.");
        if (projectile == null) return;

        // Attach the ProjectileCollisionDelegator script to the projectile.
        ProjectileCollisionDelegator delegator = projectile.GetComponent<ProjectileCollisionDelegator>();
        if (delegator == null)
        {
            delegator = projectile.AddComponent<ProjectileCollisionDelegator>();
        }

        // Subscribe to the delegate for collision handling.
        delegator.onTriggerEnter += HandleProjectileCollision;

        // Assign collision detection method to the projectile.
        Collider projectileCollider = projectile.GetComponent<Collider>();
        if (projectileCollider != null)
        {
            projectileCollider.isTrigger = true; // Ensure the collider is set to trigger
            projectile.tag = "Projectile"; // Ensure the projectile has the "Projectile" tag
        }
        else
        {
            Debug.LogWarning("No collider found on the projectile.");
        }
    }

	// called when this object's collider triggers a collision.
    private void OnTriggerEnter(Collider collider)
    {
        Debug.Log("Something entered the trigger.");
        // If this is not an active damage effect, ignore
        if (!isActive) return;

        // Check for collision with enemies
        if (collider.CompareTag("Enemy") && enemyManager != null)
        {
            enemyManager.TakeDamage(collider.gameObject, projectileDamage);
            Destroy(gameObject);  // Destroy the projectile after dealing damage
        }
    }

	// called when the projectile triggers a collision.
    private void HandleProjectileCollision(Collider collider, GameObject projectile)
    {
        if (collider.CompareTag("Enemy"))
        {
            if (enemyManager != null)
            {
                enemyManager.TakeDamage(collider.gameObject, projectileDamage);
            }
            else
            {
                Debug.LogError("EnemyManager reference not set in DamageEffect.");
            }
            Destroy(projectile);  // Destroy the projectile
        }
    }

    public void ApplyDamageEffect()
    {
        isActive = true;
    }

    public void RemoveDamageEffect()
    {
        isActive = false;
    }
}
