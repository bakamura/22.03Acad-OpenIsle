using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBehaviour : MonoBehaviour {
    [Header("Components")]
    [SerializeField] private Collider _hitDetection;
    [SerializeField] private Transform _attackPoint;
    [SerializeField] private GameObject _bulletPrefab;// if is shoot
    //[SerializeField] private Transform _bulletStartPoint;// if is shoot
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
    [SerializeField] private float _actionArea;
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
        float totalDistance = Vector3.Distance(_attackPoint.position, transform.position) + _actionArea; //_actionArea.z * .5f;
        _actionRange = totalDistance;//Mathf.Sqrt(Mathf.Pow(totalDistance, 3));//due to the action area be a cube, it make that the distance to initiate the action is the hypotenuse of the cube
        //_data.cancelAttack += DisableDetection; if with collider
    }

    private void Start() {
        float totalDistance = Vector3.Distance(_attackPoint.position, transform.position) + _actionArea;//_actionArea.z * .5f;
        pointAroundPlayer = _movmentScript._isFlying ? RandomPointInsideSphere(totalDistance) : Vector3.zero;
        //new Vector3(Random.Range(-totalDistance, totalDistance) * .9f, Random.Range(totalDistance * .5f, totalDistance * .9f), Random.Range(-totalDistance, totalDistance) * .9f)
    }

    private Vector3 RandomPointInsideSphere(float radius) {
        float theta = Random.Range(0, Mathf.PI*2);
        float v = Random.Range(radius * .5f, radius * .9f);
        float phi = Mathf.Acos((2 * v) - 1) * Mathf.Rad2Deg;
        float r = Mathf.Pow(radius*.9f, 1 / 3);
        float x = r * Mathf.Sin(phi) * Mathf.Cos(theta);
        float y = Mathf.Abs(r * Mathf.Sin(phi) * Mathf.Sin(theta));
        float z = r * Mathf.Cos(phi);
        Debug.Log(new Vector3(x, y, z));
        return new Vector3(x,y,z);
        //float x = Random.Range(-byte.MaxValue, byte.MaxValue);
        //float y = Random.Range(-byte.MaxValue, byte.MaxValue);
        //float z = Random.Range(-byte.MaxValue, byte.MaxValue);
        //x *= 1 / Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        //y *= 1 / Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        //z *= 1 / Mathf.Sqrt(Mathf.Pow(x, 2) + Mathf.Pow(y, 2) + Mathf.Pow(z, 2));
        //return new Vector3(x, y, z) * radius * .9f;
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

    //detects if the target is in the range of attack
    private void FixedUpdate() {
        // action logic
        if (Physics.CheckSphere(_attackPoint.position, _actionArea, _player) && !_isActionInCooldown && !_isKamikaze && _enemyType != EnemyTypes.shoot) {
            PlayerData.Instance.TakeDamage(_damage);
            _isActionInCooldown = true;
        }
    }

    //removes the cooldow of the attack 
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
        if (!_isTargetInRange) {//if it lost its target will try to start moving
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
            bullet.GetComponent<BulletEnemy>().Activate(true, _attackPoint.position, PlayerData.Instance.transform, _bulletSize, _bulletSpeed, _bulletMaxHeighOffset, _damage);
            _bullets.Add(bullet.GetComponent<BulletEnemy>());
        }
        else {
            foreach (BulletEnemy bullet in _bullets) {
                if (!bullet.isActiveAndEnabled) bullet.Activate(true, _attackPoint.position, PlayerData.Instance.transform, _bulletSize, _bulletSpeed, _bulletMaxHeighOffset, _damage);
                break;
            }
        }
    }

    private void KamikazeAttack() {
        if (Physics.CheckSphere(_attackPoint.position, _actionArea, _player)) PlayerData.Instance.TakeDamage(_damage);
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
        Gizmos.DrawWireSphere(_attackPoint.position, _actionArea);            
        if (UnityEditor.EditorApplication.isPlaying) {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(PlayerMovement.Instance.transform.position, _actionRange);
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(PlayerMovement.Instance.transform.position + pointAroundPlayer, new Vector3(.1f, .1f, .1f));
        }
    }
#endif
}
