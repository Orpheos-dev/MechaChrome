using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [System.Serializable]
    public class SceneCanvasPair
    {
        public string sceneName;
        [Tooltip("List of canvases associated with the scene.")]
        public List<GameObject> sceneCanvases;
        [Tooltip("Indices of the initial canvases to be active.")]
        public List<int> initialCanvasesActive;
    }

    public List<SceneCanvasPair> sceneCanvasList = new List<SceneCanvasPair>();
    private SceneCanvasPair currentSceneCanvasPair;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Move all canvases to DontDestroyOnLoad scene
            foreach (SceneCanvasPair pair in sceneCanvasList)
            {
                foreach (GameObject canvas in pair.sceneCanvases)
                {
                    DontDestroyOnLoad(canvas);
                }
            }

            // Check if the sceneCanvasList is not empty
            if (sceneCanvasList.Count > 0)
            {
                // Transition to the first scene in the list
                StartCoroutine(ChangeScene(sceneCanvasList[0].sceneName));
            }
        }
        else
        {
            Destroy(gameObject);
        }
    }

	// Switches to a new scene and sets up its UI
    public IEnumerator ChangeScene(string sceneName)
    {
        Debug.Log($"Changing scene to: {sceneName}");

        SceneCanvasPair pair = sceneCanvasList.Find(scp => scp.sceneName == sceneName);
        if (pair == null)
        {
            yield break;
        }

        // Deactivate all canvases
        foreach (SceneCanvasPair scp in sceneCanvasList)
        {
            foreach (GameObject canvas in scp.sceneCanvases)
            {
                canvas.SetActive(false);
            }
        }

        // Async load scene
        AsyncOperation asyncOperation = SceneManager.LoadSceneAsync(sceneName);
        while (!asyncOperation.isDone)
        {
            yield return null;
        }

        // Activate the initial active canvases for the current scene
        for (int i = 0; i < pair.initialCanvasesActive.Count; i++)
        {
            pair.sceneCanvases[pair.initialCanvasesActive[i]].SetActive(true);
        }

        currentSceneCanvasPair = pair;
    }

	// Activates initial canvases when a new scene is loaded
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Set the initial active canvases
        for (int i = 0; i < currentSceneCanvasPair.initialCanvasesActive.Count; i++)
        {
            currentSceneCanvasPair.sceneCanvases[currentSceneCanvasPair.initialCanvasesActive[i]].SetActive(true);
        }
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    // Function to toggle individual canvases in the current scene
    public void ToggleCanvas(int index)
    {
        if (currentSceneCanvasPair != null && index >= 0 && index < currentSceneCanvasPair.sceneCanvases.Count)
        {
            GameObject canvas = currentSceneCanvasPair.sceneCanvases[index];
            canvas.SetActive(!canvas.activeSelf);
        }
    }
    
	// Initiates a scene change to a specific scene
    public void ChangeToSpecificScene(string sceneName)
    {
        StartCoroutine(ChangeScene(sceneName));
    }
	
	// Activates the "Dead" canvas when the player dies
	public void ShowDeadCanvas()
	{
		const int DEAD_CANVAS_INDEX = 1; // If the dead canvas is different, change the index number below
	
		if (currentSceneCanvasPair != null 
			&& DEAD_CANVAS_INDEX < currentSceneCanvasPair.sceneCanvases.Count 
			&& !currentSceneCanvasPair.sceneCanvases[DEAD_CANVAS_INDEX].activeSelf)
		{
			currentSceneCanvasPair.sceneCanvases[DEAD_CANVAS_INDEX].SetActive(true);
		}
	}
}
