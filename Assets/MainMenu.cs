using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainMenu : MonoBehaviour {

    [Header("Canvas")]
    [SerializeField] private float canvasFadeDuration = 0.5f;
    [SerializeField] private CanvasGroup mainCanvas;
    private float mainCanvasAlpha = 1;
    [SerializeField] private CanvasGroup settingsCanvas;
    private float settingsCanvasAlpha = 0;
    [SerializeField] private CanvasGroup quitCanvas;
    private float quitCanvasAlpha = 0;

    private void Awake() {
    }

    private void Start() {
        UserInterface.ActivateCanvas(mainCanvas, ref mainCanvasAlpha, false);
        UserInterface.ActivateCanvas(settingsCanvas, ref settingsCanvasAlpha, false);
        UserInterface.ActivateCanvas(quitCanvas, ref quitCanvasAlpha, true);
    }

    private void Update() {
        UserInterface.FadeCanvas(mainCanvas, mainCanvasAlpha, canvasFadeDuration);
        UserInterface.FadeCanvas(settingsCanvas, settingsCanvasAlpha, canvasFadeDuration);
        UserInterface.FadeCanvas(quitCanvas,quitCanvasAlpha, canvasFadeDuration);
    }

}
