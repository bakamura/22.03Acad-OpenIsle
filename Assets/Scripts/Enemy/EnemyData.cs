using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class EnemyData : MonoBehaviour {

    [Header("Components")]

    private EnemyMovment _enemyMovment = null;
    private EnemyAnimAndVFX _visualScript;
    private EnemyBehaviour _enemyBehaviour;

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
        _enemyMovment = GetComponent<EnemyMovment>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
        _currentHealth = _maxHealth;
    }

    private void Update() {
        _currentKnockBackInvencibility -= Time.deltaTime;
    }

    public void Activate(bool isActivating) {
        _currentHealth = _maxHealth;
        _currentKnockBackInvencibility = 0;
        cancelAttack.Invoke();
        if (_enemyBehaviour.enemyType == EnemyBehaviour.EnemyTypes.neutral) _enemyBehaviour.isAgressive = false;
        gameObject.SetActive(isActivating);
    }

    public void TakeDamage(float damageAmount) {
        _currentHealth -= damageAmount;
        if (_currentHealth <= 0) {
            Activate(false);
            return;
        }
        if (_enemyBehaviour.enemyType == EnemyBehaviour.EnemyTypes.neutral) _enemyBehaviour.isAgressive = true;
        if (_currentKnockBackInvencibility <= 0) {
            _currentKnockBackInvencibility = _knockBackInvencibilityTime;
            cancelAttack.Invoke(); //
            _visualScript.StunAnim();
            if (_enemyMovment) Knockback();
            Invoke(nameof(StopKnockBack), _knockBackDuration);
        }
    }

    private void Knockback() {
        _kncockbackDirection = (transform.position - PlayerData.Instance.transform.position).normalized;
        if (!_enemyMovment._isFlying) _enemyMovment._navMeshAgent.isStopped = true;
        _enemyMovment._isMovmentLocked = true;
        InvokeRepeating(nameof(GroundKncockBackMovment), 0, Time.fixedDeltaTime);
    }

    private void GroundKncockBackMovment() {
        transform.position += _knockBackAmount * Time.fixedDeltaTime * _kncockbackDirection;
    }

    private void StopKnockBack() {
        _visualScript.EndStunAnim();
        if (_enemyMovment) {
            CancelInvoke(nameof(GroundKncockBackMovment));
            if (!_enemyMovment._isFlying) _enemyMovment._navMeshAgent.isStopped = false;
            _enemyMovment._isMovmentLocked = false;
        }
    }
}
