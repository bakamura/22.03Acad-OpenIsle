using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyData : MonoBehaviour {

    [Header("Components")]

    [System.NonSerialized] public Rigidbody rb;

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
            _currentKnockBackInvencibility = _knockBackInvencibilityTime;
            rb.velocity = (transform.position - PlayerData.Instance.transform.position).normalized * _knockBackAmount;
            //cancelAttack.Invoke(); //
            Invoke(nameof(StopKnockBack), _knockBackDuration);
        }
    }

    private void StopKnockBack() {
        rb.velocity = Vector3.zero;
    }

}
