using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour {

    [Header("Canvas")]

    [SerializeField] private float canvasFadeDuration = 0.5f;
    [SerializeField] private CanvasInfo main;
    [SerializeField] private CanvasInfo save;
    [SerializeField] private CanvasInfo settings;
    [SerializeField] private CanvasInfo quit;
    private int currentMenu = 0;

    private void Start() {
        MenuBtn(0);
    }

    private void Update() {
        UserInterface.FadeCanvas(main.canvas, main.alpha, canvasFadeDuration / 2);
        UserInterface.FadeCanvas(save.canvas, save.alpha, canvasFadeDuration / 2);
        UserInterface.FadeCanvas(settings.canvas, settings.alpha, canvasFadeDuration / 2);
        UserInterface.FadeCanvas(quit.canvas, quit.alpha, canvasFadeDuration / 2);
    }

    public void MenuBtn(int canvasID) {
        if (!(canvasID == 0)) main.alpha = UserInterface.ActivateCanvas(main.canvas, false);
        if (!(canvasID == 1)) save.alpha = UserInterface.ActivateCanvas(save.canvas, false);
        if (!(canvasID == 2)) settings.alpha = UserInterface.ActivateCanvas(settings.canvas, false);
        if (!(canvasID == 3)) quit.alpha = UserInterface.ActivateCanvas(quit.canvas, false);
        Invoke(nameof(FadeIn), canvasFadeDuration / 2);
        currentMenu = canvasID;
    }

    private void FadeIn() {
        switch (currentMenu) {
            case 0:
                main.alpha = UserInterface.ActivateCanvas(main.canvas, true);
                break;
            case 1:
                save.alpha = UserInterface.ActivateCanvas(save.canvas, true);
                break;
            case 2:
                settings.alpha = UserInterface.ActivateCanvas(settings.canvas, true);
                break;
            case 3:
                quit.alpha = UserInterface.ActivateCanvas(quit.canvas, true);
                break;
        }
    }

    public void PlaySaveBtn(int saveFileID) {
        GameManager.currentSaveFile = saveFileID;
        SceneManager.LoadScene(1);
    }

    public void QuitGameBtn() {
        Application.Quit();
    }
}

[System.Serializable]
public class CanvasInfo {
    public CanvasGroup canvas;
    [HideInInspector] public float alpha = 0;
}