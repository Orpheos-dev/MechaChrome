//using UnityEngine;
//
//public class EnemySpawner : MonoBehaviour
//{
//    public GameObject enemyPrefab;
//    public Transform[] spawnPoints;
//
//    public void SpawnEnemy()
//    {
//        int randomSpawnIndex = Random.Range(0, spawnPoints.Length);
//        Transform spawnPoint = spawnPoints[randomSpawnIndex];
//
//        GameObject newEnemy = Instantiate(enemyPrefab, spawnPoint.position, spawnPoint.rotation);
//        newEnemy.name = "Enemy"; // Set a custom name for the spawned enemy
//
//        Enemy enemyComponent = newEnemy.GetComponent<Enemy>();
//
//        // Customize enemy properties if needed, e.g., set AI behavior, configure HP, etc.
//
//        // ...
//    }
//}
//