using UnityEngine;
using UnityEngine.UI;

public static class Initiate
{
    public static void Fade(string scene, float multiplier, Color col = default(Color))
    {
        GameObject faderObj = new GameObject("Fader");
        var myCanvas = faderObj.AddComponent<Canvas>();
        myCanvas.renderMode = RenderMode.ScreenSpaceOverlay;

        faderObj.AddComponent<Fader>();
        faderObj.AddComponent<CanvasGroup>();
        faderObj.AddComponent<Image>();  // This line uses the Image class from the UnityEngine.UI namespace.

        Fader fader = faderObj.GetComponent<Fader>();
        fader.fadeDamp = multiplier;
        fader.fadeColor = col != default(Color) ? col : Color.black;

        fader.StartFading(scene);
    }
}
