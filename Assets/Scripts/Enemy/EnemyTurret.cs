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
    private BulletEnemyTurret[] _bulletInstances; // Object pooling

    private void Awake() {
        _dataScript = GetComponent<EnemyData>();
    }

    private void Start() {
        Transform bulletParent = new GameObject("EnemyTurretBullets").transform;
        _bulletInstances = new BulletEnemyTurret[_bulletAmount];
        for (int i = 0; i < _bulletAmount; i++) _bulletInstances[i] = Instantiate(_bulletPrefab, transform.position + _bulletSpawnPoint, Quaternion.identity, bulletParent).GetComponent<BulletEnemyTurret>();

        _dataScript.cancelAttack += CancelAttack; //
    }

    private void FixedUpdate() {
        if(Vector3.Distance(PlayerData.Instance.transform.position, transform.position) <= _attackRange && isAttacking) {
            Invoke(nameof(AttackShot), 0.5f);

        }

    }

    private void AttackShot() {


        isAttacking = false;
    }

    public void CancelAttack() {
        if (isAttacking) {
            CancelInvoke(nameof(AttackShot)); //
            isAttacking = false;
        }
    }
}
