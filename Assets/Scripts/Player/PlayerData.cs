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

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(this);

        rb = GetComponent<Rigidbody>();
        col = GetComponent<CapsuleCollider>();

        SaveData save = SaveSystem.LoadProgress();
        if (save != null) {
            transform.position = new Vector3(save.position[0], save.position[1], save.position[2]);
            transform.eulerAngles = new Vector3(save.rotation[0], save.rotation[1], save.rotation[2]);
            hasSword = save.hasSword;
            hasHook = save.hasHook;
            hasAmulet = save.hasAmulet;
        }
    }

    private void Start() {
        _currentHealth = maxHealth; // Change to read memory when it's implemented
    }

    public void TakeDamage(float amount) {
        _currentHealth -= amount;
        UserInterface.Instance.ChangeHealthBar(_currentHealth);
        if (_currentHealth <= 0) {
            // reset etc
        }
    }
}
