using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovment : MonoBehaviour
{
    private NavMeshAgent _navMeshAgent;
    [SerializeField] private float _detectionRange;
    [SerializeField] private Transform _followPoint;
    private bool _isTargetInRange = false;
    

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update() {
        //PlayerDetection();
        //if (_isTargetInRange) _navMeshAgent.destination = PlayerMovement.Instance.transform.position;
        if (Input.GetKeyDown(KeyCode.Space)) _navMeshAgent.destination = _followPoint.position;
    }

    private void PlayerDetection() {
        _isTargetInRange = Vector3.Distance(PlayerMovement.Instance.transform.position, transform.position) <= _detectionRange ? true : false;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
