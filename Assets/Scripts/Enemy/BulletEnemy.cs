using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletEnemy : MonoBehaviour
{
    private float _bulletSize;
    private float _bulletSpeed;
    private Transform targetCurretLocation;
    [HideInInspector] public float _damage;
    private Vector3 _targetStartActionLocation; // the location of the target at the begining of the activation of the bullet
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

    void Update()
    {
        if (this.gameObject.activeInHierarchy) {
            _interpolateAmount = (_interpolateAmount + Time.deltaTime) * _bulletSpeed;
            Vector3 pointAB = Vector3.Lerp(transform.position, middlePoint, _interpolateAmount);
            Vector3 pointBC = Vector3.Lerp(middlePoint, _targetStartActionLocation, _interpolateAmount);
            transform.position = Vector3.Lerp(pointAB, pointBC, _interpolateAmount);
            //transform.position += _bulletSpeed * Time.deltaTime * (_targetLocation - transform.position).normalized;
        }
        if (Vector3.Distance(transform.position, targetCurretLocation.position) < _bulletSize) {
            PlayerData.Instance.TakeDamage(_damage);
            Activate(false, transform.position, null);
        }
    }

    public void Activate(bool isActive, Vector3 startPosition, Transform targetTransform, float bulletSize = 0, float bulletSpeed = 0, float bulletMaxHeight = 0) {
        transform.position = startPosition;
        targetCurretLocation = targetTransform;
        if (targetTransform != null) _targetStartActionLocation = targetTransform.position;
        _bulletSize = bulletSize;
        _bulletSpeed = bulletSpeed;
        _interpolateAmount = 0;
        middlePoint = new Vector3((_targetStartActionLocation.x + startPosition.x)/2f, startPosition.y + bulletMaxHeight, (_targetStartActionLocation.z + startPosition.z) / 2f);
        this.gameObject.SetActive(isActive);        
    }

    private void OnDrawGizmos() {
        Gizmos.color = Color.green;
        if (!UnityEditor.EditorApplication.isPlaying) Gizmos.DrawWireSphere(transform.position, _bulletSizeDebug);
        else Gizmos.DrawWireSphere(transform.position, _bulletSize);
    }
}
