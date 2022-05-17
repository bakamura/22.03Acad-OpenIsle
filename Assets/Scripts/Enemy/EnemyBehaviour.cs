using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Collider _hitDetection;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private Transform _bulletStartPoint;
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
    [SerializeField] private float _bulletSize;
    [SerializeField] private float _bulletSpeed;
    [SerializeField] private short _bulletAmount;
    [SerializeField, Tooltip("bullet start point in Yaxis + this value = Max Bullet Height")] private float _bulletMaxHeighOffset;
    public float actionArea;
    public bool _canWander;
    [Tooltip("if player is inside the action area, this will follow player")] public bool _willGoTowardsPlayer;
    [HideInInspector] public bool isAgressive;
    [SerializeField] private bool _isKamikaze;
    [HideInInspector] public bool isActionInCooldown;
    private bool _isTargetInRange;
    [HideInInspector] public Vector3 pointAroundPlayer { get; private set; }
    private List<BulletEnemy> _bullets;

    private void Awake() {
        _data = GetComponent<EnemyData>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _movmentScript = GetComponent<EnemyMovment>();
        _data.cancelAttack += ActionInterupt;
        isAgressive = enemyType != EnemyTypes.neutral;
        if (enemyType == EnemyTypes.shoot) _bullets = new List<BulletEnemy>();
        //_data.cancelAttack += DisableDetection; if with collider
    }

    private void Start() {
        if (_movmentScript._isFlying) pointAroundPlayer = new Vector3(Random.Range(-actionArea * .5f, actionArea * .5f), Random.Range(actionArea * .5f, actionArea * .7f), Random.Range(-actionArea * .5f, actionArea * .5f));
    }

    private void Update() {
        if (_isTargetInRange = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= actionArea && _willGoTowardsPlayer) {
            if (isAgressive) _visualScript.AttackAnim(_attackSpeed);
            // movment lock and stop            
            _movmentScript._isMovmentLocked = true;
            if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = true;
        }
        else {
            if (_willGoTowardsPlayer && !isAgressive) EndActionSetup();
        }
    }

    private void FixedUpdate() {
        // action logic
        //if (!isActionInCooldown && _willGoTowardsPlayer && !_isKamikaze) {
        //    if (enemyType == EnemyTypes.shoot) Shoot();            
        //    else {
        if (Physics.CheckSphere(_attackPoint.position, actionArea, _player) && !isActionInCooldown && _willGoTowardsPlayer && !_isKamikaze && enemyType != EnemyTypes.shoot) {
            PlayerData.Instance.TakeDamage(_damage);
            isActionInCooldown = true;
        }
        //}
        //}
    }

    public void StartActionSetup() { // anim event
        isActionInCooldown = false;
        //_hitDetection.enabled = true;
    }

    public void EndActionSetup() { // anim event        
        if (isAgressive) {
            _visualScript.AttackAnim(0);
            if (_isKamikaze) {
                if (Physics.CheckSphere(_attackPoint.position, actionArea, _player)) PlayerData.Instance.TakeDamage(_damage);
                _movmentScript._isMovmentLocked = false;
                if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = false;
                isActionInCooldown = false;
                _data.Activate(false);
            }
            else if (enemyType == EnemyTypes.shoot) {
                Shoot();
                //isActionInCooldown = false;
            }
        }
        //_hitDetection.enabled = false;
        if (!_isTargetInRange) {
            _movmentScript._isMovmentLocked = false;
            if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = false;
        }
        isActionInCooldown = true;

    }

    private void Shoot() {
        if (_bullets.Count < _bulletAmount) {
            GameObject bullet = Instantiate(_bulletPrefab, transform.position, Quaternion.identity, null);
            bullet.GetComponent<BulletEnemy>().Activate(true, _bulletStartPoint.position, PlayerData.Instance.transform, _bulletSize, _bulletSpeed, _bulletMaxHeighOffset, _damage);
            _bullets.Add(bullet.GetComponent<BulletEnemy>());
        }
        else {
            foreach (BulletEnemy bullet in _bullets) {
                if (!bullet.isActiveAndEnabled) bullet.Activate(true, _bulletStartPoint.position, PlayerData.Instance.transform, _bulletSize, _bulletSpeed, _bulletMaxHeighOffset, _damage);
                break;
            }
        }
    }

    private void ActionInterupt() {
        isActionInCooldown = false;
        if (isAgressive) _visualScript.AttackAnim(0);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected() {
        Gizmos.color = Color.black;
        Gizmos.DrawWireSphere(_attackPoint.position, actionArea);
        if (UnityEditor.EditorApplication.isPlaying) {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(PlayerMovement.Instance.transform.position + pointAroundPlayer, new Vector3(.1f, .1f, .1f));
        }
    }
#endif
}
