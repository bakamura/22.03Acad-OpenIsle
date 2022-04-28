using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemySlow : MonoBehaviour {

    [Header("Info")]
    private EnemyData _dataScrit;
    [System.NonSerialized] public bool isAttacking;
    [SerializeField] private float attackAngleAmplitude;

    private void Awake() {
        _dataScrit = GetComponent<EnemyData>();
    }

    private void Start() {
        
    }

    private void FixedUpdate() {
        if (!isAttacking) {
            // Calc relative pos
        }
    }

    private IEnumerator Attack() {
        
        // Set animation

        yield return new WaitForSeconds(5f); // Set to animation duration

        //get player angle to compare to angle of attack
        Mathf.Atan2(transform.forward.z, transform.forward.x);
        // calc distance too
        // (this is a cone)
    }

}
