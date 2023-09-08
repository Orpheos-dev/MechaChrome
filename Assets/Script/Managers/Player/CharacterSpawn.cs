//using UnityEngine;
//
//public class CharacterSpawner : MonoBehaviour
//{
//    public GameObject characterPrefab;
//    public Transform spawnPoint;
//
//    private GameObject spawnedCharacter;
//
//    private void Start()
//    {
//        SpawnCharacter();
//    }
//
//    private void SpawnCharacter()
//    {
//        spawnedCharacter = Instantiate(characterPrefab, spawnPoint.position, spawnPoint.rotation);
//
//        // Get the original name of the prefab
//        string prefabName = characterPrefab.name;
//
//        // Assign the original prefab name to the spawned player
//        spawnedCharacter.name = prefabName;
//    }
//}
