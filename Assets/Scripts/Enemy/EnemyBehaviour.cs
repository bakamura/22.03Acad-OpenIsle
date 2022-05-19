using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    [Header("Components")]
    public Collider _hitDetection;
    public Transform _attackPoint;
    public GameObject _bulletPrefab;// if is shoot
    public Transform _bulletStartPoint;// if is shoot
    public LayerMask _player;//if is not passive

    private EnemyData _data;
    private EnemyAnimAndVFX _visualScript = null;
    private EnemyMovment _movmentScript = null;
    public enum EnemyTypes {
        fighter,
        shoot,
        neutral,
        passive //if only wants it to go towards player and never do nothing
    }

    [Header("Base Status")]
    public EnemyTypes enemyType;
    public float _damage;//if is not passive
    public float _attackSpeed;//if is not passive
    public float _rotationSpeed;//if goes to target
    public Vector3 actionArea;
    //public bool _canWander;
    //[Tooltip("if player is inside the action area, this will follow player")] public bool _willGoTowardsPlayer;//if is neutral or passive
    public bool _isKamikaze;//if is not passive

    [HideInInspector] public bool isAgressive;
    private bool isActionInCooldown;
    private bool _isTargetInRange;
    public float _actionRange { get; private set; }

    [Header("Range Status")]
    public float _bulletSize;//if is shoot
    public float _bulletSpeed;//if is shoot
    public int _bulletAmount;//if is shoot
    public float _bulletMaxHeighOffset;//if is shoot

    [HideInInspector] public Vector3 pointAroundPlayer { get; private set; }
    private List<BulletEnemy> _bullets;

    private void Awake() {
        _data = GetComponent<EnemyData>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _movmentScript = GetComponent<EnemyMovment>();
        _data.cancelAttack += ActionInterupt;
        isAgressive = enemyType != EnemyTypes.neutral && enemyType != EnemyTypes.passive;
        if (enemyType == EnemyTypes.shoot) _bullets = new List<BulletEnemy>();
        float totalDistance = Vector3.Distance(_attackPoint.position, transform.position) + actionArea.z * .5f;
        _actionRange = Mathf.Sqrt(Mathf.Pow(totalDistance, 2) * 2);
        //_data.cancelAttack += DisableDetection; if with collider
    }

    private void Start() {
        float totalDistance = Vector3.Distance(_attackPoint.position, transform.position) + actionArea.z * .5f;
        pointAroundPlayer = _movmentScript._isFlying ? new Vector3(Random.Range(-totalDistance, totalDistance) * .9f, Random.Range(totalDistance * .5f, totalDistance * .9f), Random.Range(-totalDistance, totalDistance) * .9f) : Vector3.zero;
    }

    private void Update() {
        
        if (_isTargetInRange = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= _actionRange && (/*_willGoTowardsPlayer ||*/ isAgressive)) {
            if (isAgressive) _visualScript.AttackAnim(_attackSpeed);
            // movment lock and stop            
            if (_movmentScript) {
                _movmentScript._isMovmentLocked = true;
                if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = true;
            }
            else if (enemyType == EnemyTypes.shoot) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(PlayerMovement.Instance.transform.position - transform.position), Time.deltaTime * _rotationSpeed);
        }
        else {
            if (/*_willGoTowardsPlayer &&*/ !isAgressive) EndActionSetup();
        }
    }

    private void FixedUpdate() {
        // action logic
        //if (!isActionInCooldown && _willGoTowardsPlayer && !_isKamikaze) {
        //    if (enemyType == EnemyTypes.shoot) Shoot();            
        //    else {
        if (Physics.CheckBox(_attackPoint.position, actionArea, Quaternion.identity,_player) && !isActionInCooldown /*&& _willGoTowardsPlayer*/ && !_isKamikaze && enemyType != EnemyTypes.shoot) {
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
                                   //if (isAgressive) {
        _visualScript.AttackAnim(0);
        if (_isKamikaze) {
            if (Physics.CheckBox(_attackPoint.position, actionArea, Quaternion.identity, _player)) PlayerData.Instance.TakeDamage(_damage);
            _movmentScript._isMovmentLocked = false;
            if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = false;
            isActionInCooldown = false;
            _data.Activate(false);
        }
        else if (enemyType == EnemyTypes.shoot) {
            Shoot();
            //isActionInCooldown = false;
        }
        //}
        //_hitDetection.enabled = false;
        if (!_isTargetInRange) {
            if (_movmentScript) {
                _movmentScript._isMovmentLocked = false;
                _movmentScript._navMeshAgent.isStopped = false;
            }
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
        if (enemyType != EnemyTypes.passive) {
            Gizmos.color = Color.black;
            Gizmos.DrawWireCube(_attackPoint.position, actionArea);
        }
        if (UnityEditor.EditorApplication.isPlaying) {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(PlayerMovement.Instance.transform.position + pointAroundPlayer, new Vector3(.1f, .1f, .1f));
        }
    }
#endif
}
