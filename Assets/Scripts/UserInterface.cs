using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UserInterface : MonoBehaviour {

    public static UserInterface Instance { get; private set; }

    [Header("In Game")]

    [SerializeField] private CanvasGroup ingameCanvas;
    [SerializeField] private Image healthBar;
    public Image swordBtnImage; // Save system OR object interaction wil set these.
    public Image hookBtnImage;
    public Image amuletBtnImage;

    [Header("Pause Menu")]

    [SerializeField] private CanvasGroup pauseCanvas;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    public void ChangeHealthBar(float currentHealth) {
        healthBar.fillAmount = currentHealth / PlayerData.Instance.maxHealth;
    }
}
