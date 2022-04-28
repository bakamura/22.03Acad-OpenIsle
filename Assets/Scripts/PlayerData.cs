using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public class PlayerData : MonoBehaviour {

    public static PlayerData Instance { get; private set; }

    [Header("Components")]

    public GameObject activeToolPoint;
    public static Rigidbody rb { get; private set; }

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
