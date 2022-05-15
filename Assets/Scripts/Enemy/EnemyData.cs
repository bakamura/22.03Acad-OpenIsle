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
    private Vector3 _kncockbackDirection;

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
            _currentKnockBackInvencibility = _knockBackInvencibilityTime;
            cancelAttack.Invoke(); //
            Knockback();
            Invoke(nameof(StopKnockBack), _knockBackDuration);
        }
    }

    private void Knockback() {
        _visualScript.StunAnim();
        _kncockbackDirection = (transform.position - PlayerData.Instance.transform.position).normalized;
        //if (_enemyMovment._isFlying) rb.velocity = _kncockbackDirection * _knockBackAmount;
        //else {
        if (!_enemyMovment._isFlying) _enemyMovment._navMeshAgent.isStopped = true;
        _enemyMovment._isMovmentLocked = true;
        InvokeRepeating(nameof(GroundKncockBackMovment), 0, Time.fixedDeltaTime);
        //}
    }

    private void GroundKncockBackMovment() {
        transform.position += _knockBackAmount * Time.fixedDeltaTime * _kncockbackDirection;
    }

    private void StopKnockBack() {
        _visualScript.EndStunAnim();
        if (_enemyMovment != null) {
            //if (_enemyMovment._isFlying) rb.velocity = Vector3.zero;
            //else {
            CancelInvoke(nameof(GroundKncockBackMovment));
            if (!_enemyMovment._isFlying) _enemyMovment._navMeshAgent.isStopped = false;
            //}
            _enemyMovment._isMovmentLocked = false;
        }
    }
}
