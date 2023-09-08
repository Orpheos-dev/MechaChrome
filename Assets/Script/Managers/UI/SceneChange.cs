//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;
//using UnityEngine.SceneManagement;
//
//public class ChangeScene : MonoBehaviour
//{
//    public UIManager UIManager;
//
//    public void MoveToScene(int sceneIndex)
//    {
//        Debug.Log("Moving to scene index: " + sceneIndex);
//
//        // Check if the main scene is selected
//        if (sceneIndex == UIManager.SceneArray.IndexOf("Main"))
//        {
//            Debug.Log("Main scene selected. Showing main canvas.");
//            // Let's suppose MainCanvas was the first element in the canvasArray
//            UIManager.ToggleCanvas(0);
//        }
//
//        // Load the scene
//        UIManager.ChangeScene(sceneIndex);
//    }
//}
