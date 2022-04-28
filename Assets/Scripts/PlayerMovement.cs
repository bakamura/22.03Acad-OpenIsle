using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovement : MonoBehaviour {

    public static PlayerMovement Instance { get; private set; }

    [Header("Movement")]

    [SerializeField] private float _movementSpeed = 5f;
    [SerializeField] private float _maxAirVelocity = 300f; // ???
    [SerializeField] private float _airVelocityDecrease = .25f; // ???
    [Tooltip("How Fast the object rotates to input")]
    [SerializeField] private float _turnSmoothTime = .1f;
    private float _turnSmoothVelc; //
    private float _currentDashSpeed; //

    [Header("Jump")]

    [SerializeField] private float _jumpStrengh = 100f;
    [SerializeField] private Vector3 groundCheckPoint;
    [SerializeField] private Vector3 _boxSize = Vector3.one;
    [SerializeField] private LayerMask groundLayer;
    private bool _isGrounded;
    // (4) Unnecessary?
    [SerializeField] private float _distance;
    private bool _hit;
    private RaycastHit _hitinfo;
    private float _airVelocity;


    [Header("Dash")]

    [SerializeField] private float _dashSpeed = 1000f;

    private void Awake() {
        if (Instance == null) Instance = this;
        else if (Instance != this) Destroy(gameObject);
    }

    void Update() {
        UpdateInputs();
    }

    private void FixedUpdate() {
        GroundCheck();

        Vector3 TotalMovment = new Vector3(GroundMovment().x, AirMovment() / _movementSpeed, GroundMovment().z);
        PlayerData.rb.velocity = TotalMovment * _movementSpeed * _currentDashSpeed;
        _currentDashSpeed = 1;
    }

    private void UpdateInputs() {
        if (PlayerInputs.dashKeyPressed) {
            PlayerInputs.dashKeyPressed = false;
            _currentDashSpeed = _dashSpeed; //_isDashActive = true;
        }
        if (PlayerInputs.jumpKeyPressed && _isGrounded) {
            PlayerInputs.jumpKeyPressed = false;
            _airVelocity = _jumpStrengh;
        }
    }

    private Vector3 GroundMovment() {
        if (PlayerInputs.horizontalAxis != 0 || PlayerInputs.verticalAxis != 0) {
            Vector3 groundMovment = GetMovmentDirection(new Vector2(PlayerInputs.horizontalAxis, PlayerInputs.verticalAxis)).normalized;
            return groundMovment;
        }
        else return Vector3.zero;
    }

    private float AirMovment() {
        if (!_isGrounded) {
            _airVelocity -= _jumpStrengh * _airVelocityDecrease;
            Mathf.Clamp(_airVelocity, -_maxAirVelocity, _maxAirVelocity);
        }
        return _airVelocity;
    }

    private void GroundCheck() {
        _hit = Physics.BoxCast(groundCheckPoint, _boxSize, -transform.up, out _hitinfo, Quaternion.identity, _distance, groundLayer);
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
            Gizmos.DrawRay(groundCheckPoint, -transform.up * _hitinfo.distance);
            //Draw a cube that extends to where the hit exists
            Gizmos.DrawWireCube(groundCheckPoint + -transform.up * _hitinfo.distance, _boxSize);
        }
        //If there hasn't been a hit yet, draw the ray at the maximum distance
        else {
            Gizmos.color = Color.red;
            //Draw a Ray forward from GameObject toward the maximum distance
            Gizmos.DrawRay(groundCheckPoint, -transform.up * _hitinfo.distance);
            //Draw a cube at the maximum distance
            Gizmos.DrawWireCube(groundCheckPoint + -transform.up * _hitinfo.distance, _boxSize);
        }
    }
}
