//using UnityEngine;
//
//public class RewardPanelScript : MonoBehaviour
//{
//    public static RewardPanelScript instance;
//    public PlayerController playerController; // Reference to the player controller
//    public VariableJoystick variableJoystick; // Reference to the joystick
//
//    private void Awake()
//    {
//        if (instance == null)
//        {
//            instance = this;
//        }
//        else
//        {
//            Destroy(gameObject);
//            return;
//        }
//
//        // Find the VariableJoystick component in the scene
//        variableJoystick = FindObjectOfType<VariableJoystick>();
//
//        // Hide the panel at start
//        gameObject.SetActive(false);
//    }
//
//    public void ShowRewardPanel()
//    {
//        // Show the panel
//        gameObject.SetActive(true);
//
//        // Disable player movement and joystick
//        if (playerController != null)
//        {
//            playerController.TogglePlayerMovement(false);
//        }
//
//        if (variableJoystick != null)
//        {
//            variableJoystick.enabled = false;
//        }
//    }
//
//    // Call this method to hide the panel and enable player movement
//    public void HideRewardPanel()
//    {
//        gameObject.SetActive(false);
//
//        if (playerController != null)
//        {
//            playerController.TogglePlayerMovement(true);
//        }
//
//        if (variableJoystick != null)
//        {
//            variableJoystick.enabled = true;
//        }
//    }
//}
//