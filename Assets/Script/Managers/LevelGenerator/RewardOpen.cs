//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//
//public class RewardOpen : MonoBehaviour
//{
//    // Variable to hold a reference to the UIManager
//    public UIManager uiManager;
//
//    // Method that is called when another object enters the finish's collider
//    void OnTriggerEnter(Collider other)
//    {
//        // Check if the other object is the player
//        if (other.gameObject.tag == "Player")  // Replace "Player" with the tag you're using for the player
//        {
//            // Call the ToggleCanvas method on the uiManager reference
//            uiManager.ToggleCanvas(2);  // The index of the canvas to be enabled
//        }
//    }
//}
