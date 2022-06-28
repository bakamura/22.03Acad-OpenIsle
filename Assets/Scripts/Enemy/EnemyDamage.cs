using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private EnemyBehaviour _attackScript;

    private void Awake() {
        _attackScript = GetComponent<EnemyBehaviour>();
    }
    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            _attackScript.EnemyAction();
        }
    }
}
