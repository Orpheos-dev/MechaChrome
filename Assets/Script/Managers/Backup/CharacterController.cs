//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.InputSystem;
//
//[RequireComponent(typeof(CharacterController))]
//public class PlayerController : MonoBehaviour
//{
//    public LevelManager levelManager;
//	public UIManager uiManager; // Reference to UIManager
//    public float moveSpeed = 3f;
//    public float inputDeadzone = 0.1f;
//    public float smoothTime = 0.1f;
//    public bool canMove = true;
//
//    public GameObject projectilePrefab;
//    public Transform projectileSpawnPoint;
//    public float fireRate = 1f / 3f;
//
//    private CharacterController characterController;
//    private Vector3 movementVelocity;
//    private float nextFireTime;
//
//    private List<GameObject> enemies = new List<GameObject>();
//
//    private VariableJoystick variableJoystick;
//
//    private void Awake()
//    {
//        characterController = GetComponent<CharacterController>();
//        canMove = true;
//        levelManager = FindObjectOfType<LevelManager>();
//        if (levelManager == null)
//        {
//            Debug.LogError("LevelManager not found in the scene.");
//        }
//    }
//
//    private void Start()
//    {
//        variableJoystick = FindObjectOfType<VariableJoystick>();
//        if (variableJoystick == null)
//        {
//            Debug.LogError("VariableJoystick component not found in the scene.");
//        }
//    }
//
//    private void Update()
//	{
//		enemies = new List<GameObject>(GameObject.FindGameObjectsWithTag("Enemy"));
//		if (CanFire())
//		{
//			FireProjectile();
//		}
//	
//		// Check if the 'Reward' canvas is active
//		if(uiManager.canvasArray[2].activeSelf)
//		{
//			// If it is, disable movement
//			TogglePlayerMovement(false);
//		}
//		else
//		{
//			// If it's not, enable movement
//			TogglePlayerMovement(true);
//		}
//	
//		if (canMove)
//		{
//			HandleJoystickMovement();
//		}
//	}
//	
//    private void HandleJoystickMovement()
//    {
//        if (variableJoystick != null)
//        {
//            float horizontalInput = variableJoystick.Horizontal;
//            float verticalInput = variableJoystick.Vertical;
//
//            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
//            movement = Quaternion.Euler(0f, 45f, 0f) * movement;
//            movement = transform.TransformDirection(movement);
//            movement *= moveSpeed;
//            movementVelocity = Vector3.Lerp(movementVelocity, movement, smoothTime);
//            if (canMove)
//            {
//                characterController.Move(movementVelocity * Time.deltaTime);
//            }
//        }
//    }
//
//    public void TogglePlayerMovement(bool enable)
//    {
//        canMove = enable;
//    }
//
//    private bool CanFire()
//    {
//        bool canFire = Time.time >= nextFireTime && enemies.Count > 0;
//        return canFire;
//    }
//
//    private void FireProjectile()
//    {
//        GameObject nearestEnemy = null;
//        float nearestDistance = Mathf.Infinity;
//        foreach (GameObject enemy in enemies)
//        {
//            float distance = Vector3.Distance(transform.position, enemy.transform.position);
//            if (distance < nearestDistance)
//            {
//                nearestEnemy = enemy;
//                nearestDistance = distance;
//            }
//        }
//
//        if (nearestEnemy != null)
//        {
//            Vector3 direction = (nearestEnemy.transform.position - projectileSpawnPoint.position).normalized;
//            Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.LookRotation(direction));
//        }
//        nextFireTime = Time.time + 1f / fireRate;
//    }
//
//    private void OnTriggerEnter(Collider other)
//	{
//		if (other.gameObject.CompareTag("Finish"))
//		{
//			levelManager.NextFloor();
//		}
//	
//		// Check if the player collided with the reward and it's not currently processing a reward
//		if (other.gameObject.CompareTag("Reward"))
//		{
//			// Destroy the reward object
//			Destroy(other.gameObject);
//			// Toggle the canvas
//			uiManager.ToggleCanvas(2);
//			// Disable movement
//			TogglePlayerMovement(false);
//			// Populate the skill options
//			uiManager.PopulateSkillOptions();
//		}
//	}
//	
//}
//