using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;  // Add this line.

public class Fader : MonoBehaviour
{
    public float fadeDamp = 1.0f;
    public string sceneName;
    public Color fadeColor = Color.black;
    private CanvasGroup myCanvas;
    private bool startFade = false;
    private bool fadeIn = false;

    private void Start()
    {
        myCanvas = GetComponent<CanvasGroup>();
        if (!myCanvas) Debug.LogError("No CanvasGroup component found.");

        var bg = GetComponent<Image>();
        if (!bg) Debug.LogError("No Image component found.");
        else bg.color = fadeColor;

        myCanvas.alpha = 0f;
    }

    private void Update()
	{
		if (!startFade) return;
	
		// Fade out only
		myCanvas.alpha += fadeDamp * Time.deltaTime;
		if (myCanvas.alpha >= 1)
		{
			// Switch to the target scene when the fade out effect is done.
			SceneManager.LoadScene(sceneName);
		}
	}
	
    public void StartFading(string newSceneName)
    {
        startFade = true;
        sceneName = newSceneName;
    }
}
