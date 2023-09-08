using UnityEngine;

[RequireComponent(typeof(CharacterController))]
public class PlayerController : MonoBehaviour
{
    private CharacterController characterController;
    private VariableJoystick variableJoystick;
    public float speed = 3.0f; // Speed of the player

    private void Awake()
    {
        characterController = GetComponent<CharacterController>();
    }

    private void Start()
	{
		variableJoystick = FindObjectOfType<VariableJoystick>();
		if (variableJoystick == null)
		{
			Debug.LogError("VariableJoystick component not found in the scene.");
		}
		else
		{
			variableJoystick.gameObject.SetActive(true); // Enable the joystick immediately.
		}
	}
	

    private void Update()
    {
        HandleJoystickMovement();
    }

    private void HandleJoystickMovement()
    {
        if (variableJoystick != null)
        {
            float horizontalInput = variableJoystick.Horizontal;
            float verticalInput = variableJoystick.Vertical;

            Vector3 movement = new Vector3(horizontalInput, 0f, verticalInput);
            movement = Quaternion.Euler(0f, 45f, 0f) * movement; // Adjust based on your game setup.
            movement = transform.TransformDirection(movement);
            characterController.Move(movement * speed * Time.deltaTime); // Use the speed variable
        }
    }
}
