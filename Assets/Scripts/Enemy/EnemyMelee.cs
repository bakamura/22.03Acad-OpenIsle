using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyMelee : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider _hitDetection;
    private EnemyData _data;
    private EnemyAnimAndVFX _visualScript;
    //[SerializeField] private Transform _atackPosition;
    //[SerializeField] private LayerMask _player;

    [Header("Status")]
    [SerializeField] private float _meleeRange;
    [SerializeField] private float _attackSpeed;
    [SerializeField] private float _damage;

    private bool _isAttacking = false;
    //private Collider[] _hits;

    private void Awake() {
        _data = GetComponent<EnemyData>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _data.cancelAttack += TakingDamage;
    }

    private void Update() {
        if (Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= _meleeRange && !_isAttacking) StartAttack();        
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player") PlayerData.Instance.TakeDamage(_damage);
    }

    private void StartAttack() {
        _isAttacking = true;        
        _hitDetection.enabled = true;
        _visualScript.AttackAnim(_attackSpeed);

    }

    public void EndAttack() {
        _hitDetection.enabled = false;
        _isAttacking = false;
    }

    private void TakingDamage() {
        _hitDetection.enabled = false;
    }

    //private void HitDetection() {
    //    //Physics.OverlapBoxNonAlloc(_atackPosition.position, new Vector3(_meleeRange, _meleeRange, _meleeRange) / 2, _hits);
    //    //foreach(Collider hit in _hits) {
    //    //
    //    //}
    //    //if (Physics.CheckBox(_atackPosition.position, new Vector3(_meleeRange, _meleeRange, _meleeRange) / 2, Quaternion.identity, _player)) PlayerData.Instance.TakeDamage(_damage);
    //}

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _meleeRange);
        //Gizmos.color = Color.green;
        //Gizmos.DrawWireCube(_atackPosition.position, new Vector3(_meleeRange, _meleeRange, _meleeRange));
    }
#endif
}
