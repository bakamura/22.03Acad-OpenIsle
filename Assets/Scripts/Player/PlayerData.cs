using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerData : MonoBehaviour {

    public static PlayerData Instance { get; private set; }

    [Header("Components")]

    public GameObject activeToolPoint;
    public static Rigidbody rb { get; private set; }
    public static CapsuleCollider col { get; private set; } // 

    [Header("Stats")]

    public float maxHealth;
    private float _currentHealth;

    [Header("Tools")]

    public bool hasSword;
    public bool hasHook;
    public bool hasAmulet;

    [Header("CheatEngine")]

    private bool _cheatsOn = false;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);

        DontDestroyOnLoad(gameObject);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        // UNCOMENT WHEN FINISH TESTING
        // Gets info related to the player from the savefile
        //SaveData save = SaveSystem.LoadProgress(GameManager.currentSaveFile);
        //if (save != null) {
        //    transform.position = new Vector3(save.position[0], save.position[1], save.position[2]);
        //    transform.eulerAngles = new Vector3(save.rotation[0], save.rotation[1], save.rotation[2]);
        //    hasSword = save.hasSword;
        //    hasHook = save.hasHook;
        //    hasAmulet = save.hasAmulet;
        //}
    }

    private void Start() {
        _currentHealth = maxHealth; // Change to read memory when it's implemented
    }

    // Cheat Engine
    private void Update() {
        if (Input.GetKey(KeyCode.N) && Input.GetKey(KeyCode.A) && Input.GetKeyDown(KeyCode.K)) {
            _cheatsOn = !_cheatsOn;
            if (_cheatsOn) {
                print("Cheats ON");
                maxHealth = 9999999;
                _currentHealth = maxHealth;

                rb.useGravity = false;
            }
            else {
                print("Cheats OFF");
                maxHealth = 100;
                _currentHealth = maxHealth;

                rb.useGravity = true;
            }
        }
        if (_cheatsOn) {
            rb.velocity = new Vector3(rb.velocity.x, 7 * ((Input.GetKey(KeyCode.Space) ? 1 : 0) + (Input.GetKey(KeyCode.LeftControl) ? -1 : 0)), rb.velocity.z);
        }
    }

    // Takes damage 'amount' when method called by an enemy
    public void TakeDamage(float amount) {
        _currentHealth -= amount;
        UserInterface.Instance.ChangeHealthBar(_currentHealth);
        if (_currentHealth <= 0) {
            // reset etc
        }
    }
}
