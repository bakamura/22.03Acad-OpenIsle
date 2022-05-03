using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerData : MonoBehaviour {

    public static PlayerData Instance { get; private set; }

    [Header("Components")]

    public GameObject activeToolPoint;
    public static Rigidbody rb { get; private set; }
    public static CapsuleCollider col {  get; private set; } // 

    [Header("Stats")]

    [SerializeField] private float _maxHealth;
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
    }

    private void Start() {
        _currentHealth = _maxHealth; // Change to read memory when it's implemented
    }

    private void TakeDamage(float amount) {
        _currentHealth -= amount;
    }
}
