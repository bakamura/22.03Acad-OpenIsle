using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyBehaviour : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Collider _hitDetection;
    [SerializeField] private Transform _attackPoint;
    private EnemyData _data;
    private EnemyAnimAndVFX _visualScript = null;
    private EnemyMovment _movmentScript = null;
    [SerializeField] private LayerMask _player;

    [Header("Status")]
    public float _damage;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private Vector3 _actionArea;
    [SerializeField] private bool _isAgressive;

    private float _actionRange;
    [HideInInspector] public bool _isActionInCooldown;
    private bool _isTargetInRange;

    private void Awake() {
        _data = GetComponent<EnemyData>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _data.cancelAttack += ActionInterupt;
        _movmentScript = GetComponent<EnemyMovment>();
        //_data.cancelAttack += DisableDetection; if with anim event
    }

    private void Start() {
        _actionRange = Vector3.Distance(transform.position, _attackPoint.position) + _actionArea.z / 2f;
        if (_movmentScript._navMeshAgent != null) _movmentScript._navMeshAgent.stoppingDistance = _actionRange;
    }

    private void Update() {
        _isTargetInRange = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= _actionRange;
        if (_isTargetInRange) {
            if (_isAgressive) _visualScript.AttackAnim(_attackSpeed);
            
            // movment lock and stop
            _movmentScript._isMovmentLocked = true;
            if (_movmentScript._isFlying) _data.rb.velocity = Vector3.zero;
            else _movmentScript._navMeshAgent.isStopped = true;

            //if (_actionCooldown == 0) _actionCooldown = _visualScript._animator.GetCurrentAnimatorStateInfo(0).length / _attackSpeed; //if is going to be set by the animation
        }
        else {
            if (!_isAgressive) EndActionSetup();
        }
    }

    private void FixedUpdate() {
        // for melee attack
        if (Physics.CheckBox(_attackPoint.position, _actionArea / 2f, Quaternion.identity, _player) && !_isActionInCooldown && _isAgressive) {
            PlayerData.Instance.TakeDamage(_damage);
            _isActionInCooldown = true;
        }
    }

    public void StartActionSetup() { // anim event
        _isActionInCooldown = false;
        //_hitDetection.enabled = true;
    }

    public void EndActionSetup() { // anim event
        if (_isAgressive) _visualScript.AttackAnim(0);
        if (!_isTargetInRange) _movmentScript._isMovmentLocked = false;
        if (!_movmentScript._isFlying && !_isTargetInRange) _movmentScript._navMeshAgent.isStopped = false;
        _isActionInCooldown = true;
        //_hitDetection.enabled = false;
    }

    private void ActionInterupt() {
        _isActionInCooldown = false;
        if (_isAgressive) _visualScript.AttackAnim(0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(_attackPoint.position, _actionArea);
    }
#endif
}
