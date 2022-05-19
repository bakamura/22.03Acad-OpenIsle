using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private EnemyBehaviour _attackScript;

    private void Awake() {
        _attackScript = GetComponent<EnemyBehaviour>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.CompareTag("Player") /*&& !_attackScript.isActionInCooldown*/) {
            PlayerData.Instance.TakeDamage(_attackScript._damage);
            //_attackScript.isActionInCooldown = true;
        }
    }
}
