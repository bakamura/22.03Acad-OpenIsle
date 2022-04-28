using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerData : MonoBehaviour {

    public PlayerData Instance { get; private set; }
    [Header("Components")]

    public GameObject activeToolPoint;
    public Transform checkGroundPoint;
    public LayerMask groundLayer;
    public Rigidbody rb { get; private set; }

    [Header("Abilities")]

    public bool hasSword;
    public bool hasHook;
    public bool hasAmulet;

    private void Awake() {
        if (Instance == null) Instance = this;        
        else if (Instance != this) Destroy(this);
        rb = GetComponent<Rigidbody>();
    }
}
