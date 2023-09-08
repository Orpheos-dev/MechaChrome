//using UnityEngine;
//
//public class EnemyProjectile : MonoBehaviour
//{
//    public int damage = 5; // Damage inflicted by this projectile
//
//    private void Start()
//    {
//        // Draw a ray in the direction of the projectile
//        Debug.DrawRay(transform.position, transform.forward, Color.red, 5f);
//    }
//
//    private void OnTriggerEnter(Collider other)
//    {
//        Debug.Log("Projectile hit: " + other.gameObject.name); // Print the name of the collided object
//
//        // Check if the projectile hit the player
//        if (other.gameObject.CompareTag("Player"))
//        {
//            Debug.Log("Hit player"); // This message should be printed if the collided object has the "Player" tag
//
//            // Get the CharacterStats component from the player
//            CharacterStats playerStats = other.gameObject.GetComponent<CharacterStats>();
//
//            // If playerStats exists
//            if (playerStats != null)
//            {
//                Debug.Log("Player has CharacterStats component"); // This message should be printed if the player has the CharacterStats component
//
//                // Call the TakeDamage function on playerStats
//                playerStats.TakeDamage(damage);
//            }
//            else
//            {
//                Debug.Log("Player does not have CharacterStats component"); // This message should be printed if the player does not have the CharacterStats component
//            }
//
//            // Destroy the projectile
//            Destroy(gameObject);
//        }
//        else
//        {
//            Debug.Log("Hit something else"); // This message should be printed if the collided object does not have the "Player" tag
//        }
//    }
//}
//