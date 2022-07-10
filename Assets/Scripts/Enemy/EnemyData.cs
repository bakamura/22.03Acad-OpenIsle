using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using System;

public class EnemyData : MonoBehaviour {

    [Header("Components")]

    private EnemyMovment _enemyMovment = null;
    private EnemyAnimAndVFX _visualScript;
    private EnemyBehaviour _enemyBehaviour;

    [Header("Info")]

    [SerializeField] private float _maxHealth;
    private float _currentHealth;
    [SerializeField, Range(0f, 1f), Tooltip("how much the knockBackForce is actually aplied")] private float _knockBackResistance;
    [SerializeField] private float _knockBackDuration;
    [SerializeField] private float _knockBackInvencibilityTime;
    private float _currentKnockBackInvencibility;
    [HideInInspector] public Action cancelAttack;
    [HideInInspector] public Action OnEnemyDefeat;
    private Vector3 _kncockbackDirection;
    private float _knockbackForce;

    private void Awake() {
        _enemyMovment = GetComponent<EnemyMovment>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _enemyBehaviour = GetComponent<EnemyBehaviour>();
        _currentHealth = _maxHealth;
    }

    private void Update() {
        _currentKnockBackInvencibility -= Time.deltaTime;
    }

    public void Activate(int isActivating) {//0 = false, 1 = true. needs to be int because its being used in animation events
        bool active = isActivating > 1;
        _currentHealth = _maxHealth;
        _currentKnockBackInvencibility = 0;
        cancelAttack.Invoke();
        if (_enemyBehaviour._enemyType == EnemyBehaviour.EnemyTypes.neutral) _enemyBehaviour.isAgressive = false;
        gameObject.SetActive(active);
    }

    public void TakeDamage(float damageAmount, float knockBackAmount, float hitStunDuration) {
        _currentHealth -= damageAmount;
        _knockbackForce = knockBackAmount;
        if (_currentHealth <= 0) {
            if(OnEnemyDefeat != null) OnEnemyDefeat.Invoke();
            _visualScript.DeathAnim();
            return;
        }
        if (_enemyBehaviour._enemyType == EnemyBehaviour.EnemyTypes.neutral) _enemyBehaviour.isAgressive = true;
        if (_currentKnockBackInvencibility <= 0) {
            _currentKnockBackInvencibility = _knockBackInvencibilityTime;
            cancelAttack.Invoke(); //
            _visualScript.StunAnim();
            if (_enemyMovment) Knockback();
            Invoke(nameof(StopKnockBack), Mathf.Clamp(hitStunDuration * _knockBackResistance, 1f, 10));
        }
    }

    private void Knockback() {
        _kncockbackDirection = (transform.position - PlayerData.Instance.transform.position).normalized;
        if (!_enemyMovment._isFlying) _enemyMovment._navMeshAgent.isStopped = true;
        _enemyMovment.SetMovmentLock(true);
        InvokeRepeating(nameof(KnockBackMovment), 0, Time.fixedDeltaTime);
    }

    private void KnockBackMovment() {
        transform.position += (_knockbackForce * _knockBackResistance) * Time.fixedDeltaTime * _kncockbackDirection;
    }

    private void StopKnockBack() {
        _visualScript.EndStunAnim();
        if (_enemyMovment) {
            CancelInvoke(nameof(KnockBackMovment));
            if (!_enemyMovment._isFlying) _enemyMovment._navMeshAgent.isStopped = false;
            _enemyMovment.SetMovmentLock(false);
        }
    }
}
