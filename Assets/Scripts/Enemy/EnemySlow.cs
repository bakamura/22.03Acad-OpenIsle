using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlow : MonoBehaviour {

    [Header("Components")]
    private EnemyData _dataScript;
    private Rigidbody rb;

    [Header("Info")]
    [SerializeField] private float movementSpeed;
    [System.NonSerialized] public bool isAttacking; //
    [SerializeField] private float _attackDamage;
    [SerializeField] private float _attackRange;
    [SerializeField] private float _halfAttackAngleAmplitude;

    private void Awake() {
        _dataScript = GetComponent<EnemyData>();
    }

    private void Start() {
        _dataScript.cancelAttack += CancelAttack; //
    }

    private void FixedUpdate() {
        if (!isAttacking) {
            Vector3 playerDirection = PlayerData.Instance.transform.position - transform.position;
            rb.velocity = playerDirection.normalized * movementSpeed;

            if (Vector3.Distance(PlayerData.Instance.transform.position, transform.position) <= _attackRange / 2) {
                isAttacking = true;
                rb.velocity = Vector3.zero;

                Invoke(nameof(AttackCone) , 5); // Set to animation duration
            }
        }
    }

    private void AttackCone() {
        // Cone Area Calc (May be substituted by attached object)
        if (Vector3.Distance(PlayerData.Instance.transform.position, transform.position) < _attackRange) {
            Vector3 playerDirection = PlayerData.Instance.transform.position - transform.position;
            if (Mathf.Atan2(playerDirection.z, playerDirection.x) - Mathf.Atan2(transform.forward.z, transform.forward.x) <= _halfAttackAngleAmplitude) {
                // do dmg
            }
        }

        isAttacking = false;
    }

    // To be called when KB
    public void CancelAttack() {
        if (isAttacking) {
            CancelInvoke(nameof(AttackCone)); //
            isAttacking = false;
        }
    }

}
    