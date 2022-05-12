using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

    public static UserInterface Instance { get; private set; }

    [Header("Debug SaveSystem")]

    [SerializeField] private bool saveTheData = false;

    [Header("Info")]

    [SerializeField] private float canvasFadeDuration = 0.5f;
    public static bool isGamePaused = false;

    [Header("In Game")]

    [SerializeField] private CanvasGroup ingameCanvas;
    private float ingameCanvasAlpha = 1;
    [SerializeField] private Image healthBar;
    public Image swordBtnImage; // Save system OR object interaction wil set these.
    public Image hookBtnImage;
    public Image amuletBtnImage;

    [Header("Pause Menu")]

    [SerializeField] private CanvasGroup pauseCanvas;
    private float pauseCanvasAlpha = 0;
    [SerializeField] private KeyCode pauseKey;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        SaveData save = SaveSystem.LoadProgress();
        if (save != null) {
            swordBtnImage.enabled = save.hasSword;
            hookBtnImage.enabled = save.hasHook;
            amuletBtnImage.enabled = save.hasAmulet;
        }
    }

    private void Update() {
        // Debug Save
        if (saveTheData) {
            saveTheData = false;
            SaveSystem.SaveProgress();
        }

        // Input
        if (Input.GetKeyDown(pauseKey)) {
            if (ingameCanvas.interactable) {
                ActivateCanvas(ingameCanvas, ref ingameCanvasAlpha, false, true);
                ActivateCanvas(pauseCanvas, ref pauseCanvasAlpha, true);
                Time.timeScale = 0;
                isGamePaused = true;
            }
            else {
                ActivateCanvas(ingameCanvas, ref ingameCanvasAlpha, true, true);
                ActivateCanvas(pauseCanvas, ref pauseCanvasAlpha, false);
                Invoke(nameof(SetTimeScale), canvasFadeDuration);
            }
        }

        // Fade transition
        if (ingameCanvasAlpha == 1f && ingameCanvasAlpha > ingameCanvas.alpha) ingameCanvas.alpha += Time.unscaledDeltaTime / canvasFadeDuration;
        else if (ingameCanvasAlpha == 0f) ingameCanvas.alpha -= Time.unscaledDeltaTime / canvasFadeDuration;
        if (pauseCanvasAlpha == 1f && pauseCanvasAlpha > pauseCanvas.alpha) pauseCanvas.alpha += Time.unscaledDeltaTime / canvasFadeDuration;
        else if (pauseCanvasAlpha == 0f) pauseCanvas.alpha -= Time.unscaledDeltaTime / canvasFadeDuration;
    }

    private void SetTimeScale() {
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void ChangeHealthBar(float currentHealth) {
        healthBar.fillAmount = currentHealth / PlayerData.Instance.maxHealth;
    }

    public static void ActivateCanvas(CanvasGroup canvas, ref float alpha, bool active, bool onlyVisual = false) {
        alpha = active ? 1f : 0f;
        if (onlyVisual) canvas.alpha = active ? 1f : 0f;
        else {
            canvas.interactable = active;
            canvas.blocksRaycasts = active;
        }
    }
}
