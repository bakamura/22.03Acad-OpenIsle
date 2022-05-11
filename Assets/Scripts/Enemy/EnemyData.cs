using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyData : MonoBehaviour {

    [Header("Components")]

    [System.NonSerialized] public Rigidbody rb;
    private EnemyMovment _enemyMovment = null;
    private EnemyAnimAndVFX _visualScript;

    [Header("Info")]

    [SerializeField] private float _maxHealth;
    private float _currentHealth;
    [SerializeField] private float _knockBackAmount;
    [SerializeField] private float _knockBackDuration;
    [SerializeField] private float _knockBackInvencibilityTime;
    private float _currentKnockBackInvencibility;
    [System.NonSerialized] public UnityAction cancelAttack;

    private void Awake() {
        rb = GetComponent<Rigidbody>();
        _enemyMovment = GetComponent<EnemyMovment>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
    }

    private void Update() {
        _currentKnockBackInvencibility -= Time.deltaTime;
    }

    public void Activate(bool isActivating) {
        // rb.simulated = isActivating;

        _currentHealth = _maxHealth;
    }

    public void TakeDamage(float damageAmount) {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0) Activate(false);
        if (_currentKnockBackInvencibility <= 0) {
            if (_enemyMovment != null) _enemyMovment._isMovmentLocked = true;
            _currentKnockBackInvencibility = _knockBackInvencibilityTime;
            rb.isKinematic = false;
            rb.velocity = (transform.position - PlayerData.Instance.transform.position).normalized * _knockBackAmount;
            cancelAttack.Invoke(); //
            _visualScript.StunAnim();
            Invoke(nameof(StopKnockBack), _knockBackDuration);
        }
    }

    private void StopKnockBack() {
        if (_enemyMovment != null) _enemyMovment._isMovmentLocked = false;
        _visualScript.EndStunAnim();
        rb.isKinematic = true;
        //rb.velocity = Vector3.zero;
    }
}
