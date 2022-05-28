using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Collider _hitDetection;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private GameObject _bulletPrefab;// if is shoot
    [SerializeField] private Transform _bulletStartPoint;// if is shoot
    [SerializeField] private LayerMask _player;//if is not passive

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
    public EnemyTypes _enemyType;
    public float _damage;//if is not passive
    [SerializeField] private float _attackSpeed;//if is not passive
    [SerializeField] private float _rotationSpeed;//if goes to target
    [SerializeField] private Vector3 _actionArea;
    [SerializeField] private bool _isKamikaze;//if is not passive

    [HideInInspector] public bool isAgressive;
    private bool _isActionInCooldown;
    private bool _isTargetInRange;
    public float _actionRange { get; private set; }

    [Header("Range Status")]
    [SerializeField] private float _bulletSize;//if is shoot
    [SerializeField] private float _bulletSpeed;//if is shoot
    [SerializeField] private int _bulletAmount;//if is shoot
    [SerializeField] private float _bulletMaxHeighOffset;//if is shoot

    [HideInInspector] public Vector3 pointAroundPlayer { get; private set; }
    private List<BulletEnemy> _bullets;

    private void Awake() {
        _data = GetComponent<EnemyData>();
        _visualScript = GetComponent<EnemyAnimAndVFX>();
        _movmentScript = GetComponent<EnemyMovment>();
        _data.cancelAttack += ActionInterupt;
        isAgressive = _enemyType != EnemyTypes.neutral && _enemyType != EnemyTypes.passive;
        if (_enemyType == EnemyTypes.shoot) _bullets = new List<BulletEnemy>();
        float totalDistance = Vector3.Distance(_attackPoint.position, transform.position) + _actionArea.z * .5f;
        _actionRange = Mathf.Sqrt(Mathf.Pow(totalDistance, 2) * 2);
        //_data.cancelAttack += DisableDetection; if with collider
    }

    private void Start() {
        float totalDistance = Vector3.Distance(_attackPoint.position, transform.position) + _actionArea.z * .5f;
        pointAroundPlayer = _movmentScript._isFlying ? new Vector3(Random.Range(-totalDistance, totalDistance) * .9f, Random.Range(totalDistance * .5f, totalDistance * .9f), Random.Range(-totalDistance, totalDistance) * .9f) : Vector3.zero;
    }

    private void Update() {
        if (_isTargetInRange = Vector3.Distance(transform.position, PlayerMovement.Instance.transform.position) <= _actionRange) {
            if (isAgressive) _visualScript.AttackAnim(_attackSpeed);
            // movment lock and stop            
            if (_movmentScript) {
                _movmentScript._isMovmentLocked = true;
                if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = true;
            }
            else if (_enemyType == EnemyTypes.shoot) transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(PlayerMovement.Instance.transform.position - transform.position), Time.deltaTime * _rotationSpeed);
        }
        else {
            if (!isAgressive) EndActionSetup();
        }
    }

    private void FixedUpdate() {
        // action logic
        if (Physics.CheckBox(_attackPoint.position, _actionArea, Quaternion.identity, _player) && !_isActionInCooldown && !_isKamikaze && _enemyType != EnemyTypes.shoot) {
            PlayerData.Instance.TakeDamage(_damage);
            _isActionInCooldown = true;
        }
    }

    public void StartActionSetup() { // anim event
        _isActionInCooldown = false;
        //_hitDetection.enabled = true;
    }

    public void EndActionSetup() { // anim event        
        if (isAgressive) {
            _visualScript.AttackAnim(0);
            if (_isKamikaze) KamikazeAttack();
            else if (_enemyType == EnemyTypes.shoot) Shoot();            
        }
        //_hitDetection.enabled = false;
        if (!_isTargetInRange) {
            if (_movmentScript) {
                _movmentScript._isMovmentLocked = false;
                if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = false;
            }
        }
        _isActionInCooldown = true;
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

    private void KamikazeAttack() {
        if (Physics.CheckBox(_attackPoint.position, _actionArea, Quaternion.identity, _player)) PlayerData.Instance.TakeDamage(_damage);
        _movmentScript._isMovmentLocked = false;
        if (_movmentScript._navMeshAgent) _movmentScript._navMeshAgent.isStopped = false;
        _isActionInCooldown = false;
        _data.Activate(false);
    }

    private void ActionInterupt() {
        _isActionInCooldown = false;
        if (isAgressive) _visualScript.AttackAnim(0);
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
