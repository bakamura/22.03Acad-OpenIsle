using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovment : MonoBehaviour {
    [Header("Components")]
    [HideInInspector] public NavMeshAgent _navMeshAgent;
    private EnemyAnimAndVFX _visualData;
    private EnemyBehaviour _behaviourData;

    [Header("Info")]
    [SerializeField] private float _movmentSpeed;
    [SerializeField] private float _rotationSpeed;
    [SerializeField] private float _detectionRange;
    public bool _isFlying;
    [HideInInspector] public bool _isMovmentLocked;
    private bool _isTargetInRange = false;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _visualData = GetComponent<EnemyAnimAndVFX>();
        _behaviourData = GetComponent<EnemyBehaviour>();
        if (_navMeshAgent != null) _navMeshAgent.speed = _movmentSpeed;       
    }

    private void Update() {
        PlayerDetection();
        if (!_isFlying) GroundMovment();
    }

    private void FixedUpdate() {
        if (_isFlying) FlyingMovment();
    }

    private void GroundMovment() {
        if (_isTargetInRange) {
            _navMeshAgent.isStopped = false;
            _navMeshAgent.destination = PlayerMovement.Instance.transform.position;
        }
        else _navMeshAgent.isStopped = true;
    }

    private void FlyingMovment() {
        if (_isTargetInRange && !_isMovmentLocked) {
            Vector3 _movmentDirection = ((PlayerData.Instance.transform.position + _behaviourData.pointAroundPlayer) - transform.position).normalized;
            transform.position += _movmentSpeed * Time.deltaTime * _movmentDirection;
        }
    }

    private void PlayerDetection() {
        _isTargetInRange = Vector3.Distance(PlayerMovement.Instance.transform.position, transform.position) <= _detectionRange;
        if (_isTargetInRange) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(PlayerData.Instance.transform.position - transform.position), Time.deltaTime * _rotationSpeed);
        float mov = _isTargetInRange ? 1f : 0f;
        _visualData.MovmentAnim(mov);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);        
    }
}
