//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//
//public class PauseButtonScript : MonoBehaviour
//{
//    private Button button;
//
//    private void Start()
//    {
//        button = GetComponent<Button>();
//        SceneManager.sceneLoaded += OnSceneLoaded;
//    }
//
//    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
//    {
//		Debug.Log("OnSceneLoaded method called in PauseButtonScript");
//        if (scene.name == "Level")  // replace "Level" with the name of your Level scene
//        {
//            button.onClick.AddListener(GameManager.Instance.TogglePauseGame);
//        }
//    }
//
//    private void OnDestroy()
//    {
//        SceneManager.sceneLoaded -= OnSceneLoaded;
//    }
//}
