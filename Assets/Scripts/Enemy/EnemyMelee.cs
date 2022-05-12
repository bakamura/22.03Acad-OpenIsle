using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Collider _hitDetection;
    [SerializeField] private Transform _attackPoint;
    private EnemyData _data;
    private EnemyAnimAndVFX _visualScript;
    [SerializeField] private LayerMask _player;

    [Header("Status")]
    [SerializeField] private Vector3 _meleeArea;
    [SerializeField] private float _attackSpeed;
    public float _damage;

    private bool _isAttacking = false;
    public bool _alreadyDeltDamage;

    private void Awake() {
        _data = GetComponent<EnemyData>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _data.cancelAttack += StopAttack;
        //_data.cancelAttack += DisableDetection; if with anim event
    }

    private void Update() {
        if (Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= Vector3.Distance(transform.position, _attackPoint.position) + _meleeArea.z / 2f && !_isAttacking) {
            _visualScript.AttackAnim(_attackSpeed);
            _isAttacking = true;
            Invoke(nameof(StopAttack), _visualScript._animator.GetCurrentAnimatorStateInfo(0).length / _attackSpeed);
        }
    }

    private void FixedUpdate() {
        if (_isAttacking) {
            if (Physics.CheckBox(_attackPoint.position, _meleeArea / 2f, Quaternion.identity, _player) && !_alreadyDeltDamage) {
                PlayerData.Instance.TakeDamage(_damage);
                _alreadyDeltDamage = true;
            }
        }
    }

    public void EnableDetection() { // anim event
        _hitDetection.enabled = true;
    }

    public void DisableDetection() { // anim event
        _hitDetection.enabled = false;
        _isAttacking = false;
        _alreadyDeltDamage = false;
    }

    private void StopAttack() {
        _isAttacking = false;
        _alreadyDeltDamage = false;
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(_attackPoint.position, _meleeArea);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(_atackPosition.position, new Vector3(_meleeRange, _meleeRange, _meleeRange));
    }
#endif
}
