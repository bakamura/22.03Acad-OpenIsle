using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSword : MonoBehaviour {

    [SerializeField] private float _knockBackForce = 5f;
    [SerializeField] private float _stunDuration;

    // If the collision is not in this "Swing"'s already hit list,
    // Enemy: deals damage
    // Object: destroy it (if "Breakable")
    private void OnTriggerEnter(Collider other) {
        if (!PlayerTools.instance.swordCollisions.Contains(other)) {
            switch (other.tag) {
                case "Enemy":
                    other.GetComponent<EnemyData>().TakeDamage(PlayerTools.instance.swordDamage, _knockBackForce, _stunDuration);
                    break;
                case "Breakable":
                    other.GetComponent<BreakableObjects>().DestroyObject();
                    break;
            }
            PlayerTools.instance.swordCollisions.Add(other);
        }
    }
}
