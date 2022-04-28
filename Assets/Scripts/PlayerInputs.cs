using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerInputs : MonoBehaviour {

    public static PlayerInputs Instance { get; private set; }

    [Header("Outputs")]

    [HideInInspector] public static float horizontalAxis = 0;
    [HideInInspector] public static float verticalAxis = 0;
    [HideInInspector] public static bool jumpKeyPressed = false;
    [HideInInspector] public static bool dashKeyPressed = false;
    [HideInInspector] public static bool swordKeyPressed = false;
    [HideInInspector] public static bool hookKeyPressed = false;
    [HideInInspector] public static bool amuletKeyPressed = false;
    [HideInInspector] public static bool interactKeyPressed = false; //

    [Header("Inputs")]

    [System.NonSerialized] public static bool canInput = true;
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

    // TO DO: Change key codes for different platforms 

    [Header("CameraDebug")]

    [SerializeField] private KeyCode _confineCursorKey;
    private bool _isConfined = false;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    private void Update() {
        if (canInput) {
            // Make axis smoothing occur in this script
            verticalAxis = (Input.GetKey(backwardKey) ? -1 : 0) + (Input.GetKey(backwardKey) ? 1 : 0);
            horizontalAxis = (Input.GetKey(leftKey) ? -1 : 0) + (Input.GetKey(rightKey) ? 1 : 0);
            // !!! Think on how to store input for a few milliseconds to provide smoother gameplay
            if (Input.GetKeyDown(jumpKey)) jumpKeyPressed = true;
            if (Input.GetKeyDown(dashKey)) dashKeyPressed = true;
            if (Input.GetKeyDown(swordKey)) swordKeyPressed = true;
            if (Input.GetKeyDown(hookKey)) hookKeyPressed = true;
            if (Input.GetKeyDown(amuletKey)) amuletKeyPressed = true;
            if (Input.GetKeyDown(interactKey)) interactKeyPressed = true;
        }

        // Debug
        if (Input.GetKeyDown(_confineCursorKey)) {
            if (_isConfined) _isConfined = false;
            else _isConfined = true;
            Cursor.lockState = _isConfined ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}
