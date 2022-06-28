using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviourDetection : MonoBehaviour
{
    [SerializeField] private EnemyBehaviour _behaviour;

    private void OnTriggerEnter(Collider other) {
        if(other.CompareTag("Player")) _behaviour.TargetDetected();
    }

    private void OnTriggerExit(Collider other) {
        if(other.CompareTag("Player")) _behaviour.TargetLost();
    }
}
