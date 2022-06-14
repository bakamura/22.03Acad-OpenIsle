using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSword : MonoBehaviour {
    [SerializeField] private float _knockBackForce = 5f;
    [SerializeField] private float _stunDuration;
    private void OnTriggerEnter(Collider other) {
        if (!PlayerTools.instance.swordCollisions.Contains(other)) {
            switch (other.tag) {
                case "Enemy":
                    other.GetComponent<EnemyData>().TakeDamage(PlayerTools.instance.swordDamage, _knockBackForce, _stunDuration);
                    break;
                case "Breakable":
                    //case used to the vines in the platform area and the breakable doors in puzzle area
                    other.GetComponent<BreakableObjects>().DestroyObject();
                    break;
            }
            PlayerTools.instance.swordCollisions.Add(other);
        }
    }
}
