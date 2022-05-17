using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTurret : MonoBehaviour {

    [Header("Components")]
    private EnemyData _dataScript;

    [Header("Info")]
    [SerializeField] private float _attackDamage;
    [System.NonSerialized] public bool isAttacking; //
    [SerializeField] private float _attackRange;

    [SerializeField] private Vector3 _bulletSpawnPoint;
    [SerializeField] private GameObject _bulletPrefab;
    [SerializeField] private int _bulletAmount;
    private BulletEnemy[] _bulletInstances; // Object pooling

    private void Awake() {
        _dataScript = GetComponent<EnemyData>();
    }

    private void Start() {
        Transform bulletParent = new GameObject("EnemyTurretBullets").transform;
        _bulletInstances = new BulletEnemy[_bulletAmount];
        for (int i = 0; i < _bulletAmount; i++) _bulletInstances[i] = Instantiate(_bulletPrefab, transform.position + _bulletSpawnPoint, Quaternion.identity, bulletParent).GetComponent<BulletEnemy>();

        _dataScript.cancelAttack += CancelAttack; //
    }

    private void FixedUpdate() {
        if(Vector3.Distance(PlayerData.Instance.transform.position, transform.position) <= _attackRange) {
            if (isAttacking) {
                Vector3 playerDirection = PlayerData.Instance.transform.position - transform.position;
                transform.rotation = Quaternion.Euler(0, Mathf.Atan2(playerDirection.x, playerDirection.z), 0);
            }
            else {
                isAttacking = true;
                Invoke(nameof(AttackShot), 0.5f);
            }
        }

    }

    private void AttackShot() {


        isAttacking = false; // Should move elsewhere?
    }

    public void CancelAttack() {
        if (isAttacking) {
            CancelInvoke(nameof(AttackShot)); //
            isAttacking = false;
        }
    }
}
