using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MonoBehaviour {

    [Tooltip("First element should be it's spawn position")]
    [SerializeField] private Vector3[] _path;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _correctionMarginDistance;
    private int _targetPoint = 1;
    private int _direction = 1;
    private bool _hasSpring = false;

    private void Start() {
        if (GetComponent<PlatformSpring>() != null) _hasSpring = true;
    }

    private void FixedUpdate() {
        if ((_path[_targetPoint] - transform.position).magnitude <= _correctionMarginDistance) {
            transform.position = _path[_targetPoint];
            if (_targetPoint == 0) _direction = 1;
            else if (_targetPoint == _path.Length - 1) _direction = -1;
            _targetPoint += _direction;
        }
        transform.position += (_path[_targetPoint] - transform.position).normalized * _movementSpeed * Time.fixedDeltaTime;
    }

    private void OnCollisionEnter(Collision collision) {
        if (!_hasSpring && collision.transform.tag == "Player" && PlayerData.rb.transform.position.y - (PlayerData.rb.transform.lossyScale.y / 2f) > transform.position.y + (transform.lossyScale.y / 2f) - 0.05f) collision.transform.parent = transform;
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.transform.tag == "Player" && collision.transform.parent == transform) collision.transform.parent = null;
    }
}
