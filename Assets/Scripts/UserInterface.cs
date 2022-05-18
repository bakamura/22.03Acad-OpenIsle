using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

    public static UserInterface Instance { get; private set; }

    [Header("Debug SaveSystem")]

    [SerializeField] private bool saveTheData = false;
    [SerializeField] private bool eraseTheData = false;

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

        SaveData save = SaveSystem.LoadProgress(GameManager.currentSaveFile);
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
            SaveSystem.SaveProgress(GameManager.currentSaveFile);
        }
        if (eraseTheData) {
            eraseTheData = false;
            SaveSystem.EraseProgress(GameManager.currentSaveFile);
        }

        // Input
        if (Input.GetKeyDown(pauseKey)) {
            if (ingameCanvas.interactable) {
                ingameCanvasAlpha = ActivateCanvas(ingameCanvas, false, true);
                pauseCanvasAlpha = ActivateCanvas(pauseCanvas, true);
                Time.timeScale = 0;
                isGamePaused = true;
            }
            else {
                ingameCanvasAlpha = ActivateCanvas(ingameCanvas, true, true);
                pauseCanvasAlpha = ActivateCanvas(pauseCanvas, false);
                Invoke(nameof(SetTimeScale), canvasFadeDuration);
            }
        }

        // Fade transition
        FadeCanvas(ingameCanvas, ingameCanvasAlpha, canvasFadeDuration);
        FadeCanvas(pauseCanvas, pauseCanvasAlpha, canvasFadeDuration);
    }

    private void SetTimeScale() {
        Time.timeScale = 1;
        isGamePaused = false;
    }

    public void ChangeHealthBar(float currentHealth) {
        healthBar.fillAmount = currentHealth / PlayerData.Instance.maxHealth;
    }

    public static float ActivateCanvas(CanvasGroup canvas, bool active, bool onlyVisual = false) {
        if (onlyVisual) canvas.alpha = active ? 1f : 0f;
        else {
            canvas.interactable = active;
            canvas.blocksRaycasts = active;
        }
        return active ? 1f : 0f;
    }

    public static void FadeCanvas(CanvasGroup canvas, float targetAlpha, float fadeDuration) {
        if (targetAlpha == 1f && targetAlpha > canvas.alpha) canvas.alpha += Time.unscaledDeltaTime / fadeDuration;
        else if (targetAlpha == 0f) canvas.alpha -= Time.unscaledDeltaTime / fadeDuration;
    }
}
