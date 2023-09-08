//using System.Collections;
//using UnityEngine;
//using UnityEngine.UI;
//using UnityEngine.SceneManagement;
//
//public class GameManager : MonoBehaviour
//{
//    public static GameManager Instance;
//
//    public int playerXP;
//    public int playerCurrency;
//
//    private bool isGamePaused = false;
//
//    public Text XPText;  // Drag and drop your XP text object in the inspector
//    public Text CurrencyText;  // Drag and drop your Currency text object in the inspector
//
//    // New field for ButtonPause
//    private Button ButtonPause;
//
//    private void Awake()
//    {
//        if (Instance == null)
//        {
//            Instance = this;
//            DontDestroyOnLoad(gameObject);
//        }
//        else
//        {
//            Destroy(gameObject);
//        }
//    }
//
//    public void TogglePauseGame()
//    {
//        GameObject pauseCanvas = UIManager.Instance.canvasArray[3];
//
//        if (pauseCanvas == null)
//        {
//            Debug.LogError("PauseCanvas not found or missing in canvasArray.");
//            return;
//        }
//
//        if (isGamePaused)
//        {
//            // Unpause the game
//            Time.timeScale = 1;
//            isGamePaused = false;
//            pauseCanvas.SetActive(false);
//        }
//        else
//        {
//            // Pause the game
//            Time.timeScale = 0;
//            isGamePaused = true;
//            pauseCanvas.SetActive(true);
//        }
//    }
//    
//    public void LoadMainMenuScene()
//    {
//        Time.timeScale = 1; // Make sure time is running normally
//        SceneManager.LoadScene("Main");  // replace "Main" with the name of your main menu scene
//    }
//}
//