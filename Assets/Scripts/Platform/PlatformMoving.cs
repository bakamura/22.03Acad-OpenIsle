using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMoving : MonoBehaviour {

    private bool isActive = false;

    [Header("Components")]
    private Collider _collider; // Rename?
    private MeshRenderer _mesh;

    [Tooltip("First element should be it's spawn position")]
    [SerializeField] private Vector3[] _path;
    [SerializeField] private float _movementSpeed;
    [SerializeField] private float _correctionMarginDistance;
    private int _targetPoint = 1;
    private int _direction = 1;

    private void Awake() {
        _collider = GetComponent<Collider>();
        _mesh = GetComponent<MeshRenderer>();
    }

    private void FixedUpdate() {
        if (isActive) {
            if ((_path[_targetPoint] - transform.position).magnitude <= _correctionMarginDistance) {
                transform.position = _path[_targetPoint];
                if (_targetPoint == 0) _direction = 1;
                else if (_targetPoint == _path.Length - 1) _direction = -1;
                _targetPoint += _direction;
            }
            transform.position += (_path[_targetPoint] - transform.position).normalized * _movementSpeed * Time.deltaTime;
        }
    }

    private void OnCollisionEnter(Collision collision) {
        if (collision.transform.tag == "Player") collision.transform.parent = transform;
    }

    private void OnCollisionExit(Collision collision) {
        if (collision.transform.tag == "Player" && collision.transform.parent == transform) collision.transform.parent = null;
    }

    public void Activate(bool activating) {
        isActive = activating;
        _collider.enabled = activating;
        _mesh.enabled = activating;
        if (activating) {
            transform.position = _path[0];
            _targetPoint = 1;
            _direction = 1;
        }
    }
}
