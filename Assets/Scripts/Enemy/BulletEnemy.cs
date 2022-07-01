using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour {
    private float _bulletSize;
    private float _bulletSpeed;
    private Transform _targetTransform;
    private float _damage;
    private Vector3 _targetLocationAtStart; // the location of the target at the begining of the activation of the bullet
    private Vector3 _bulletLocationAtStart;
    [SerializeField] private float _bulletSizeDebug;
    private Vector3 middlePoint;
    private float _interpolateAmount;

    //private Collider _hitBox;
    //private void Awake() {
    //    _hitBox = GetComponent<Collider>();
    //}
    //private void OnCollisionEnter(Collision collision) {
    //    if (collision.gameObject.tag == targetPosition.gameObject.tag) {
    //        PlayerData.Instance.TakeDamage(_damage);
    //        Activate(false, transform.position, null);
    //    }
    //}

    void Update() {
        _interpolateAmount = (_interpolateAmount + Time.deltaTime) * _bulletSpeed;
        Vector3 pointAB = Vector3.Lerp(_bulletLocationAtStart, middlePoint, _interpolateAmount);
        Vector3 pointBC = Vector3.Lerp(middlePoint, _targetLocationAtStart, _interpolateAmount);
        transform.position = Vector3.Lerp(pointAB, pointBC, _interpolateAmount);
        float distancefromTarget = Vector3.Distance(transform.position, _targetTransform.position);
        if (distancefromTarget <= _bulletSize) PlayerData.Instance.TakeDamage(_damage);
        if (Vector3.Distance(transform.position, _targetLocationAtStart) <= _bulletSize || distancefromTarget <= _bulletSize) Activate(false, transform.position, null);
    }

    public void Activate(bool isActive, Vector3 startPosition, Transform targetTransform, float bulletSize = 0, float bulletSpeed = 0, float bulletMaxHeight = 0, float damage = 0) {
        this.gameObject.SetActive(isActive);
        transform.position = startPosition;
        _bulletLocationAtStart = startPosition;
        _targetTransform = targetTransform;
        _interpolateAmount = 0;
        _targetLocationAtStart = targetTransform ? targetTransform.position : Vector3.zero;
        _bulletSize = bulletSize;
        _bulletSpeed = bulletSpeed;
        _damage = damage;
        middlePoint = new Vector3((_targetLocationAtStart.x + startPosition.x) / 2f, startPosition.y + bulletMaxHeight, (_targetLocationAtStart.z + startPosition.z) / 2f);
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        //if (!UnityEditor.EditorApplication.isPlaying) Gizmos.DrawWireSphere(transform.position, _bulletSizeDebug);
        //else Gizmos.DrawWireSphere(transform.position, _bulletSize);
    }
}
