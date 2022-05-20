using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour {

    public static PlayerInputs Instance { get; private set; }

    [Header("Outputs")]

    [HideInInspector] public static float horizontalAxis = 0;
    [HideInInspector] public static float verticalAxis = 0;
    [HideInInspector] public static float jumpKeyPressed = 0;
    [HideInInspector] public static float dashKeyPressed = 0;
    [HideInInspector] public static float swordKeyPressed = 0;
    [HideInInspector] public static float hookKeyPressed = 0;
    [HideInInspector] public static float hookKeyReleased = 0;
    [HideInInspector] public static float amuletKeyPressed = 0;
    [HideInInspector] public static float interactKeyPressed = 0; //

    [Header("Inputs")]

    [SerializeField] private float _inputStoreDuration = 0.1f;
    public KeyCode forwardKey;
    public KeyCode backwardKey;
    public KeyCode leftKey;
    public KeyCode rightKey;
    public KeyCode jumpKey;
    public KeyCode dashKey;
    public KeyCode swordKey;
    public KeyCode hookKey;
    public KeyCode amuletKey;
    public KeyCode interactKey; // Use or not?
    [System.NonSerialized] public static bool canInput = true;

    // TO DO: Change key codes for different platforms 

    [Header("CameraDebug")]

    [SerializeField] private KeyCode _debugPauseTimeKey;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Update() {
        if (canInput) {
            // Axis
            verticalAxis = (Input.GetKey(backwardKey) ? -1 : 0) + (Input.GetKey(forwardKey) ? 1 : 0);
            horizontalAxis = (Input.GetKey(leftKey) ? -1 : 0) + (Input.GetKey(rightKey) ? 1 : 0);
            // Make axis smoothing occur in this script

            // Triggers
            if (Input.GetKeyDown(jumpKey)) jumpKeyPressed = _inputStoreDuration;
            if (Input.GetKeyDown(dashKey)) dashKeyPressed = _inputStoreDuration;
            if (Input.GetKeyDown(swordKey)) swordKeyPressed = _inputStoreDuration;
            if (Input.GetKeyDown(hookKey)) hookKeyPressed = _inputStoreDuration;
            if (Input.GetKeyUp(hookKey)) hookKeyReleased = _inputStoreDuration;
            if (Input.GetKeyDown(amuletKey)) amuletKeyPressed = _inputStoreDuration;
            if (Input.GetKeyDown(interactKey)) interactKeyPressed = _inputStoreDuration;
        }

        // Input memo vanishes even if input is locked
        jumpKeyPressed -= Time.deltaTime;
        dashKeyPressed -= Time.deltaTime;
        swordKeyPressed -= Time.deltaTime;
        hookKeyPressed -= Time.deltaTime;
        amuletKeyPressed -= Time.deltaTime;
        interactKeyPressed -= Time.deltaTime;

        // Debug
        if (Input.GetKeyDown(_debugPauseTimeKey)) {
            Time.timeScale = Time.timeScale == 0 ? 1f : 0f;
        }
    }
}
