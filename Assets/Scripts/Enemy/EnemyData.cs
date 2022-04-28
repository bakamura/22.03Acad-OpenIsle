using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyData : MonoBehaviour {

    [Header("Components")]

    [System.NonSerialized] public Rigidbody2D rb;

    [Header("Info")]

    [SerializeField] private float _maxHealth;
    private float _currentHealth;
    [SerializeField] private float _knockBackAmount;
    [SerializeField] private float _knockBackInvencibilityTime;
    private float _currentKnockBackInvencibility;

    private void Awake() {
        rb = GetComponent<Rigidbody2D>();
    }

    private void Update() {
        _currentKnockBackInvencibility -= Time.deltaTime;
    }

    public void Activate(bool isActivating) {
        rb.simulated = isActivating;

        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damageAmount) {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0) Activate(false);
        if (_currentKnockBackInvencibility <= 0) {
            _currentKnockBackInvencibility = _knockBackInvencibilityTime;
            // Calculate relative pos to player to KB
        }
    }

}
