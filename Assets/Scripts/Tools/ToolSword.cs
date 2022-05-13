using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolSword : MonoBehaviour {

    private void OnTriggerEnter(Collider other) {
        if (!PlayerTools.instance.swordCollisions.Contains(other)) {
            Debug.Log("do hit");
            switch (other.tag) {
                case "Enemy":
                    other.GetComponent<EnemyData>().TakeDamage(PlayerTools.instance.swordDamage);
                    break;
                case "Breakable":
                    //case used to the vines in the plateform area and the breakable doors in puzzle area
                    other.GetComponent<BreakableObjects>().DestroyObject();
                    break;
            }
            PlayerTools.instance.swordCollisions.Add(other);
        }
    }
}
