using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovment : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Transform _followPoint;
    private NavMeshAgent _navMeshAgent;
    private EnemyAnimAndVFX _visualScript;

    [Header("Info")]
    [SerializeField] private float _detectionRange;
    private bool _isTargetInRange = false;
    [HideInInspector] public bool _isMovmentLocked = false;
    

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
    }

    private void Update() {
        PlayerDetection();
        if (_isTargetInRange && !_isMovmentLocked) _navMeshAgent.destination = PlayerMovement.Instance.transform.position;
        //if (Input.GetKeyDown(KeyCode.Space)) _navMeshAgent.destination = _followPoint.position;
    }

    private void PlayerDetection() {
        _isTargetInRange = Vector3.Distance(PlayerMovement.Instance.transform.position, transform.position) <= _detectionRange ? true : false;
        float mov = _isTargetInRange ? 1f : 0f;
        _visualScript.MovmentAnim(mov);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
