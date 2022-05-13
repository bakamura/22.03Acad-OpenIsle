using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyMovment : MonoBehaviour {
    [Header("Components")]
    //[SerializeField] private Transform _followPoint;
    [HideInInspector] public NavMeshAgent _navMeshAgent;
    private EnemyAnimAndVFX _visualData;
    private EnemyData _data;

    [Header("Info")]
    [SerializeField] private float _speed;
    [SerializeField] private float _detectionRange;
    public bool _isFlying;
    [HideInInspector] public bool _isMovmentLocked;
    private bool _isTargetInRange = false;
    private Vector3 _lookDirection;

    private void Awake() {
        _navMeshAgent = GetComponent<NavMeshAgent>();
        _visualData = GetComponent<EnemyAnimAndVFX>();
        _data = GetComponent<EnemyData>();
        if (_navMeshAgent != null) _navMeshAgent.speed = _speed;
        //if (_isFlying) {
        //    _navMeshAgent.enabled = false;
        //    _data.rb.useGravity = false;
        //}
    }

    private void Update() {
        PlayerDetection();
        if (!_isFlying) GroundMovment();
        //if (Input.GetKeyDown(KeyCode.Space)) _navMeshAgent.destination = _followPoint.position;
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
        if (_isTargetInRange && !_isMovmentLocked) _data.rb.velocity = _lookDirection.normalized * _speed;
        else _data.rb.velocity = Vector3.zero;
    }

    private void PlayerDetection() {
        _isTargetInRange = Vector3.Distance(PlayerMovement.Instance.transform.position, transform.position) <= _detectionRange;
        if (_isTargetInRange) {
            _lookDirection = PlayerData.Instance.transform.position - transform.position;
            if (_isFlying) Quaternion.Euler(Mathf.Atan2(_lookDirection.z, _lookDirection.y) * Mathf.Rad2Deg, Mathf.Atan2(_lookDirection.x, _lookDirection.z) * Mathf.Rad2Deg, Mathf.Atan2(_lookDirection.x, _lookDirection.y) * Mathf.Rad2Deg);
            else transform.rotation = Quaternion.Euler(0, Mathf.Atan2(_lookDirection.x, _lookDirection.z) * Mathf.Rad2Deg, 0);
        }
        float mov = _isTargetInRange ? 1f : 0f;
        _visualData.MovmentAnim(mov);
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, _detectionRange);
    }
}
