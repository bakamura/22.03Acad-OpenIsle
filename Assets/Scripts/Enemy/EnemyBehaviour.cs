using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Collider _hitDetection;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private LayerMask _player;

    private EnemyData _data;
    private EnemyAnimAndVFX _visualScript = null;
    private EnemyMovment _movmentScript = null;
    public enum EnemyTypes {
        fighter,
        shoot,
        neutral,
    }

    [Header("Status")]
    public EnemyTypes enemyType;
    public float _damage;
    [SerializeField] private float _attackSpeed;
    public Vector3 _actionArea;
    public bool _canWander;
    [Tooltip("if player is inside the action area, this will follow player")] public bool _willGoTowardsPlayer;
    public bool isAgressive;
    [SerializeField] private bool _isKamikaze;

    [HideInInspector] public float actionRange { get; private set; }
    [HideInInspector] public bool isActionInCooldown;
    private bool _isTargetInRange;
    [HideInInspector] public Vector3 pointAroundPlayer { get; private set; }

    private void Awake() {
        _data = GetComponent<EnemyData>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _movmentScript = GetComponent<EnemyMovment>();
        _data.cancelAttack += ActionInterupt;
        isAgressive = enemyType != EnemyTypes.neutral;
        //_data.cancelAttack += DisableDetection; if with collider
    }

    private void Start() {
        if (_movmentScript._isFlying) pointAroundPlayer = new Vector3(Random.Range(-_actionArea.x / 2.1f, _actionArea.x / 2.1f), Random.Range(_actionArea.y / 2.5f, _actionArea.y / 2.1f), Random.Range(-_actionArea.z / 2.1f, _actionArea.z / 2.1f));
        actionRange = Vector3.Distance(transform.position, _attackPoint.position) + _actionArea.magnitude / 2.5f;
        //if (_movmentScript._navMeshAgent != null) _movmentScript._navMeshAgent.stoppingDistance = _canWander ? _movmentScript.minDistanceFromWanderingPoint : actionRange;
    }

    private void Update() {
        if (_isTargetInRange = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= actionRange && _willGoTowardsPlayer) {
            if (isAgressive) _visualScript.AttackAnim(_attackSpeed);
            // movment lock and stop
            _movmentScript._isMovmentLocked = true;
            if (!_movmentScript._isFlying) _movmentScript._navMeshAgent.isStopped = true;
        }
        else {
            if (!_willGoTowardsPlayer) EndActionSetup();
        }
    }

    private void FixedUpdate() {
        // for melee attack
        if (Physics.CheckBox(_attackPoint.position, _actionArea / 2f, Quaternion.identity, _player) && !isActionInCooldown && _willGoTowardsPlayer && !_isKamikaze) {
            PlayerData.Instance.TakeDamage(_damage);
            isActionInCooldown = true;
        }
    }

    public void StartActionSetup() { // anim event
        isActionInCooldown = false;
        //_hitDetection.enabled = true;
    }

    public void EndActionSetup() { // anim event        
        if (_willGoTowardsPlayer) _visualScript.AttackAnim(0);
        //_hitDetection.enabled = false;

        if (_isKamikaze) {
            if (Physics.CheckSphere(_attackPoint.position, _actionArea.magnitude, _player)) PlayerData.Instance.TakeDamage(_damage);
            _movmentScript._isMovmentLocked = false;
            if (!_movmentScript._isFlying) _movmentScript._navMeshAgent.isStopped = false;
            isActionInCooldown = false;
            _data.Activate(false);
        }
        else {
            if (!_isTargetInRange) {
                _movmentScript._isMovmentLocked = false;
                if (!_movmentScript._isFlying) _movmentScript._navMeshAgent.isStopped = false;
            }
            isActionInCooldown = true;
        }
    }

    private void ActionInterupt() {
        isActionInCooldown = false;
        if (_willGoTowardsPlayer) _visualScript.AttackAnim(0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireCube(_attackPoint.position, _actionArea);
        if (UnityEditor.EditorApplication.isPlaying) {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(PlayerMovement.Instance.transform.position + pointAroundPlayer, new Vector3(.1f, .1f, .1f));
        }
    }
#endif
}
