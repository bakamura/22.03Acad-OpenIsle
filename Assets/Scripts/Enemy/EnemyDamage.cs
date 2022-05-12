using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyDamage : MonoBehaviour
{
    private EnemyMelee _meleeScript;

    private void Awake() {
        _meleeScript = GetComponent<EnemyMelee>();
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Player" && !_meleeScript._alreadyDeltDamage) {
            PlayerData.Instance.TakeDamage(_meleeScript._damage);
            _meleeScript._alreadyDeltDamage = true;
        }
    }
}
