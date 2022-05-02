using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSword : MonoBehaviour {

    private void OnCollisionEnter(Collision collision) {
        if (!PlayerTools.instance.swordCollisions.Contains(collision)) {
            switch (collision.gameObject.tag) {
                case "Enemy":
                    collision.gameObject.GetComponent<EnemyData>().TakeDamage(PlayerTools.instance.swordDamage);
                    break;
                case "Breakable":
                    //case used to the vines in the plateform area and the breakable doors in puzzle area
                    break;
            }
            PlayerTools.instance.swordCollisions.Add(collision);
        }
    }
}
