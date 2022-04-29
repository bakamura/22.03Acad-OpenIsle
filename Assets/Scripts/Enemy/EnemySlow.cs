using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlow : MonoBehaviour {

    [Header("Components")]
    private EnemyData _dataScrit;
    private Rigidbody rb;

    [Header("Info")]
    [SerializeField] private float movementSpeed;
    [System.NonSerialized] public bool isAttacking; //
    [SerializeField] private float _attackRange;
    [SerializeField] private float _halfAttackAngleAmplitude;

    private void Awake() {
        _dataScrit = GetComponent<EnemyData>();
    }

    private void Start() {
        
    }

    private void FixedUpdate() {
        if (!isAttacking) {
            Vector3 playerDirection = PlayerData.Instance.transform.position - transform.position;
            rb.velocity = playerDirection.normalized * movementSpeed;

            if (Vector3.Distance(PlayerData.Instance.transform.position, transform.position) <= _attackRange / 2) {
                StartCoroutine(Attack());
            }
        }
    }

    private IEnumerator Attack() {
        isAttacking = true;
        rb.velocity = Vector3.zero;

        // Set animation

        yield return new WaitForSeconds(5f); // Set to animation duration

        // Cone Area Calc (May be substituted by attached object)
        if (Vector3.Distance(PlayerData.Instance.transform.position, transform.position) < _attackRange) {
            Vector3 playerDirection = PlayerData.Instance.transform.position - transform.position;
            if (Mathf.Atan2(playerDirection.z, playerDirection.x) - Mathf.Atan2(transform.forward.z, transform.forward.x) <= _halfAttackAngleAmplitude) {

            }
        }

        isAttacking = false;
    }

}
    