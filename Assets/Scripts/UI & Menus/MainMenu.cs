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

    // Starts on the Main Menu Canvas
    private void Start() {
        MenuBtn(0);
    }

    // More info on Method definition
    private void Update() {
        UserInterface.FadeCanvas(main.canvas, main.alpha, canvasFadeDuration / 2);
        UserInterface.FadeCanvas(save.canvas, save.alpha, canvasFadeDuration / 2);
        UserInterface.FadeCanvas(settings.canvas, settings.alpha, canvasFadeDuration / 2);
        UserInterface.FadeCanvas(quit.canvas, quit.alpha, canvasFadeDuration / 2);
    }

    // Sets every canvas alpha to 0, then Invokes 'FadeIn'
    public void MenuBtn(int canvasID) {
        main.alpha = UserInterface.ActivateCanvas(main.canvas, false);
        save.alpha = UserInterface.ActivateCanvas(save.canvas, false);
        settings.alpha = UserInterface.ActivateCanvas(settings.canvas, false);
        quit.alpha = UserInterface.ActivateCanvas(quit.canvas, false);
        Invoke(nameof(FadeIn), canvasFadeDuration / 2);
        currentMenu = canvasID;
    }

    // Sets the current menu's alpha to 1
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

// Stores relevant info about a CanvasGroup
[System.Serializable]
public class CanvasInfo {
    public CanvasGroup canvas;
    [HideInInspector] public float alpha = 0; // not the current alpha, but the target alpha ('UserInterface.FadeCanvas' for more details)
}