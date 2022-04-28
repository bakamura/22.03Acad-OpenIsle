using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovment : MonoBehaviour {

    [Header("Components")]

    [SerializeField] private PlayerData _playerData;

    [Header("Values")]

    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _dashSpeed = 1000f;
    [SerializeField] private float _jumpForce = 100f;
    [SerializeField] private float _maxAirVelocity = 300f;
    [SerializeField] private float _airVelocityDecrease = .25f;
    [Tooltip("How Fast the player rotates to face foward")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc;
    private float _currentDashSpeed;

    [Header("Inputs")]

    private float _horzInput;
    private float _vertcInput;
    private bool _isDashActive = false;
    private bool _isGrounded = false;

    [Header("CheckGroundDebug")]

    [SerializeField] private float _distance;
    [SerializeField] private Vector3 _boxSize = Vector3.one;
    private bool _hit;
    private RaycastHit _hitinfo;
    private float _airVelocity;

    [Header("CameraDebug")]

    [SerializeField] private KeyCode _confineCursorKey;
    private bool _isConfined = false; //

    private void Awake() {
        //_playerData = GetComponent<PlayerData>();
    }

    void Update() {
        UpdateInputs();
        if (Input.GetKeyDown(KeyCode.L)) {
            if (_isConfined) _isConfined = false;
            else _isConfined = true;
            Cursor.lockState = _isConfined ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }

    private void FixedUpdate() {
        GroundCheck();
        Vector3 TotalMovment = new Vector3(GroundMovment().x, AirMovment() / _movementSpeed, GroundMovment().z);
        _playerData.rb.velocity = TotalMovment * _movementSpeed * _currentDashSpeed;
        _currentDashSpeed = 1;
    }


    private void UpdateInputs() {
        _horzInput = Input.GetAxis("Horizontal");
        _vertcInput = Input.GetAxis("Vertical");
        if (Input.GetButtonDown("Dash")) _currentDashSpeed = _dashSpeed; //_isDashActive = true;
        if (Input.GetButtonDown("Jump") && _isGrounded) _airVelocity = _jumpForce;        
    }

    private Vector3 GroundMovment() {
        if (_horzInput != 0 || _vertcInput != 0) {
            Vector3 groundMovment = GetMovmentDirection(new Vector2(_horzInput, _vertcInput)).normalized;
            return groundMovment;
        }
        else return Vector3.zero;
    }

    private float AirMovment() {
        if (!_isGrounded) {
            _airVelocity -= _jumpForce * _airVelocityDecrease;
            Mathf.Clamp(_airVelocity, -_maxAirVelocity, _maxAirVelocity);
        }
        return _airVelocity;
    }

    private void GroundCheck() {
        _hit = Physics.BoxCast(_playerData.checkGroundPoint.position, _boxSize, -transform.up, out _hitinfo, Quaternion.identity, _distance, _playerData.groundLayer);
        if (_hit) {
            _isGrounded = true;
            if (_airVelocity < 0) _airVelocity = 0;
        }
        else _isGrounded = false;        
    }

    private Vector3 GetMovmentDirection(Vector2 direction) {
        Vector3 lookDirection = new Vector3(direction.x, 0, direction.y).normalized;
        float targetLookAngle = Mathf.Atan2(lookDirection.x, lookDirection.z) * Mathf.Rad2Deg + Camera.main.transform.eulerAngles.y;
        float smoothLookAngle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetLookAngle, ref _turnSmoothVelc, _turnSmoothTime);
        transform.rotation = Quaternion.Euler(0, smoothLookAngle, 0);
        Vector3 moveDirection = Quaternion.Euler(0, targetLookAngle, 0) * Vector3.forward;
        return moveDirection;
    }

    void OnDrawGizmos() {
        //Check if there has been a hit yet
        if (_hit) {
            Gizmos.color = Color.green;
            //Draw a Ray forward from GameObject toward the hit
            Gizmos.DrawRay(_playerData.checkGroundPoint.position, -transform.up * _hitinfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(_playerData.checkGroundPoint.position + -transform.up * _hitinfo.distance, _boxSize);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else {
            Gizmos.color = Color.red;
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(_playerData.checkGroundPoint.position, -transform.up * _hitinfo.distance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(_playerData.checkGroundPoint.position + -transform.up * _hitinfo.distance, _boxSize);
        }
    }
}
